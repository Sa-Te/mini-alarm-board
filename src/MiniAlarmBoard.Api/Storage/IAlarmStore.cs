using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Storage;

public interface IAlarmStore
{
    void Add(Alarm alarm);
    IReadOnlyList<Alarm> GetAll();
}
