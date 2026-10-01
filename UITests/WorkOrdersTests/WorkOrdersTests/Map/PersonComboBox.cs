using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Компонент выпадающего списка выбора персонала.
    /// </summary>
    public class PersonComboBox : WorkOrdersPage
    {
        /// <summary>
        /// Конструктор выпадающего списка персонала.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public PersonComboBox(IPage page) : base(page) { }

        /// <summary>
        /// Элемент списка с группой электробезопасности "V гр. ЭБ".
        /// </summary>
        public ILocator GREB => Page.GetByRole(AriaRole.Listbox).GetByText("V гр. ЭБ", new() { Exact = true });
    }
}
