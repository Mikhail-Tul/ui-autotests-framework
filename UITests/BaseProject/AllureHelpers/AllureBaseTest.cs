using Allure.Net.Commons;
using BaseProject.AllureHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BaseProject.AllureHelpers
{
    [TestClass]
    public class AllureBaseTest
    {
        protected static AllureLifecycle Allure => AllureLifecycle.Instance;
        protected string TestUuid;
        protected MethodInfo CurrentMethod;

        // Карточка теста: лайфцикл мутирует именно этот объект,
        // поэтому в Cleanup мы видим все шаги со статусами и statusDetails
        protected Allure.Net.Commons.TestResult CurrentTestResult;

        // ============================================================
        // БЛОК 1. Один раз на процесс тестов:
        // архивация прошлого прогона, ретенция по глубине, маркер сессии,
        // общая папка результатов, копирование categories.json
        // ============================================================
        static AllureBaseTest()
        {
            // 0. Где мы вообще
            string solutionRoot = null;
            try { solutionRoot = GetSolutionRoot(); }
            catch (Exception ex) { InitLog("Ошибка GetSolutionRoot: " + ex.Message); }

            InitLog($"=== Начало инициализации, solutionRoot={solutionRoot ?? "НЕ НАЙДЕН"} ===");

            // 1. Мьютекс в ОТДЕЛЬНОМ try/catch: его проблемы не должны убивать инициализацию
            System.Threading.Mutex mux = null;
            bool owned = false;
            try
            {
                mux = new System.Threading.Mutex(false, "AllureSessionArchiveMutex");
                try
                {
                    owned = mux.WaitOne(TimeSpan.FromSeconds(10));
                }
                catch (System.Threading.AbandonedMutexException)
                {
                    // Мьютекс "брошен" убитым testhost-процессом прошлого прогона —
                    // он уже наш, спокойно продолжаем
                    owned = true;
                }
                InitLog($"mutex: owned={owned}");
            }
            catch (Exception ex)
            {
                InitLog("Мьютекс недоступен, продолжаем без него: " + ex.Message);
            }

            // 2. Архивация и ретенция — свой try/catch, это необязательная часть
            try
            {
                if (!string.IsNullOrEmpty(solutionRoot))
                {
                    string resultsPath = Path.Combine(solutionRoot, "AllureResults");
                    string historyRoot = Path.Combine(solutionRoot, "AllureHistory");
                    string markerPath = Path.Combine(solutionRoot, ".allure-session");

                    int retentionDepth = ReadIntConfig("retentionDepth", 5);
                    int sessionTimeoutMinutes = ReadIntConfig("sessionTimeoutMinutes", 120);
                    InitLog($"Настройки: depth={retentionDepth}, timeout={sessionTimeoutMinutes}");

                    if (IsNewSession(markerPath, sessionTimeoutMinutes))
                    {
                        ArchiveResults(resultsPath, historyRoot);
                        EnforceRetention(historyRoot, retentionDepth);
                    }

                    File.WriteAllText(markerPath, DateTime.UtcNow.ToString("o"));
                }
            }
            catch (Exception ex)
            {
                InitLog("Ошибка архивации/ретенции (некритично): " + ex);
            }
            finally
            {
                try { if (owned && mux != null) mux.ReleaseMutex(); } catch { }
                try { mux?.Dispose(); } catch { }
            }

            // 3. ОБЯЗАТЕЛЬНАЯ часть: папка, конфиг, категории — каждый шаг независим
            try
            {
                if (string.IsNullOrEmpty(solutionRoot))
                {
                    InitLog("КРИТИЧНО: корень решения не найден, Allure будет писать в bin");
                    return;
                }

                string resultsPath = Path.Combine(solutionRoot, "AllureResults");
                if (!Directory.Exists(resultsPath))
                {
                    Directory.CreateDirectory(resultsPath);
                    InitLog("Создана папка: " + resultsPath);
                }

                RewriteConfigDirectory(resultsPath);
                InitLog("allureConfig.json переписан, directory=" + resultsPath);

                TryCopyCategories(solutionRoot, resultsPath);
                InitLog("=== Инициализация завершена ===");
            }
            catch (Exception ex)
            {
                InitLog("КРИТИЧЕСКАЯ ошибка инициализации: " + ex);
                System.Diagnostics.Debug.WriteLine($"[Allure] Ошибка инициализации: {ex}");
            }
        }

        // ============================================================
        // БЛОК 2. Перед каждым тестом
        // ============================================================
        [TestInitialize]
        public void Setup()
        {
            TestUuid = Guid.NewGuid().ToString("N");

            string className = TestContext.FullyQualifiedTestClassName;
            if (string.IsNullOrEmpty(className)) className = GetType().FullName;
            string fullName = $"{className}.{TestContext.TestName}";

            var testResult = new Allure.Net.Commons.TestResult
            {
                uuid = TestUuid,
                name = TestContext.TestName,
                fullName = fullName
            };

            // Всё опциональное — в try/catch: ошибка здесь не может убить карточку теста
            try
            {
                string methodName = TestContext.TestName.Split(' ')[0];
                MethodInfo method = GetType().GetMethod(
                    methodName, BindingFlags.Public | BindingFlags.Instance);
                CurrentMethod = method;

                string vsName = TryGetVsDisplayName();
                if (!string.IsNullOrEmpty(vsName)) testResult.name = vsName;

                AllureAttributeMapper.ApplyAttributes(testResult, GetType(), method);

                var rowValues = TryGetDataRowValues();
                if (rowValues != null) AttachDataRowCore(testResult, method, rowValues);
            }
            catch (Exception ex)
            {
                InitLog("Ошибка Setup: " + ex);
                System.Diagnostics.Debug.WriteLine($"[Allure] Ошибка Setup: {ex}");
            }

            CurrentTestResult = testResult;
            Allure.StartTestCase(testResult);   // выполняется ВСЕГДА
        }

        // ============================================================
        // БЛОК 3. После каждого теста: статус + statusDetails из упавшего шага
        // ============================================================
        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                var outcome = TestContext.CurrentTestOutcome;

                Allure.UpdateTestCase(tc =>
                {
                    if (outcome == UnitTestOutcome.Passed)
                    {
                        tc.status = Status.passed;
                        return;
                    }

                    var failedStep = FindFailedStep(CurrentTestResult?.steps);

                    if (failedStep != null)
                    {
                        tc.status = failedStep.status == Status.broken
                            ? Status.broken
                            : Status.failed;
                        tc.statusDetails = failedStep.statusDetails;
                    }
                    else
                    {
                        tc.status = Status.failed;
                        tc.statusDetails = new StatusDetails
                        {
                            message = $"Тест завершился с исходом: {outcome}"
                        };
                    }
                });

                Allure.StopTestCase();
                Allure.WriteTestCase();
            }
            catch (Exception ex)
            {
                InitLog("Ошибка Cleanup: " + ex);
                System.Diagnostics.Debug.WriteLine($"[Allure] Ошибка Cleanup: {ex}");
            }
            finally
            {
                TouchSessionMarker();
            }
        }

        // ============================================================
        // DATAROW / DYNAMICDATA
        // ============================================================

        // Вызывается аспектом DataRowArgsAspect (автоматический путь)
        public static void ReportDataRowStatic(MethodBase method, object[] values)
        {
            if (method == null || values == null || values.Length == 0) return;

            var paramNames = method.GetParameters().Select(p => p.Name).ToArray();
            var parts = new List<string>();

            for (int i = 0; i < values.Length; i++)
            {
                string pname = i < paramNames.Length && paramNames[i] != null
                    ? paramNames[i]
                    : "arg" + i;

                AllureApi.AddTestParameter(pname, values[i]);
                parts.Add(FormatValue(values[i]));
            }

            // Имя как в Test Explorer: CreateOrderAsync ("Испытания повышенным напряжением")
            AllureApi.SetTestName(method.Name + " (" + string.Join(", ", parts) + ")");
        }

        // Ручной путь: одна строка ReportDataRow(...) первой строкой теста
        protected void ReportDataRow(params object[] values)
        {
            AttachDataRowCore(CurrentTestResult, CurrentMethod, values);
        }

        private static void AttachDataRowCore(Allure.Net.Commons.TestResult tr, MethodBase method, object[] values)
        {
            if (tr == null || values == null || values.Length == 0) return;

            var paramNames = method?.GetParameters().Select(p => p.Name).ToArray()
                             ?? Array.Empty<string>();

            for (int i = 0; i < values.Length; i++)
            {
                string pname = i < paramNames.Length && paramNames[i] != null
                    ? paramNames[i]
                    : "arg" + i;

                if (tr.parameters.Any(p => p.name == pname)) continue; // без дублей

                tr.parameters.Add(new Parameter
                {
                    name = pname,
                    value = FormatValue(values[i])
                });
            }

            string suffix = "(" + string.Join(", ", values.Select(FormatValue)) + ")";
            if (!tr.name.EndsWith(")"))
                tr.name = tr.name + " " + suffix;
        }

        private static string FormatValue(object v)
        {
            if (v == null) return "null";
            if (v is string s) return "\"" + s + "\"";
            return v.ToString();
        }

        // Попытка достать значения строки средствами MSTest (если версия отдаёт)
        private object[] TryGetDataRowValues()
        {
            var ctxType = TestContext.GetType();
            var drProp = ctxType.GetProperty("DataRow");
            if (drProp != null)
            {
                var dr = drProp.GetValue(TestContext);
                if (dr is object[] arr) return arr;
                if (dr is System.Data.DataRow row) return row.ItemArray;
            }
            return null;
        }

        // DisplayName как в VS (если MSTest его отдаёт)
        private string TryGetVsDisplayName()
        {
            var ctxType = TestContext.GetType();
            var dnProp = ctxType.GetProperty("TestDisplayName");
            if (dnProp != null)
            {
                var dn = dnProp.GetValue(TestContext) as string;
                if (!string.IsNullOrWhiteSpace(dn)) return dn;
            }
            if (TestContext.TestName != null && TestContext.TestName.Contains("("))
                return TestContext.TestName;
            return null;
        }

        // ============================================================
        // РЕТЕНЦИЯ И СЕССИИ
        // ============================================================

        private static bool IsNewSession(string markerPath, int timeoutMinutes)
        {
            if (!File.Exists(markerPath)) return true;

            var text = File.ReadAllText(markerPath).Trim();
            if (DateTime.TryParse(
                    text, null,
                    System.Globalization.DateTimeStyles.RoundtripKind,
                    out var last))
            {
                return (DateTime.UtcNow - last.ToUniversalTime()).TotalMinutes > timeoutMinutes;
            }
            return true;
        }

        private static void ArchiveResults(string resultsPath, string historyRoot)
        {
            try
            {
                if (!Directory.Exists(resultsPath)) return;
                if (!Directory.EnumerateFileSystemEntries(resultsPath).Any()) return;

                if (!Directory.Exists(historyRoot))
                    Directory.CreateDirectory(historyRoot);

                string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string archivePath = Path.Combine(historyRoot, stamp);
                int n = 1;
                while (Directory.Exists(archivePath))
                    archivePath = Path.Combine(historyRoot, stamp + "_" + (++n));

                Directory.Move(resultsPath, archivePath);
            }
            catch (Exception ex)
            {
                // Например, allure serve держит файлы открытыми — пропускаем архивацию
                System.Diagnostics.Debug.WriteLine($"[Allure] Архивация пропущена: {ex.Message}");
            }
        }

        private static void EnforceRetention(string historyRoot, int depth)
        {
            if (!Directory.Exists(historyRoot)) return;

            var archives = Directory.GetDirectories(historyRoot)
                .OrderByDescending(d => Path.GetFileName(d))
                .ToList();

            foreach (var old in archives.Skip(Math.Max(depth, 0)))
            {
                try { Directory.Delete(old, recursive: true); }
                catch { /* папка занята — удалим в следующий раз */ }
            }
        }

        private static void TouchSessionMarker()
        {
            try
            {
                string root = GetSolutionRoot();
                if (root == null) return;
                File.WriteAllText(
                    Path.Combine(root, ".allure-session"),
                    DateTime.UtcNow.ToString("o"));
            }
            catch { /* маркер не критичен */ }
        }

        // ============================================================
        // КОНФИГУРАЦИЯ И СЛУЖЕБНЫЕ
        // ============================================================

        private static int ReadIntConfig(string key, int defaultValue)
        {
            try
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, "allureConfig.json");
                if (!File.Exists(configPath)) return defaultValue;

                var m = Regex.Match(
                    File.ReadAllText(configPath),
                    $"\"{key}\"\\s*:\\s*(\\d+)");
                return m.Success ? int.Parse(m.Groups[1].Value) : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static void RewriteConfigDirectory(string resultsPath)
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "allureConfig.json");
            string json = File.Exists(configPath) ? File.ReadAllText(configPath) : "{}";
            string escapedPath = resultsPath.Replace("\\", "\\\\");
            var regex = new Regex("\"directory\"\\s*:\\s*\"[^\"]*\"");

            if (regex.IsMatch(json))
                json = regex.Replace(json, $"\"directory\": \"{escapedPath}\"");
            else if (json.Contains("\"allure\""))
                json = json.Replace("\"allure\": {", $"\"allure\": {{\n    \"directory\": \"{escapedPath}\",");
            else
                json = $"{{\n  \"allure\": {{\n    \"directory\": \"{escapedPath}\"\n  }}\n}}";

            File.WriteAllText(configPath, json);
        }

        private static void TryCopyCategories(string solutionRoot, string resultsPath)
        {
            string[] candidates =
            {
                Path.Combine(AppContext.BaseDirectory, "categories.json"),
                Path.Combine(solutionRoot, "categories.json")
            };

            string src = candidates.FirstOrDefault(File.Exists);
            if (src == null) return;

            try
            {
                File.Copy(src, Path.Combine(resultsPath, "categories.json"), overwrite: true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Allure] categories.json не скопирован: {ex.Message}");
            }
        }

        private static string GetSolutionRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !directory.GetFiles("*.slnx").Any())
                directory = directory.Parent;
            return directory?.FullName ?? AppContext.BaseDirectory;
        }

        private static StepResult FindFailedStep(IEnumerable<StepResult> steps)
        {
            if (steps == null) return null;

            foreach (var step in steps)
            {
                var nested = FindFailedStep(step.steps);
                if (nested != null) return nested;

                if (step.status == Status.failed || step.status == Status.broken)
                    return step;
            }

            return null;
        }

        private static readonly object LogSync = new object();

        /// <summary>
        /// Лог инициализации и критичных сбоев: файл allure-init.log в корне решения.
        /// Больше никаких "тихих" падений.
        /// </summary>
        internal static void InitLog(string msg)
        {
            try
            {
                string root = GetSolutionRoot() ?? AppContext.BaseDirectory;
                string path = Path.Combine(root, "allure-init.log");
                lock (LogSync)
                {
                    File.AppendAllText(path,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg + Environment.NewLine);
                }
            }
            catch { /* лог не критичен */ }
        }

        public TestContext TestContext { get; set; }
    }
}