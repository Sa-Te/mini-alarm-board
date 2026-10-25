using MiniAlarmBoard.Api.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MiniAlarmBoard.Api.Storage;

public interface IAlarmStore
{
    void Add(Alarm alarm);
    IReadOnlyList<Alarm> GetAll();
    IReadOnlyList<Alarm> GetByAreaSince(string area, DateTimeOffset since);
}
