using System;
using System.Collections.Generic;
using System.Text;

namespace DispJournalTests.Map
{
    public class SelectionOfPerson : NewRecordWin
    {
        public SelectionOfPerson(IPage page) : base(page)
        {
            
        }

        public ILocator SelectButton => Page.GetByTestId("dx-button-cadbcb");
    }
}
