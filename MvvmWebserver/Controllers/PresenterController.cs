using Microsoft.AspNetCore.Mvc;
using MvvmCore.Grains;
using MvvmCore.Utils;
using MvvmCore.ViewModel;

namespace MvvmWebserver.Controllers;

[ApiController]
[Route("[controller]")]
public class PresenterController(IGrainFactory GrainFactory) : ControllerBase
{
  [HttpPost("Demo", Name = "Demo")]
  public async Task<int> Demo()
  {
    var grain = GrainFactory.GetGrain<IDemoGrain>(Guid.Empty);
    return await grain.AddAsync(3, 4);
  }

  [HttpPost("StartViewModel", Name = "StartViewModel")]
  public async Task StartViewModel(Guid id)
  {
    var grain = GrainFactory.GetGrain<IChangeManagerGrain>(id);
    var result = await grain.StartViewModelAsync(typeof(PresenterViewModel));
    await HttpContext.Response.BodyWriter.WriteAsync(result);
  }

  [HttpPost("SetProp", Name = "SetProp")]
  public async Task SetProp(Guid id, string propName, string value)
  {
    var grain = GrainFactory.GetGrain<IChangeManagerGrain>(id);
    var result = await grain.SetPropAsync(propName, value);
    await HttpContext.Response.BodyWriter.WriteAsync(result);
  }

  [HttpPost("InvokeCommand", Name = "InvokeCommand")]
  public async Task InvokeCommand(Guid id, string commandName)
  {
    var grain = GrainFactory.GetGrain<IChangeManagerGrain>(id);
    var result = await grain.InvokeCommandAsync(commandName);

    await HttpContext.Response.BodyWriter.WriteAsync(result);
  }
}
