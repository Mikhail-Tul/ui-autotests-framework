using BaseProject.Base;
using ShiftManagerTests.Map;

namespace ShiftManagerTests.BaseShiftManager
{
    public class Base : PlayInit
    {
        protected ShiftMPage ShiftMPage => new(Page);
        protected ForceShiftTransferWindow ShiftTransferWin => new(Page);
    }
}
