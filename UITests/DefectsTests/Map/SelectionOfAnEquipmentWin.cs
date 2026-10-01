using System;
using System.Collections.Generic;
using System.Text;

namespace DefectsTests.Map
{
    public class SelectionOfAnEquipmentWin : CreateWin
    {
        public SelectionOfAnEquipmentWin(IPage page) : base(page) { }

        public ILocator SelectButton => Page.GetByTestId("dx-button-0f8003");
    }
}
