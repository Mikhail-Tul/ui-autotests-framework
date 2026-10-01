using BaseProject.AllureHelpers;
using WorkOrdersTests.BaseWorkOrders;
using WorkOrdersTests.Extensions;
using static Microsoft.Playwright.Assertions;

namespace WorkOrdersTests.Tests
{
    [TestClass]
    [DataRowArgsAspect]
    [BaseWorkOrdersAttributeSmoke]
    public sealed class CreateRecordEU : Auth
    {
        [TestMethod]
        [Retry(2)]
        [AllureStory("Создание наряда на ЭУ")]
        [AllureTmsItem("TMS-1002")]
        [BaseAttributeSmoke]
        /*[DataRow("Испытания повышенным напряжением")]
        [DataRow("Работы без снятия напряжения")]
        [DataRow("Работы на высоте")]*/
        [DataRow("Работы под наведенным напряжением")]
        public async Task CreateRecord(string workTypes)
        {
            await AllureApi.Step("Нажать на \"Новый наряд-допуск для работы в ЭУ\"", async () =>
            {
                await WorkOrdersPage.EUButton.ClickAsync();

            });
            await AllureApi.Step("Выбрать руководителя работ", async () =>
            {
                await Page.ChoicePersonInWindowAsync(PersonTree, "руководителя работ", Crew.WorkSupervisor);

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
            await AllureApi.Step("Выбрать категорию работ", async () =>
            {
                await Expect(CreateWin.WorkTypeCombobox).ToBeVisibleAsync();
                await CreateWin.WorkTypeCombobox.ClickAsync();
                await Page.GetByText(workTypes).ClickAsync();

            });
            await AllureApi.Step("Выбрать \"Объект\"", async () =>
            {
                await Page.AddObjectAsync(SubstationTree);

            });
            await AllureApi.Step("Выбрать \"Место работ\"", async () =>
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
            await AllureApi.Step("Выбрать \"Время окончания работ\"", async () =>
            {
                await Page.FillDatesAsync(false, DateTime.Now.AddDays(1));

            });
            await AllureApi.Step("Заполнить \"Наименования электроустановок\"", async () =>
            {
                await CreateWin.NameOfElectricalInstallationsTextBox.ClickAsync();
                await CreateWin.NameOfElectricalInstallationsTextBox.PressSequentiallyAsync(Config.TextPattern);

            });
            await AllureApi.Step("Запонить \"Отдельные указания\"", async () =>
            {
                await CreateWin.SeparateInstructionsTextBox.ClickAsync();
                await CreateWin.SeparateInstructionsTextBox.PressSequentiallyAsync(Config.TextPattern);

            });
            await AllureApi.Step("Выбрать \"Журнал учета\"", async () =>
            {
                await CreateWin.AccountingLogDropDown.ClickAsync();
                await Page.GetByText(NameAccountingLog, new() { Exact = true }).ClickAsync();

            });
            await AllureApi.Step("Заполнить \"Комментарий\"", async () =>
            {
                await CreateWin.CommentButton.ClickAsync();
                await CommentWindow.TextBox.ClickAsync();
                await CommentWindow.TextBox.PressSequentiallyAsync(Config.TextPattern);
                await CommentWindow.SaveButton.ClickAsync();

            });
            await AllureApi.Step("Выдать наряд", async () =>
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
