using Orleans;

namespace MvvmCore.Grains;

public interface IChangeManagerGrain: IGrainWithGuidKey
{
  Task<byte[]> InvokeCommandAsync(string commandName);
  Task<byte[]> SetPropAsync(string propName, string value);
  Task<byte[]> StartViewModelAsync(Type vmType);
}
