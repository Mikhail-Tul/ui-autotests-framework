using BaseProject.Base;
using BaseProject.Extensions;
using WorkOrdersTests.Extensions;
using WorkOrdersTests.Map;
using static Microsoft.Playwright.Assertions;

namespace WorkOrdersTests.BaseWorkOrders
{
    [TestClass]
    /// <summary>
    /// Класс авторизации.
    /// Выполняет переход на страницу приложения и инициализацию окна авторизации.
    /// </summary>
    public class Auth : WorkOrderBaseClass
    {
        [TestInitialize]
        [AllureStep("Открыть главную страницу приложения")]
        /// <summary>
        /// Метод инициализации авторизации, запускаемый перед тестами.
        /// </summary>
        public async Task Init()
        {
            await AllureApi.Step("Подготовить данные и авторизация", async () =>
            {
                PersonTree = await APIContext.GetTreePerson(Config.PersonName);
                SubstationName = await APIContext.GetSubstationName();
                SubstationTree = await APIContext.GetSubstationTree(SubstationName);
                EquipmentName = await APIContext.GetEquipmentName(SubstationName);
                NameAccountingLog = await APIContext.GetAccountingLog();
                await AuthorizeAsync("workorders");
                await Expect(WorkOrdersPage.CountPageIndicator).ToBeVisibleAsync(new() { Timeout = 30000 });
                await Expect(WorkOrdersPage.NoButton).ToBeInViewportAsync(new() { Timeout = 30000 });
                await WorkOrdersPage.NoButton.ClickAsync();
            });
        }
    }
}
