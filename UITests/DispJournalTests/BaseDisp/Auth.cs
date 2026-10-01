using BaseProject.Base;
using Allure.Net.Commons;

namespace DispJournalTests.BaseDisp
{ 
    [TestClass]
    public class Auth :  Base
    {
        [TestInitialize]
        public async Task AuthAsync()
        {
            await AllureApi.Step("Подготовить данные и авторизация", async () =>
            {
                await AuthorizeAsync("dispjournal");
            });
        }
    }
}
