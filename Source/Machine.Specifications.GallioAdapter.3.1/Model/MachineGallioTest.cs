using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Tree;
using Machine.Specifications.GallioAdapter.Services;
using Gallio.Model.Helpers;

namespace Machine.Specifications.GallioAdapter.Model
{
    public abstract class MachineGallioTest : Test
    {
        protected MachineGallioTest(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
        }
    }
}
