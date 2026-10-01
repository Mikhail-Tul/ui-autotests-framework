using BaseProject.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftManagerTests.Map
{
    public class ShiftMPage
    {
        public IPage Page;

        public ShiftMPage(IPage page)
        {
            Page = page;
        }

        public ILocator DisplaySettingsDropDown => Page.GetByText("Настройки отображения");
        public ILocator OnlyWithTheShiftStaffCheckBox => Page.GetByRole(AriaRole.Checkbox, new() { Name = "Только с персоналом на смене" });
        public ILocator PersonGridcell => Page.GetByRole(AriaRole.Gridcell, new() { Name = Config.PersonName, Exact = true });
        public ILocator ForceShiftTransferButton => Page.GetByTestId("dx-button-fa-fa-arrows");
    }
}
