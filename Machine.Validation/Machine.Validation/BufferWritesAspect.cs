using System;
using System.Collections.Generic;
using System.Reflection;
using PostSharp.Laos;

namespace Machine.Validation
{
  [Serializable]
  public class BufferWritesAspect : OnFieldAccessAspect
  {
    private readonly Dictionary<FieldInfo, object> _values = new Dictionary<FieldInfo, object>();

    public override void OnGetValue(FieldAccessEventArgs eventArgs)
    {
      Console.WriteLine("Get {0} {1} exposed={2} stored={3}", eventArgs.DeclaringType, eventArgs.FieldInfo, eventArgs.ExposedFieldValue, eventArgs.StoredFieldValue);
      if (!_values.ContainsKey(eventArgs.FieldInfo))
      {
        _values[eventArgs.FieldInfo] = eventArgs.StoredFieldValue;
      }
      eventArgs.ExposedFieldValue = _values[eventArgs.FieldInfo];
    }

    public override void OnSetValue(FieldAccessEventArgs eventArgs)
    {
      Console.WriteLine("Set {0} {1} exposed={2} stored={3}", eventArgs.DeclaringType, eventArgs.FieldInfo, eventArgs.ExposedFieldValue, eventArgs.StoredFieldValue);
      _values[eventArgs.FieldInfo] = eventArgs.ExposedFieldValue;
    }
  }
}
