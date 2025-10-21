namespace MvvmCore.Model;

public class ToUpperTextConverter
{
  public string ConvertText(string inputText)
  {
    Console.WriteLine($"ToUpperTextConverter: Converting text '{inputText}' to uppercase.");
    return inputText.ToUpperInvariant();
  }
}
