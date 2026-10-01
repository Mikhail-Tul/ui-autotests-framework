using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Базовый класс для всех страниц приложения.
    /// Содержит общие элементы управления и ссылку на страницу браузера.
    /// </summary>
    public class WorkOrdersPage
    {
        protected readonly IPage Page;

        /// <summary>
        /// Конструктор базового класса страницы.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public WorkOrdersPage(IPage page)
        {
            Page = page;
        }

        /// <summary>
        /// Кнопка создания заявки (EU).
        /// </summary>
        public ILocator EUButton => Page.GetByTestId("dx-button-1ffd67");

        /// <summary>
        /// Индикатор текущей страницы в списке.
        /// </summary>
        public ILocator CountPageIndicator => Page.GetByRole(AriaRole.Button, new() { Name = "Page 1" });

        /// <summary>
        /// Кнопка "Нет".
        /// </summary>
        public ILocator NoButton => Page.GetByTestId("dx-button-d95a6a");

        /// <summary>
        /// Кнопка "Заказ".
        /// </summary>
        public ILocator OrderButton => Page.GetByTestId("dx-button-ef66f1");
    }
}
