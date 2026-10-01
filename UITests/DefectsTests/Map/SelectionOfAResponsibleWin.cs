using System;
using System.Collections.Generic;
using System.Text;

namespace DefectsTests.Map
{
    public class SelectionOfAResponsibleWin : CreateWin
    {
        public SelectionOfAResponsibleWin(IPage page) : base(page) { }

        public ILocator SelectButton => Page.GetByTestId("dx-button-eca5f3");
    }
}
