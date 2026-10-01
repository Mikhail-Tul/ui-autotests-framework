using Allure.Net.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseProject.AllureHelpers
{
    /// <summary>
    /// Читает Allure-атрибуты с метода, класса, базовых классов и интерфейсов
    /// (включая композитные "матрёшки" и шорткаты Bdd/SuiteHierarchy)
    /// и превращает их в метки, ссылки, описание и имя теста.
    /// </summary>
    public static class AllureAttributeMapper
    {
        /// <summary>Поставь false, когда диагностика не нужна</summary>
        public static bool DebugMode = false;

        public static void ApplyAttributes(
            Allure.Net.Commons.TestResult testResult, Type testClass, MethodInfo testMethod)
        {
            var raw = CollectAttributes(testClass, testMethod);
            var attributes = ExpandComposite(raw);

            if (DebugMode)
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== DEBUG: Allure-атрибуты после распаковки ===");
                foreach (var ad in attributes
                    .Where(a => a.AttributeType.Name.StartsWith("Allure")))
                {
                    var args = string.Join(", ",
                        ad.ConstructorArguments.Select(FormatArg));
                    sb.AppendLine($"{ad.AttributeType.Name}({args})");
                }
                testResult.description = sb.ToString();
            }

            foreach (var ad in attributes)
            {
                var args = ad.ConstructorArguments;
                string v0 = GetString(args, 0);

                switch (ad.AttributeType.Name)
                {
                    // ===== BDD-иерархия (вкладка "Функционал") =====
                    case "AllureEpicAttribute":
                        AddLabel(testResult, Label.Epic(v0));
                        break;
                    case "AllureFeatureAttribute":
                        AddLabel(testResult, Label.Feature(v0));
                        break;
                    case "AllureStoryAttribute":
                        AddLabel(testResult, Label.Story(v0));
                        break;
                    case "AllureBddHierarchyAttribute":
                        AddLabel(testResult, Label.Epic(GetString(args, 0)));
                        AddLabel(testResult, Label.Feature(GetString(args, 1)));
                        AddLabel(testResult, Label.Story(GetString(args, 2)));
                        break;

                    // ===== Suite-иерархия (вкладка "Тест сюиты") =====
                    case "AllureParentSuiteAttribute":
                        AddLabel(testResult, Label.ParentSuite(v0));
                        break;
                    case "AllureSuiteAttribute":
                        AddLabel(testResult, Label.Suite(v0));
                        break;
                    case "AllureSubSuiteAttribute":
                        AddLabel(testResult, Label.SubSuite(v0));
                        break;
                    case "AllureSuiteHierarchyAttribute":
                        AddLabel(testResult, Label.ParentSuite(GetString(args, 0)));
                        AddLabel(testResult, Label.Suite(GetString(args, 1)));
                        AddLabel(testResult, Label.SubSuite(GetString(args, 2)));
                        break;

                    // ===== Метаданные =====
                    case "AllureOwnerAttribute":
                        AddLabel(testResult, Label.Owner(v0));
                        break;
                    case "AllureSeverityAttribute":
                        if (args.Count > 0 && args[0].Value != null)
                            testResult.labels.Add(
                                Label.Severity((SeverityLevel)(int)args[0].Value));
                        break;
                    case "AllureTagAttribute":
                        foreach (var tag in GetStrings(args))
                            testResult.labels.Add(Label.Tag(tag));
                        break;
                    case "AllureLabelAttribute":
                        if (args.Count >= 2)
                            testResult.labels.Add(new Label
                            {
                                name = GetString(args, 0),
                                value = GetString(args, 1)
                            });
                        break;
                    case "AllureIdAttribute":
                        if (args.Count > 0 && args[0].Value is int allureId)
                            testResult.labels.Add(Label.AllureId(allureId));
                        break;

                    // ===== Имя и описание =====
                    case "AllureNameAttribute":
                        if (v0 != null) testResult.name = v0;
                        break;
                    case "AllureDescriptionAttribute":
                        ApplyDescription(testResult, v0,
                            GetNamedBool(ad, "Append"), html: false);
                        break;
                    case "AllureDescriptionHtmlAttribute":
                        ApplyDescription(testResult, v0,
                            GetNamedBool(ad, "Append"), html: true);
                        break;

                    // ===== Ссылки =====
                    case "AllureIssueAttribute":
                        AddLink(testResult, LinkType.ISSUE, v0, GetLinkName(ad, args));
                        break;
                    case "AllureTmsItemAttribute":
                        AddLink(testResult, LinkType.TMS_ITEM, v0, GetLinkName(ad, args));
                        break;
                    case "AllureLinkAttribute":
                        AddLink(testResult, null, v0, GetLinkName(ad, args));
                        break;
                }
            }
        }

        // ---------- Сбор атрибутов: метод + класс + базы + интерфейсы ----------
        private static List<CustomAttributeData> CollectAttributes(
            Type testClass, MethodInfo testMethod)
        {
            var result = new List<CustomAttributeData>();

            if (testMethod != null)
                result.AddRange(testMethod.GetCustomAttributesData());

            for (var t = testClass; t != null && t != typeof(object); t = t.BaseType)
                result.AddRange(t.GetCustomAttributesData());

            foreach (var i in testClass.GetInterfaces())
                result.AddRange(i.GetCustomAttributesData());

            return result;
        }

        // ---------- Распаковка "матрёшки" (атрибуты на атрибутах) ----------
        private static List<CustomAttributeData> ExpandComposite(
            List<CustomAttributeData> source)
        {
            var result = new List<CustomAttributeData>(source);
            var visited = new HashSet<string>();
            var queue = new Queue<CustomAttributeData>(source);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var attrType = current.AttributeType;

                if (!visited.Add(attrType.FullName ?? attrType.Name))
                    continue;

                foreach (var nested in attrType.GetCustomAttributesData())
                {
                    result.Add(nested);
                    queue.Enqueue(nested);
                }
            }

            return result;
        }

        // ---------- Вспомогательные ----------
        private static void AddLabel(Allure.Net.Commons.TestResult tr, Label label)
        {
            if (label?.value != null)
                tr.labels.Add(label);
        }

        private static void AddLink(Allure.Net.Commons.TestResult tr, string type, string url, string name)
        {
            if (url == null) return;
            tr.links.Add(new Link { type = type, url = url, name = name });
        }

        private static void ApplyDescription(
            Allure.Net.Commons.TestResult tr, string text, bool append, bool html)
        {
            if (text == null) return;

            if (html)
            {
                tr.descriptionHtml = append && !string.IsNullOrEmpty(tr.descriptionHtml)
                    ? tr.descriptionHtml + text
                    : text;
            }
            else
            {
                tr.description = append && !string.IsNullOrEmpty(tr.description)
                    ? tr.description + "\n\n" + text
                    : text;
            }
        }

        private static string GetLinkName(
            CustomAttributeData ad, IList<CustomAttributeTypedArgument> args)
        {
            if (args.Count >= 2) return GetString(args, 1);
            return GetNamedString(ad, "Name");
        }

        private static string GetNamedString(CustomAttributeData ad, string member)
        {
            var na = ad.NamedArguments.FirstOrDefault(n => n.MemberName == member);
            return na.TypedValue.Value as string;
        }

        private static bool GetNamedBool(CustomAttributeData ad, string member)
        {
            var na = ad.NamedArguments.FirstOrDefault(n => n.MemberName == member);
            return na.TypedValue.Value is bool b && b;
        }

        private static string FormatArg(CustomAttributeTypedArgument arg)
        {
            if (arg.Value is int i && arg.ArgumentType.IsEnum)
                return Enum.ToObject(arg.ArgumentType, i).ToString();
            return arg.Value?.ToString() ?? "null";
        }

        private static string GetString(
            IList<CustomAttributeTypedArgument> args, int index)
        {
            return args.Count > index && args[index].Value is string s ? s : null;
        }

        private static IEnumerable<string> GetStrings(
            IList<CustomAttributeTypedArgument> args)
        {
            if (args.Count == 1 && args[0].Value is string single)
            {
                yield return single;
            }
            else if (args.Count == 1 && args[0].Value is System.Collections.IEnumerable list)
            {
                foreach (var item in list)
                {
                    if (item is CustomAttributeTypedArgument typed && typed.Value is string s)
                        yield return s;
                }
            }
        }
    }
}