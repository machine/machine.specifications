using System;
using System.Collections.Generic;
using System.Reflection;

using PostSharp.Extensibility;
using PostSharp.Laos;

namespace Machine.Validation
{
  [Serializable, AttributeUsage(AttributeTargets.Class), MulticastAttributeUsage(MulticastTargets.Class)]
  public class BufferWritesAttribute : CompoundAspect
  {
    public override void ProvideAspects(object element, LaosReflectionAspectCollection collection)
    {
      Type targettype = (Type)element;
      BufferWritesAspect bufferWritesAspect = new BufferWritesAspect();
      foreach (FieldInfo fi in targettype.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
      {
        if (!fi.IsStatic)
        {
          collection.AddAspect(fi, bufferWritesAspect);
        }
      }

    }
  }
}
