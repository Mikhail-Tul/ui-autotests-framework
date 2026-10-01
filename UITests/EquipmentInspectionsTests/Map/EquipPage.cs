using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentInspectionsTests.Map
{
    public class EquipPage
    {
        public IPage Page { get; }
        public EquipPage(IPage page)
        {
            Page = page;
        }

        public ILocator NewStatementControlButton => Page.GetByRole(AriaRole.Button, new() { Name = "Новые ведомости контроля" });
    }
}
