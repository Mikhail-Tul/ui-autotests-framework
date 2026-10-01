using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Класс, представляющий элемент календаря для выбора даты и времени.
    /// </summary>
    public class CalendarEl : WorkOrdersPage
    {
        /// <summary>
        /// Конструктор элемента календаря.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public CalendarEl(IPage page) : base(page) { }

        /// <summary>
        /// Локатор области дат календаря.
        /// </summary>
        public ILocator CalendarDates => Page.Locator(".dx-calendar").
            Filter(new() { Has = Page.GetByRole(AriaRole.Grid, new() { Name = "Календарь" }) });

        /// <summary>
        /// Локатор области выбора времени в календаре.
        /// </summary>
        public ILocator CalendarTimes => Page.Locator(".dx-popup-normal").
            Filter(new() { Has = Page.GetByRole(AriaRole.Grid, new() { Name = "Календарь" }) }).
            Locator(".dx-datebox-datetime-time-side");

        /// <summary>
        /// Локатор всплывающего окна календаря.
        /// </summary>
        public ILocator CalendarPopUp => Page.Locator(".dx-popup-normal").
            Filter(new() { Has = Page.GetByRole(AriaRole.Grid, new() { Name = "Календарь" }) });

        /// <summary>
        /// Кнопка подтверждения выбора (OK).
        /// </summary>
        public ILocator OKButton => Page.GetByRole(AriaRole.Button, new() { Name = "OK", Exact = true });

        /// <summary>
        /// Кнопка установки текущей даты (Сегодня).
        /// </summary>
        public ILocator TodayButton => Page.GetByRole(AriaRole.Button, new() { Name = "Сегодня" });

        /// <summary>
        /// Кнопка увеличения значения часов.
        /// </summary>
        public ILocator HoureUpButton => Page.Locator(".dx-popup-normal").
            Filter(new() { Has = Page.GetByRole(AriaRole.Grid, new() { Name = "Календарь" }) }).
            Locator(".dx-datebox-datetime-time-side").Locator(".dx-numberbox-spin-up").Nth(0);

        /// <summary>
        /// Кнопка увеличения значения минут.
        /// </summary>
        public ILocator MinuteUpButton => Page.Locator(".dx-popup-normal").
            Filter(new() { Has = Page.GetByRole(AriaRole.Grid, new() { Name = "Календарь" }) }).
            Locator(".dx-datebox-datetime-time-side").Locator(".dx-numberbox-spin-up").Nth(1);

        /// <summary>
        /// Локатор самой сетки календаря для выбора конкретной даты.
        /// </summary>
        public ILocator DateInCalendar => Page.GetByRole(AriaRole.Grid, new() { Name = "Календарь" });
    }
}
