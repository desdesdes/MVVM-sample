using Microsoft.AspNetCore.Mvc;
using MvvmCore.Utils;
using MvvmCore.ViewModel;

namespace MvvmWebserver.Controllers;

[ApiController]
[Route("[controller]")]
public class PresenterController : ControllerBase
{
  private static Dictionary<Guid, ChangeManager> _cms = new();

  [HttpPost("StartViewModel", Name = "StartViewModel")]
  public void StartViewModel(Guid id)
  {
    var cm = new ChangeManager(new PresenterViewModel());

    cm.WriteVmToJson(HttpContext.Response.BodyWriter.AsStream());

    _cms.Add(id, cm);
  }

  [HttpPost("SetProp", Name = "SetProp")]
  public void SetProp(Guid id, string propName, string value)
  {
    var cm = _cms[id];

    cm.SetProp(propName, value);

    cm.WriteVmChangesToJson(HttpContext.Response.BodyWriter.AsStream());
    cm.ClearChanges();
  }

  [HttpPost("InvokeCommand", Name = "InvokeCommand")]
  public void InvokeCommand(Guid id, string commandName)
  {
    var cm = _cms[id];

    cm.InvokeCommand(commandName);

    cm.WriteVmChangesToJson(HttpContext.Response.BodyWriter.AsStream());
    cm.ClearChanges();
  }
}
