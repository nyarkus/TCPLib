namespace TCPLib.Server.Net
{
    public class BanMessage
    {
        public TimeSpan period;
        public string reason;
        public BanMessage(TimeSpan period = default, string reason = "")
        {
            this.period = period;
            this.reason = reason;
        }
        public BanMessage(DateTime period = default, string reason = "")
        {
            this.period = period - DateTime.UtcNow;
            this.reason = reason;
        }
    }
}