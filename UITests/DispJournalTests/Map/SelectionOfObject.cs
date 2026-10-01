using System;
using System.Collections.Generic;
using System.Text;

namespace DispJournalTests.Map
{
    public class SelectionOfObject : NewRecordWin
    {
        public SelectionOfObject(IPage page) : base(page)
        {
            
        }

        public ILocator Switch => Page.GetByTestId("dx-switch-51ad68");
        public ILocator SelectButton => Page.GetByTestId("dx-button-24d2fa");
    }
}
