using System.Text.Json.Nodes;
using MvvmCore.Utils;
using Orleans.Placement;

namespace MvvmCore.Grains;

[CollectionAgeLimit("0:0:10")]
public class ChangeManagerGrain : Grain<ChangeManagerGrainState>, IChangeManagerGrain
{
  ChangeManager? _cm;

  public async Task<byte[]> StartViewModelAsync(Type vmType)
  {
    var vm = Activator.CreateInstance(vmType) as ObservableObject ??
      throw new Exception("Could not create ViewModel of type " + vmType.FullName);

    _cm = new ChangeManager(vm);
    State.VmType = vmType;

    return await WriteFullStateAsync();
  }

  private async Task<byte[]> WriteFullStateAsync()
  {
    using var stream = new MemoryStream();
    _cm!.WriteVmToJson(stream);
    var data = stream.ToArray();

    State.VmJson = data;
    await WriteStateAsync();

    return data;
  }

  private void RestoreState()
  {
    if(_cm == null)
    {
      if(State.VmType == null)
      {
        throw new Exception("ChangeManager not initialized. Call StartViewModel first.");
      }

      var json = (JsonObject)JsonObject.Parse(State.VmJson)!;
      var vm = Activator.CreateInstance(State.VmType, new object[] { json }) as ObservableObject ??
        throw new Exception("Could not create ViewModel of type " + State.VmType.FullName);

      _cm = new ChangeManager(vm);
    }
  }

  public async Task<byte[]> SetPropAsync(string propName, string value)
  {
    RestoreState();

    _cm!.SetProp(propName, value);

    await WriteFullStateAsync();

    using var stream = new MemoryStream();
    _cm.WriteVmChangesToJson(stream);
    _cm.ClearChanges();

    return stream.ToArray();
  }

  public async Task<byte[]> InvokeCommandAsync(string commandName)
  {
    RestoreState();

    _cm.InvokeCommand(commandName);
    await WriteFullStateAsync();

    using var stream = new MemoryStream();
    _cm.WriteVmChangesToJson(stream);
    _cm.ClearChanges();

    return stream.ToArray();
  }
}
