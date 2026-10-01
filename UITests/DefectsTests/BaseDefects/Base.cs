using BaseProject.Base;
using DefectsTests.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace DefectsTests.BaseDefects
{
    public class Base : PlayInit
    {
        protected DefectsPage DefectsPage => new(Page);
        protected CreateWin CreateWin => new(Page);
        protected List<string> ObjectTree = new();
        protected SelectionOfAnObjectWin ObjectWin => new(Page);
        protected SelectionOfAnEquipmentWin EquipWin => new(Page);
        protected SelectionOfAResponsibleDepartmentWin DepWin => new(Page);
        protected SelectionOfAResponsibleWin ResWin => new(Page);
    }
}
