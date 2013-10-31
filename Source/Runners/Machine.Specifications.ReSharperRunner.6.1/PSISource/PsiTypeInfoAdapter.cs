using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

using Machine.Specifications.Sdk;

namespace Machine.Specifications.ReSharperRunner
{
    public class PsiTypeInfoAdapter : ITypeInfo
    {
        readonly IClass psiType;

        public PsiTypeInfoAdapter(IClass psiType)
        {
            this.psiType = psiType;
        }


        public bool ExistsAnySpecifications()
        {

            throw new NotImplementedException();
        }

        public bool ExistsAnyBehaviors()
        {
            throw new NotImplementedException();
        }

        public string GetStringOfSubjectAttribute()
        {
            throw new NotImplementedException();
        }

        public bool IsStruct
        {
            get
            {
                return false;
            }
        }

        public int GenericParametersCount { get; private set; }
        
        public bool HasBehaviorAttributeName { get; private set; }

        public bool IsAbstract
        {
            get { return psiType.IsAbstract; }
        }

        public bool IsSealed
        {
            get { return psiType.IsSealed; }
        }
    }
}
