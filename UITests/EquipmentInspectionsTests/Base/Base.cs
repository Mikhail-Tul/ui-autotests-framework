using EquipmentInspectionsTests.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentInspectionsTests.Base
{
    public class Base : PlayInit
    {
        protected EquipPage EquipPage => new(Page);
        protected NewStatementWin NewStatementWin => new(Page);
        protected string OrgUid = "";
        protected string PersonUid = "";
        protected string SubstationUid = "";
        protected string MeasurementValueUid = "";
        protected string ControlObjectUid = "";
        protected string StatementTypesUid = "";
        protected string UserRoleUid = "";
    }
}
