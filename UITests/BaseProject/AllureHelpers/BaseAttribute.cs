using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;

namespace BaseProject.AllureHelpers
{
    [TestClass]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureTag("smoke")]
    public class BaseAttributeSmoke : AllureMetaAttribute
    {
    }
}
