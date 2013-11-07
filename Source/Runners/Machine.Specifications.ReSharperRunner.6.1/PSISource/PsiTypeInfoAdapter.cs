using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

using Machine.Specifications.Sdk;
using CLRTypeName = JetBrains.ReSharper.Psi.ClrTypeName;


namespace Machine.Specifications.ReSharperRunner
{
    public class PsiTypeInfoAdapter// : ITypeInfo
    {
        //readonly IClass psiType;

        //public PsiTypeInfoAdapter(IClass psiType)
        //{
        //    this.psiType = psiType;
        //}

        //public bool IsStruct
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        //public bool ExistsAnySpecifications()
        //{
        //    throw new NotImplementedException();
        //}

        ////public bool ExistsAnyBehaviors()
        ////{
        ////    return psiType.GetBehaviors().Any();
        ////}

        //public int GenericParametersCount { get; private set; }

        //public bool HasBehaviorAttributeName
        //{
        //    get
        //    {
        //        return psiType.HasAttributeInstance(new CLRTypeName(new BehaviorAttributeFullName()), false);
        //    }
        //}

        //public bool IsAbstract
        //{
        //    get { return psiType.IsAbstract; }
        //}

        //public bool IsSealed
        //{
        //    get { return psiType.IsSealed; }
        //}
    }
}
