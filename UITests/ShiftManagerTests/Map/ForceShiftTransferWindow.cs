using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftManagerTests.Map
{
    public class ForceShiftTransferWindow : ShiftMPage
    {
        public ForceShiftTransferWindow(IPage page) : base(page)
        {
            
        }

        public ILocator HostPersonComboBox => Page.GetByTestId("dx-select-box-91e0d9").GetByRole(AriaRole.Combobox);
        public ILocator EndWorkItem => Page.GetByText("Завершение работы");
        public ILocator ReasonTextBox => Page.GetByTestId("dx-text-area-84bb39");
        public ILocator ShiftTransferButton => Page.GetByTestId("dx-button-SHIFT_TRANSF");
    }
}
