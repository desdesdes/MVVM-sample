using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;

namespace MvvmCore.Utils;

public class ChangeManager
{
  HashSet<string> _changedProperties = new();
  ObservableObject _viewModel;

  public ChangeManager(ObservableObject viewModel)
  {
    _viewModel = viewModel;
    _viewModel.PropertyChanged += RecordPropertyChanged;
  }

  private void RecordPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    _changedProperties.Add(e.PropertyName!);
  }

  public void WriteVmToJson(Stream destination)
  {
    using var writer = new Utf8JsonWriter(destination);
    writer.WriteStartObject();

    var vmType = _viewModel.GetType();
    foreach(var prop in vmType.GetProperties())
    {
      if(prop.PropertyType == typeof(ICommand))
      {
        // ignore, this are commands
        continue;
      }
      WritePropertyToJson(_viewModel, writer, prop);
    }

    writer.WriteEndObject();
  }


  public void WriteVmChangesToJson(Stream destination)
  {
    using var writer = new Utf8JsonWriter(destination);
    writer.WriteStartObject();

    var vmType = _viewModel.GetType();
    foreach(var prop in vmType.GetProperties())
    {
      if(prop.PropertyType == typeof(ICommand))
      {
        // ignore, this are commands
        continue;
      }

      if(!_changedProperties.Contains(prop.Name))
      {
        continue;
      }

      WritePropertyToJson(_viewModel, writer, prop);
    }

    writer.WriteEndObject();
  }

  public void ClearChanges()
  {
    _changedProperties.Clear();
  }


  private void WritePropertyToJson(object vm, Utf8JsonWriter writer, System.Reflection.PropertyInfo prop)
  {
    writer.WritePropertyName(prop.Name);

    if(prop.PropertyType == typeof(string))
    {
      writer.WriteStringValue((string?)prop.GetValue(vm));
    }
    else if(prop.PropertyType == typeof(int))
    {
      writer.WriteNumberValue((int)prop.GetValue(vm)!);
    }
    else if(prop.PropertyType == typeof(bool))
    {
      writer.WriteBooleanValue((bool)prop.GetValue(vm)!);
    }
    else if(prop.PropertyType.IsAssignableTo(typeof(IEnumerable<string>)))
    {
      var items = (IEnumerable<string>)prop.GetValue(vm)!;

      // Make sure then wheen the collection changes we also write the collection
      if(items is INotifyCollectionChanged notifyCollectionChanged)
      {
        notifyCollectionChanged.CollectionChanged += (_, _) =>
        {
          _changedProperties.Add(prop.Name);
        };
      }

      writer.WriteStartArray();

      foreach(var item in items)
      {
        writer.WriteStringValue(item);
      }

      writer.WriteEndArray();
    }
    else
    {
      throw new Exception($"Unsupported property type '{prop.PropertyType}'.");
    }
  }

  public void SetProp(string propName, string value)
  {
    var vmType = _viewModel.GetType();

    var prop = vmType.GetProperty(propName) ?? throw new Exception($"Missing property '{propName}'.");
    prop.SetValue(_viewModel, Convert.ChangeType(value, prop.PropertyType));
  }

  public void InvokeCommand(string commandName)
  {
    var vmType = _viewModel.GetType();

    var prop = vmType.GetProperty(commandName) ?? throw new Exception($"Missing command '{commandName}'.");
    var command = prop.GetValue(_viewModel) as ICommand ?? throw new Exception($"Property '{commandName}' is not a command.");
    command.Execute(null);
  }
}
