using System.Collections.Generic;

namespace Machine.Specifications.Reporting.Model
{
  public class Metadata
  {
    readonly IDictionary<string, object> _values = new Dictionary<string, object>();

    public object this[string key]
    {
      get
      {
        object value;
        if (_values.TryGetValue(key, out value))
        {
          return value;
        }

        return null;
      }
      set
      {
        if (_values.ContainsKey(key))
        {
          _values[key] = value;
          return;
        }

        _values.Add(key, value);
      }
    }
  }
}