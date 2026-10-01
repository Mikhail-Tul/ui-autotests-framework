using BaseProject.Base;

namespace ShiftHandoverTests.Maps
{
    public class ShiftHandPage
    {
        public IPage Page;

        public ShiftHandPage(IPage page)
        {
            Page = page;
        }

        public ILocator RoleTextBox => Page.GetByTestId("dx-text-box-16043e").GetByRole(AriaRole.Textbox);
        public ILocator BeginPasswordTextBox => Page.GetByTestId("dx-text-box-49c875").GetByRole(AriaRole.Textbox);
        public ILocator BeginOrEndWorkButton => Page.GetByTestId("dx-button--SHIFT_ACCEP");
        public ILocator EndPasswordTextBox => Page.GetByTestId("dx-text-box-629a50").GetByRole(AriaRole.Textbox);
        public ILocator RaportTextBox => Page.GetByRole(AriaRole.Application).GetByRole(AriaRole.Textbox);
        public ILocator Role => Page.GetByRole(AriaRole.Option, new() { NameRegex = new(@$".*\s{Config.ShiftRole}$") });
        public ILocator OfflineTitle => Page.GetByText("Автономный режим");
        public ILocator OfflineTitleBox => Page.Locator(".nav_title box offline");
    }
}
