using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace DispJournalTests.Map
{
    public class DispPage
    {
        public IPage Page { get; set; }

        public DispPage(IPage page)
        {
            Page = page;
        }

        public ILocator NewRecordButton => Page.GetByTestId("dx-button-1b254a");
    }
}
