using BaseProject.Base;
using ShiftHandoverTests.Maps;

namespace ShiftHandoverTests.BaseShiftManager
{
    public class Base : PlayInit
    {
        protected ShiftHandPage ShiftHandPage => new(Page);
    }
}
