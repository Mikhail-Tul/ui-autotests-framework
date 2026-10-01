using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DispJournalTests.Map
{
    public class NewRecordWin : DispPage
    {
        public NewRecordWin(IPage page) : base(page)
        {
            
        }

        public ILocator CategoryBookButton => Page.GetByTestId("dx-button-20f76d");
        public ILocator ObjectBookButton => Page.GetByTestId("dx-button-3df38d");
        public ILocator CommandMessageCombobox => Page.GetByTestId("dx-select-box-238765");
        public ILocator DirectionMessage => Page.GetByText(new Regex(@$"^передано$"));
        public ILocator CommandMessageBookButton => Page.GetByTestId("dx-button-6e64ab");
        public ILocator EditorContenTextBox => Page.GetByRole(AriaRole.Textbox, new() { Name = "Editor content" });
        public ILocator AddButton => Page.GetByTestId("dx-button-cf1833");
    }
}
