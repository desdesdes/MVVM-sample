using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmCore.Model;
using MvvmCore.Utils;

namespace MvvmCore.ViewModel;

public class PresenterViewModel : ObservableObject
{
  private readonly ToUpperTextConverter _textConverter = new();
  private string? _text;
  private readonly ObservableCollection<string> _history = new ObservableCollection<string>();

  public string? Text
  {
    get { return _text; }
    set
    {
      _text = value;
      RaisePropertyChangedEvent("Text");
    }
  }

  public ObservableCollection<string> History
  {
    get { return _history; }
  }

  public ICommand ConvertTextCommand
  {
    get { return new DelegateCommand(ConvertText); }
  }

  private void ConvertText()
  {
    if(string.IsNullOrWhiteSpace(Text))
    {
      return;
    }

    AddToHistory(_textConverter.ConvertText(Text));
    Text = string.Empty;
  }

  private void AddToHistory(string item)
  {
    if(!_history.Contains(item))
    {
      _history.Add(item);
    }
  }
}
