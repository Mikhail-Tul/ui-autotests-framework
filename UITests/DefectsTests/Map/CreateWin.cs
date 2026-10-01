using System;
using System.Collections.Generic;
using System.Text;

namespace DefectsTests.Map
{
    public class CreateWin : DefectsPage
    {
        public CreateWin(IPage page) : base(page) { }

        public ILocator TypeOfDefectCombobox => Page.GetByTestId("dx-select-box-c6cd9c");
        public ILocator DescriptionDefectsTextBox => Page.GetByTestId("dx-text-area-043bc7");
        public ILocator ObjectBookButton => Page.GetByTestId("dx-button-be5a9d");
        public ILocator ObjectTextBox => Page.GetByTestId("dx-text-box-7775c8");
        public ILocator EquipmentTextBox => Page.GetByTestId("dx-text-box-1e2034");
        public ILocator EquipmentBookButton => Page.GetByTestId("dx-button-2ab749");
        public ILocator ResponsibleDepartmentTextBox => Page.GetByPlaceholder("Ответственное подразделение");
        public ILocator ResponsibleDepartmentBookButton => Page.GetByTitle("Ответственное подразделение").GetByTestId("dx-button-bf5b20");
        public ILocator ResponsibleTextBox => Page.GetByPlaceholder("Ответственный");
        public ILocator ResponsibleBookButton => Page.GetByTitle("Ответственный").GetByTestId("dx-button-bf5b20");
        public ILocator SaveButton => Page.GetByTestId("dx-button-47cfaf");
        public ILocator ListOfDefectTypes => Page.GetByRole(AriaRole.Listbox);
    }
}
