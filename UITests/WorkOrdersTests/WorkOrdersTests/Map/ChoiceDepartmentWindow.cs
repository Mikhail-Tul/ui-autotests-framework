using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Окно выбора подразделения.
    /// </summary>
    public class ChoiceDepartmentWindow : WorkOrdersPage
    {
        /// <summary>
        /// Конструктор окна выбора подразделения.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public ChoiceDepartmentWindow(IPage page) : base(page) { }

        /// <summary>
        /// Кнопка подтверждения выбора подразделения.
        /// </summary>
        public ILocator ChoiceButton => Page.Locator("dx-button").Filter(new() { HasText = "Выбрать" });
    }
}
