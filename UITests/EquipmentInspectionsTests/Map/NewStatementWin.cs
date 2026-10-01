using BaseProject.Base;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentInspectionsTests.Map
{
    public class NewStatementWin
    {
        public IPage Page { get; }
        public NewStatementWin(IPage page)
        {
            Page = page;
        }

        public ILocator StatementOption => Page.GetByRole(AriaRole.Option).GetByText(Config.TextPattern);
        public ILocator CreateStatement => Page.GetByRole(AriaRole.Button, new() { Name = "Создать ведомости" });
    }
}
