using System;
using System.Collections.Generic;
using System.Text;

namespace DefectsTests.Map
{
    public class SelectionOfAnObjectWin : CreateWin
    {
        public SelectionOfAnObjectWin(IPage page) : base(page) { }

        public ILocator SelectButton => Page.GetByTestId("dx-button-0f8003");
        public ILocator ScrollAria => Page.GetByRole(AriaRole.Dialog)
                        .Filter(new() { HasText = "Выбор оборудования" })
                        .Locator(".dx-scrollable-container");
        public ILocator FullTreeSwitch => Page.GetByTestId("dx-switch-a18829");
    }
}
