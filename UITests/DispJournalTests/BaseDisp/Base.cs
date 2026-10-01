using BaseProject.Base;
using DispJournalTests.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace DispJournalTests.BaseDisp
{
    public class Base : PlayInit
    {
        protected static List<string> CategoryTypes = new();
        protected static Dictionary<string, List<string>> Categorys = new();
        protected DispPage DispPage => new(Page);
        protected NewRecordWin NewRecordWin => new(Page);
        protected SelectionOfCategory SelectionOfCategory => new(Page);
        protected SelectionOfObject SelectionOfObject => new(Page);
        protected SelectionOfPerson SelectionOfPerson => new(Page);
        protected SaveRecordWin SaveRecordWin => new(Page);
        protected string DispGuidRecord = Guid.NewGuid().ToString();
    }
}
