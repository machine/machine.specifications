using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.GallioAdapter.TestResources
{
  [Ignore]
  public class ignored_context_spec
  {
    public static bool established;
    public static bool because;
    public static bool spec;

    Establish context = () =>
      established = true;

    Because action = () =>
      because = true;

    It should = () =>
      spec = true;
  }

  public class ignored_specification_spec
  {
    public static bool established;
    public static bool because;
    public static bool spec;

    Establish context = () =>
      established = true;

    Because action = () =>
      because = true;

    [Ignore]
    It should = () =>
      spec = true;
  }
}
