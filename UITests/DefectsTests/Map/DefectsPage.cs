using System;
using System.Collections.Generic;
using System.Text;

namespace DefectsTests.Map
{
    public class DefectsPage
    {
        public IPage Page;

        public DefectsPage(IPage page)
        {
            Page = page;
        }

        public ILocator MeFilter => Page.GetByRole(AriaRole.Link, new() { Name = "Мои" });
        public ILocator NewDefectButton => Page.GetByTestId("dx-button-167032");
        public ILocator NoButton => Page.GetByTestId("dx-button-9f51f7");
        public ILocator OfflineTitle => Page.Locator(".flex-fixed.offline-logo");
    }
}
