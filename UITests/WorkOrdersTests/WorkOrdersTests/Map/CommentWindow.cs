using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrdersTests.Map
{
    /// <summary>
    /// Окно ввода комментария.
    /// </summary>
    public class CommentWindow : CreateWindow
    {
        /// <summary>
        /// Конструктор окна комментария.
        /// </summary>
        /// <param name="page">Экземпляр страницы Playwright.</param>
        public CommentWindow(IPage page) : base(page) { }

        /// <summary>
        /// Текстовое поле для ввода комментария.
        /// </summary>
        public ILocator TextBox => Page.GetByTestId("dx-text-box-47b2a4");

        /// <summary>
        /// Кнопка сохранения комментария.
        /// </summary>
        public ILocator SaveButton => Page.GetByTestId("dx-button-be4429");
    }
}
