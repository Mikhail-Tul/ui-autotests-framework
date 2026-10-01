using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Окно выбора объектов.
    /// </summary>
    public class ChoiceObjectWindow : WorkOrdersPage
    {
        /// <summary>
        /// Конструктор окна выбора объектов.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public ChoiceObjectWindow(IPage page) : base(page) { }

        /// <summary>
        /// Переключатель режима отображения полного дерева объектов.
        /// </summary>
        public ILocator FullTreeSwitch => Page.GetByTestId("dx-switch-25895f");

        /// <summary>
        /// Кнопка сохранения выбранных объектов.
        /// </summary>
        public ILocator SaveButton => Page.GetByTestId("dx-button-dc6059");
    }
}
