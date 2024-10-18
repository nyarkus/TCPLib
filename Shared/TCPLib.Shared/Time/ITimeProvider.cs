using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPLib.Time
{
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }
    public class UtcTime : ITimeProvider
    {
        public DateTimeOffset Now { get; } = DateTimeOffset.UtcNow;
    }
    public static class TimeProvider
    {
        public static ITimeProvider timeProvider { get; } = new UtcTime();
        public static DateTimeOffset Now => timeProvider.Now;
    }
#if DEBUG
    public class TestableTime : ITimeProvider
    {
        public DateTimeOffset Now { get; } = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
    }
#endif
}
