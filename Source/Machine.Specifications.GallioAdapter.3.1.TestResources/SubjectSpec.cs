using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.GallioAdapter.TestResources
{
  [Subject( typeof(bool), "Testing out the framework")]
  public class subject_spec
  {
    It should_have_a_subject_as_the_category = () => 
      true.ShouldBeTrue();
  }
}
