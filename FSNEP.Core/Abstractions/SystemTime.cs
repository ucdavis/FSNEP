using System;

namespace FSNEP.Core.Abstractions
{
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;
        
        public static void Reset()
        {
            Now = () => DateTime.Now;
        }
    }
}