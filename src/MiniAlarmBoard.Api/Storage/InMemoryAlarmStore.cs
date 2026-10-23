using MiniAlarmBoard.Api.Models;
namespace MiniAlarmBoard.Api.Storage;

//not thread safe yet
public class InMemoryAlarmStore : IAlarmStore
{
    private readonly List<Alarm> _alarms = new();

    public void Add(Alarm alarm)
    {
        if (alarm is null) throw new ArgumentNullException(nameof(alarm));
        _alarms.Add(alarm);
    }

    public IReadOnlyList<Alarm> GetAll()
    {
        return _alarms.ToList();
    }
}
