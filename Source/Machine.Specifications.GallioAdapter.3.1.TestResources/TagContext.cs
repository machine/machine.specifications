using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.GallioAdapter.TestResources
{
  [Tags("tag")]
  public class tag_spec
  {
    It should = () => 
      true.ShouldBeTrue();
  }

  [Tags("one","two", "three")]
  public class multiple_tag_spec
  {
    It should = () => 
      true.ShouldBeTrue();
  }
}
