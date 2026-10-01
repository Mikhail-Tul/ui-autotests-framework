using System;
using System.Collections.Generic;
using System.Text;

namespace DispJournalTests.Map
{
    public class SelectionOfCategory : NewRecordWin
    {
        public SelectionOfCategory(IPage page) : base(page)
        {
            
        }

        public ILocator SelectButton => Page.GetByTestId("dx-button-d78603");
    }
}
