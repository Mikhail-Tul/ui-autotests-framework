using BaseProject.Base;
using Allure.Net.Commons;

namespace DefectsTests.BaseDefects 
{ 
    [TestClass]
    public class Auth : Base
    {
        [TestInitialize]
        public async Task AuthAsync()
        {
            await AllureApi.Step("Подготовить данные и авторизация", async () =>
            {
                SubstationName = await APIContext.GetSubstationName();
                ObjectTree = await APIContext.GetSubstationTree(SubstationName);
                EquipmentName = await APIContext.GetEquipmentName(SubstationName);
                PersonTree = await APIContext.GetTreePerson(Config.PersonName);
                await AuthorizeAsync("defects");
            });
        }
    }
}
