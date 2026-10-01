using AspectInjector.Broker;
using BaseProject.AllureHelpers;
using System;
using System.Linq;
using System.Reflection;

namespace BaseProject.AllureHelpers
{
    /// <summary>
    /// Повесить на тестовый класс: автоматически передаёт в Allure-отчёт
    /// аргументы каждого тест-метода (значения DataRow / DynamicData).
    /// Работает через AspectInjector — тот же механизм, что и [AllureStep].
    /// </summary>
    [Aspect(Scope.Global)]
    [Injection(typeof(DataRowArgsAspect))]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class DataRowArgsAspect : Attribute
    {
        [Advice(Kind.Before, Targets = Target.Method)]
        public void ReportArgs(
            [Argument(Source.Method)] MethodBase method,
            [Argument(Source.Arguments)] object[] args)
        {
            if (args == null || args.Length == 0) return;

            // Реагируем только на тест-методы,
            // чтобы вспомогательные методы с параметрами не шумели
            bool isTest = method.GetCustomAttributes(true)
                .Any(a => a.GetType().Name is "TestMethodAttribute"
                                       or "DataTestMethodAttribute");
            if (!isTest) return;

            AllureBaseTest.ReportDataRowStatic(method, args);
        }
    }
}