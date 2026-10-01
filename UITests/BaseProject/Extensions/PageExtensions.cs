using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.Playwright.Assertions;

namespace BaseProject.Extensions
{
    /// <summary>
    /// Расширения для работы со страницей Playwright.
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Прокручивает контейнер до тех пор, пока целевой элемент не станет видимым в области просмотра.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="target">Целевой элемент для поиска.</param>
        /// <param name="scrollContainer">Контейнер, внутри которого выполняется прокрутка.</param>
        public static async Task ScrollWhileUnvisibleAsync(this IPage page, ILocator target, ILocator scrollContainer, int? maxScroll = null)
        {
            int limit = maxScroll ?? 20;
            for (int i = 0; i < limit; i++)
            {
                try
                {
                    await Expect(target).ToBeInViewportAsync(new() { Timeout = 1000, Ratio = 1 });
                    return;
                }
                catch (Exception)
                {
                    await scrollContainer.EvaluateAsync("el => el.scrollBy(0, window.innerHeight / 3)");
                    if (maxScroll == null) await page.WaitForTimeoutAsync(1000);
                }
            }

            throw new InvalidOperationException(
                $"Не найден после прокруток ({limit} попыток): целевой элемент не стал видимым в контейнере прокрутки");
        }

        /// <summary>
        /// Создает локатор для кнопки сворачивания/разворачивания элемента в дереве.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="nameOrg">Название организации/объекта.</param>
        /// <returns>Локатор кнопки.</returns>
        public static ILocator BuildCollapsedButton(this IPage page, string nameOrg)
        {
            return page.GetByRole(AriaRole.Gridcell, new() { Name = nameOrg })
                .Locator(".dx-treelist-collapsed");
        }

        /// <summary>
        /// Выбирает объект в иерархическом дереве, разворачивая необходимые узлы.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="substationTree">Список элементов пути к объекту.</param>
        public static async Task SelectObjectAsync(this IPage page, List<string> substationTree)
        {
            foreach (var substation in substationTree)
            {
                if (substation == substationTree.Last())
                {
                    await Expect(page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new($@".\s{substation}\s.", RegexOptions.IgnoreCase) })
                        .GetByRole(AriaRole.Checkbox))
                        .ToBeVisibleAsync(new() { Timeout = 10000 });
                    await page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new($@".\s{substation}\s.", RegexOptions.IgnoreCase) })
                        .ClickAsync();
                    await page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new($@".\s{substation}\s.", RegexOptions.IgnoreCase) })
                        .GetByRole(AriaRole.Checkbox)
                        .ClickAsync();
                }
                else
                {
                    await page.ScrollWhileUnvisibleAsync(
                        page.BuildCollapsedButton(substation),
                        page.Locator(".dx-treelist-rowsview")
                        .Locator(".dx-scrollable-container"));
                    await page.BuildCollapsedButton(substation).ClickAsync();
                }
            }
        }
    }
}
