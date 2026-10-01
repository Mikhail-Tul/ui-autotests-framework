using System;
using System.Collections.Generic;
using System.Text;

namespace DispJournalTests.Extensions
{
    public static class DispPageExtensions
    {
        public static ILocator FilterForDispRecords(this IPage page, string text)
        {
            return page.GetByRole(AriaRole.Row)
                .Filter(new() { HasText = text });
        }
    }
}
