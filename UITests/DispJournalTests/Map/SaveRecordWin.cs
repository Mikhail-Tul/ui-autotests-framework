using System;
using System.Collections.Generic;
using System.Text;

namespace DispJournalTests.Map
{
    public class SaveRecordWin : NewRecordWin
    {
        public SaveRecordWin(IPage page) : base(page)
        {
            
        }

        public ILocator AddRecordButton => Page.GetByTestId("dx-button-b9f57d");
    }
}
