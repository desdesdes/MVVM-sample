using Orleans;

namespace MvvmCore.Grains;

public interface IDemoGrain: IGrainWithGuidKey
{
  Task<int> AddAsync(int left, int right);
}

public class DemoGrain : Grain, IDemoGrain
{
  public Task<int> AddAsync(int left, int right)
  {
    return Task.FromResult(left + right);
  }
}
