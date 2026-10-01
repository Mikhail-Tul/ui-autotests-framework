using System;
using System.Collections.Generic;
using System.Text;

namespace DefectsTests.Map
{
    public class SelectionOfAResponsibleDepartmentWin : CreateWin
    {
        public SelectionOfAResponsibleDepartmentWin(IPage page) : base(page) { }

        public ILocator SelectButton => Page.GetByTestId("dx-button-eca5f3");
    }
}
