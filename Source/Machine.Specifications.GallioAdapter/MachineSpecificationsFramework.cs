// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Modified by and Portions Copyright 2008 Machine Project

using System.Collections.Generic;
using System.Linq;
using Gallio.Model;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Machine.Specifications.GallioAdapter
{
  public class MachineSpecificationsFramework : BaseTestFramework
  {  
    public override TestDriverFactory GetTestDriverFactory()
    {
      return CreateDriver;
    }

    static ITestDriver CreateDriver(
      IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> testFrameworkHandles,
      TestFrameworkOptions testFrameworkOptions,
      ILogger logger)
    {
      return new Services.MachineSpecificationsDriver();
    }
  }
}
