﻿// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

using Gallio.Model.Helpers;

namespace Machine.Specifications.GallioAdapter.Services
{
  internal class MachineSpecificationsDriver : SimpleTestDriver
  {
    protected override string FrameworkName
    {
      get { return "Machine Specifications"; }
    }

    protected override TestExplorer CreateTestExplorer()
    {
      return new MachineSpecificationsExplorer();
    }

    protected override TestController CreateTestController()
    {
      return new DelegatingTestController( test => new Services.MachineSpecificationController() );
    }    
  }
}
