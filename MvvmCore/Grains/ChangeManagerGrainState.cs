namespace MvvmCore.Grains;

public class ChangeManagerGrainState
{
  public Type? VmType { get; set; }
  public byte[]? VmJson { get; set; }
}
