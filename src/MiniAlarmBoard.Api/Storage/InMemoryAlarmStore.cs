using MiniAlarmBoard.Api.Models;
namespace MiniAlarmBoard.Api.Storage;

//not thread safe yet
public class InMemoryAlarmStore : IAlarmStore
{
    private readonly List<Alarm> _alarms = new();
    private readonly object _gate = new();

    public void Add(Alarm alarm)
    {
        if (alarm is null) throw new ArgumentNullException(nameof(alarm));
        lock (_gate)
        {
            _alarms.Add(alarm);
        }
    }

    public IReadOnlyList<Alarm> GetAll()
    {
        lock (_gate)
        {
            return _alarms.ToList();
        }
    }

    public IReadOnlyList<Alarm> GetByAreaSince(string area, DateTimeOffset since)
    {
        lock (_gate)
        {
            return _alarms.Where(a => string.Equals(a.Area, area) && a.Timestamp >= since).ToList();
        }
    }
}
