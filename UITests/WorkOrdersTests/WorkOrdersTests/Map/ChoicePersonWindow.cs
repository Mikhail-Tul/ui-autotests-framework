using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Окно выбора персонала.
    /// </summary>
    public class ChoicePersonWindow : WorkOrdersPage
    {
        /// <summary>
        /// Конструктор окна выбора персонала.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public ChoicePersonWindow(IPage page) : base(page) { }

        /// <summary>
        /// Кнопка подтверждения выбора сотрудника.
        /// </summary>
        public ILocator ChoiceButton => Page.GetByTestId("dx-button-a16a74");
    }
}
