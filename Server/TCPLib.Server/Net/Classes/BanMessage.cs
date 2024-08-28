namespace TCPLib.Server.Net;
public class BanMessage
{
    public TimeSpan? period;
    public string? reason;
    public BanMessage(TimeSpan? period = null, string? reason = null)
    {
        this.period = period;
        this.reason = reason;
    }
    public BanMessage(DateTime? period = null, string? reason = null)
    {
        this.period = period - DateTime.UtcNow;
        this.reason = reason;
    }
}