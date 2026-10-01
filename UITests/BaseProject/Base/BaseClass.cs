using BaseProject.AllureHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseProject.Base
{
    public class BaseClass : AllureBaseTest
    {
        protected List<string> PersonTree { get; set; } = new();
        protected List<string> SubstationTree { get; set; } = new();
        protected string SubstationName { get; set; } = "";
        protected List<string> ShiftUids { get; set; } = new();
        protected string EquipmentName { get; set; } = "";
    }
}
