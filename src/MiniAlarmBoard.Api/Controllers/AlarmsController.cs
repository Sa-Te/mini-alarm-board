using Microsoft.AspNetCore.Mvc;
using MiniAlarmBoard.Api.Models;
using MiniAlarmBoard.Api.Storage;

namespace MiniAlarmBoard.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AlarmsController : ControllerBase
{
    private readonly IAlarmStore _store;

    public AlarmsController(IAlarmStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IReadOnlyList<Alarm> Get()
    {
        return _store.GetAll();
    }
}