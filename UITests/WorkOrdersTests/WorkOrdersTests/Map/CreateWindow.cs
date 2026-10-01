using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Окно создания новой заявки.
    /// Содержит все основные поля и кнопки для заполнения параметров работ.
    /// </summary>
    public class CreateWindow : WorkOrdersPage
    {
        /// <summary>
        /// Конструктор окна создания заявки.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public CreateWindow(IPage page) : base(page) { }

        /// <summary>
        /// Кнопка выбора подразделения из справочника.
        /// </summary>
        public ILocator ChoiceDepartmentButton => Page.GetByPlaceholder("Подразделение").GetByTitle("Выбор из справочника");

        /// <summary>
        /// Поле ввода подразделения.
        /// </summary>
        public ILocator ChoiceDepartmentTextInput => Page.GetByPlaceholder("Подразделение");

        /// <summary>
        /// Кнопка очистки поля подразделения.
        /// </summary>
        public ILocator ClearDepartmentButton => Page.GetByPlaceholder("Подразделение").GetByTitle("Очистить");

        /// <summary>
        /// Переключатель огневых работ.
        /// </summary>
        public ILocator SwitchFire => Page.GetByRole(AriaRole.Button, new() { Name = "ВЫКЛ" });

        /// <summary>
        /// Выпадающий список категорий работ.
        /// </summary>
        public ILocator WorkTypeCombobox => Page.GetByTitle("Категория работ");

        /// <summary>
        /// Кнопка добавления объектов из справочника.
        /// </summary>
        public ILocator AddObjectButton => Page.GetByTitle("Добавить объекты из справочника");

        /// <summary>
        /// Заголовок раздела утверждающих и согласующих огневых работ.
        /// </summary>
        public ILocator TitleSwitchFire => Page.GetByText("Утверждающий и согласующие огневые работы:");

        /// <summary>
        /// Альтернативная кнопка добавления объектов из справочника (малая кнопка).
        /// </summary>
        public ILocator AddObjecttButton => Page.Locator(".dx-button-smalls").GetByTitle("Добавить объекты из справочника");

        /// <summary>
        /// Заголовок или область выбора мест работ.
        /// </summary>
        public ILocator AddPlaceWork => Page.GetByTitle("Места работ");

        /// <summary>
        /// Кнопка добавления мест работ.
        /// </summary>
        public ILocator AddPlaceWorkButton => Page.GetByTitle("Места работ").GetByRole(AriaRole.Button);

        /// <summary>
        /// Поле ввода содержания работ.
        /// </summary>
        public ILocator TheContentOfTheWorksTextInput => Page.GetByTitle("Содержание работ");

        /// <summary>
        /// Кнопка вставки выбранных объектов в содержание работ.
        /// </summary>
        public ILocator AddObjectInContentOfTheWorkButton => Page.GetByTitle("Вставить объекты");

        /// <summary>
        /// Кнопка вставки выбранных мест работ в содержание работ.
        /// </summary>
        public ILocator AddWorkPlaceInContentOfTheWorkButton => Page.GetByTitle("Вставить места работ");

        /// <summary>
        /// Область-плейсхолдер для времени начала работ.
        /// </summary>
        public ILocator TimeStartWorksPlaceholder => Page.Locator(".placeholder-block").
            Filter(new() { Has = Page.GetByText("Время начала работ") });

        /// <summary>
        /// Кнопка вызова календаря для установки времени начала работ.
        /// </summary>
        public ILocator TimeStartWorksCalendarButton => Page.Locator(".placeholder-block").
            Filter(new() { Has = Page.GetByText("Время начала работ") }).GetByRole(AriaRole.Button).Nth(1);

        /// <summary>
        /// Область-плейсхолдер для времени завершения работ.
        /// </summary>
        public ILocator TimeEndWorksPlaceholder => Page.Locator(".placeholder-block").
            Filter(new() { Has = Page.GetByText("Время завершения работ") });

        /// <summary>
        /// Кнопка вызова календаря для установки времени завершения работ.
        /// </summary>
        public ILocator TimeEndWorksCalendarButton => Page.Locator(".placeholder-block").
            Filter(new() { Has = Page.GetByText("Время завершения работ") }).GetByRole(AriaRole.Button).Nth(1);

        /// <summary>
        /// Поле ввода названия электроустановок.
        /// </summary>
        public ILocator NameOfElectricalInstallationsTextBox => Page.GetByTestId("dx-text-box-46232a");

        /// <summary>
        /// Поле ввода отдельных указаний.
        /// </summary>
        public ILocator SeparateInstructionsTextBox => Page.GetByPlaceholder("Отдельные указания");

        /// <summary>
        /// Выпадающий список выбора журнала учета.
        /// </summary>
        public ILocator AccountingLogDropDown => Page.GetByPlaceholder("Журнал учёта");

        /// <summary>
        /// Кнопка редактирования комментария.
        /// </summary>
        public ILocator CommentButton => Page.Locator(".accordion-header").
            Filter(new() { Has = Page.GetByText("Комментарий:") }).GetByLabel("fa fa-pen");

        /// <summary>
        /// Кнопка вызова меню действий.
        /// </summary>
        public ILocator DoButton => Page.GetByRole(AriaRole.Button, new() { Name = "Действия" });

        /// <summary>
        /// Кнопка выдачи заявки.
        /// </summary>
        public ILocator IssueButton => Page.GetByText("Выдать", new() { Exact = true });

        /// <summary>
        /// Поле ввода мер по подготовке.
        /// </summary>
        public ILocator MeasuresForPreparationTextBox => Page.Locator("app-preparations-mec textarea");

        /// <summary>
        /// Поле ввода должности.
        /// </summary>
        public ILocator PostTextBox => Page.GetByTitle("Должность");
    }
}
