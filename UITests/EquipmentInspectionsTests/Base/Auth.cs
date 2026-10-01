namespace EquipmentInspectionsTests.Base
{
    [TestClass]
    public class Auth : Base
    {
        [TestInitialize]
        [AllureStep("Открыть главную страницу приложения")]
        public async Task AuthAsync()
        {
            await AllureApi.Step("Подготовить данные и авторизация", async () =>
            {
                await AuthorizeAsync("equipmentinspections");
            });
        }
    }
}
