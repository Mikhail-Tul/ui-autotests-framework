using BaseProject.Base;
using BaseProject.AllureHelpers;
using WorkOrdersTests.BaseWorkOrders;
using WorkOrdersTests.Extensions;
using static Microsoft.Playwright.Assertions;


namespace WorkOrdersTests.Tests
{
    [TestClass]
    [DataRowArgsAspect]
    [BaseWorkOrdersAttributeSmoke]
    public sealed class CreateOrder : Auth
    {
        [TestMethod]
        [Retry(2)]
        [AllureStory("Создание распоряжения")]
        [AllureTmsItem("TMS-1001")]
        [BaseAttributeSmoke]
        /*[DataRow("Испытания повышенным напряжением")]
        [DataRow("Работы без снятия напряжения")]
        [DataRow("Работы на высоте")]*/
        [DataRow("Работы под наведенным напряжением")]
        public async Task CreateOrderAsync(string workType)
        {
            await AllureApi.Step("Нажать на кнопку \"Новое распоряжение\"", async () =>
            {
                await WorkOrdersPage.OrderButton.ClickAsync();
            });
            await AllureApi.Step("Выбрать допускающего", async () =>
            {
                await Page.ChoicePersonInWindowAsync(PersonTree, "допускающего", Crew.PermitIssuer);
            });
            await AllureApi.Step("Выбрать производителя работ", async () =>
            {
                await Page.ChoicePersonInWindowAsync(PersonTree, "производителя работ", Crew.WorkProducer);
            });
            await AllureApi.Step("Выбрать наблюдающего", async () =>
            {
                await Page.ChoicePersonInWindowAsync(PersonTree, "наблюдающего", Crew.Observer);
            });
            await AllureApi.Step("Выбрать члена бригады", async () =>
            {
                await Page.ChoicePersonInWindowAsync(PersonTree, "члена бригады", Crew.CrewMember);
            });
            await AllureApi.Step("Выбрать \"Категорию работ\"", async () =>
            {
                await Expect(CreateWin.WorkTypeCombobox).ToBeVisibleAsync();
                await CreateWin.WorkTypeCombobox.ClickAsync();
                await Page.GetByText(workType).ClickAsync();
            });
            await AllureApi.Step("Добавить \"Объект\"", async () =>
            {
                await Page.AddObjectAsync(SubstationTree);
            });
            await AllureApi.Step("Добавить \"Место работ\"", async () =>
            {
                await Page.AddPlaceWorkAsync(SubstationName, EquipmentName);
            });
            await AllureApi.Step("Заполнить \"Содержание работ\"", async () =>
            {
                await Page.FillTheContentOfTheWorksAsync();
            });
            await AllureApi.Step("Выбрать \"Время начала работ\"", async () =>
            {
                await Page.FillDatesAsync(true, DateTime.Now);
            });
            await AllureApi.Step("Заполнить \"Меры по подготовке\"", async () =>
            {
                await CreateWin.MeasuresForPreparationTextBox.ClickAsync();
                await CreateWin.MeasuresForPreparationTextBox.PressSequentiallyAsync(Config.TextPattern);
            });
            await AllureApi.Step("Выбрать \"Журнал учета\"", async () =>
            {
                await CreateWin.AccountingLogDropDown.ClickAsync();
                await Page.GetByText(NameAccountingLog, new() { Exact = true }).ClickAsync();
            });
            await AllureApi.Step("Добавить \"Комментарий\"", async () =>
            {
                await CreateWin.CommentButton.ClickAsync();
                await CommentWindow.TextBox.ClickAsync();
                await CommentWindow.TextBox.PressSequentiallyAsync(Config.TextPattern);
                await CommentWindow.SaveButton.ClickAsync();
            });
            await AllureApi.Step("Выдать распоряжение", async () =>
            {
                await CreateWin.DoButton.ClickAsync();
                await CreateWin.IssueButton.ClickAsync();
                await Expect(Statuses.CreatedStat).ToBeVisibleAsync();
            });
        }

        [AllureAfter]
        [TestCleanup]
        public void Clear()
        {
            PersonTree.Clear();
            SubstationTree.Clear();
        }
    }
}
