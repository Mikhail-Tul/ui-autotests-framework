using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Класс для работы со статусами заявки.
    /// </summary>
    public class Statuses : WorkOrdersPage
    {
        /// <summary>
        /// Конструктор класса статусов.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public Statuses(IPage page) : base(page) { }

        /// <summary>
        /// Локатор статуса "Создан".
        /// </summary>
        public ILocator CreatedStat => Page.Locator(".dxPopupTitleCustom").GetByText("Создан", new() { Exact = true });
    }
}
