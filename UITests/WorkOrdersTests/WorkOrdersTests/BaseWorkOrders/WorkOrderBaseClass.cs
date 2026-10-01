using BaseProject.Base;
using BaseProject.Extensions;
using WorkOrdersTests.Extensions;
using WorkOrdersTests.Map;
using static Microsoft.Playwright.Assertions;

namespace WorkOrdersTests.BaseWorkOrders
{
    /// <summary>
    /// Базовый класс для тестов заявок на выполнение работ.
    /// Содержит общие Page Object модели и вспомогательные методы для подготовки данных и навигации.
    /// </summary>
    public class WorkOrderBaseClass : PlayInit
    {
        protected WorkOrdersPage WorkOrdersPage => new(Page);
        protected CreateWindow CreateWin => new(Page);
        protected ChoiceDepartmentWindow ChoiceDepartmentWindow => new(Page);
        protected CommentWindow CommentWindow => new(Page);
        protected Statuses Statuses => new(Page);
        public string NameAccountingLog { get; set; } = "";
    }
}
