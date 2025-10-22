using MvvmCore.Utils;
using Orleans.Placement;

namespace MvvmCore.Grains;

[Immovable] // makes sure orleans does not move this grain to another silo
[CollectionAgeLimit(Hours = 8)] // if grain is used for 8 hours, it can be removed
public class ChangeManagerGrain : Grain, IChangeManagerGrain
{
  ChangeManager? _cm;

  public async Task<byte[]> StartViewModelAsync(Type vmType)
  {
    var vm = Activator.CreateInstance(vmType) as ObservableObject ??
      throw new Exception("Could not create ViewModel of type " + vmType.FullName);

    _cm = new ChangeManager(vm);

    using var stream = new MemoryStream();
    _cm.WriteVmToJson(stream);

    return stream.ToArray();
  }

  public async Task<byte[]> SetPropAsync(string propName, string value)
  {
    if(_cm == null)
    {
      throw new Exception("ChangeManager not initialized. Call StartViewModel first.");
    }

    _cm.SetProp(propName, value);

    using var stream = new MemoryStream();
    _cm.WriteVmChangesToJson(stream);
    _cm.ClearChanges();

    return stream.ToArray();
  }

  public async Task<byte[]> InvokeCommandAsync(string commandName)
  {
    if(_cm == null)
    {
      throw new Exception("ChangeManager not initialized. Call StartViewModel first.");
    }

    _cm.InvokeCommand(commandName);

    using var stream = new MemoryStream();
    _cm.WriteVmChangesToJson(stream);
    _cm.ClearChanges();

    return stream.ToArray();
  }
}
