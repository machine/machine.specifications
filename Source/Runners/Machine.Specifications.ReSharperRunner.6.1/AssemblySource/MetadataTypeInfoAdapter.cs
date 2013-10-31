using System;
using System.Linq;
using JetBrains.Metadata.Reader.API;

using Machine.Specifications.Sdk;


namespace Machine.Specifications.ReSharperRunner
{
    // Provides an implementation of ITypeInfo when exploring a physical assembly's metadata
    //public class MetadataTypeInfoAdapter : ITypeInfo
    //{
    //    readonly IMetadataTypeInfo metadataTypeInfo;

    //    public MetadataTypeInfoAdapter(IMetadataTypeInfo metadataTypeInfo)
    //    {
    //        this.metadataTypeInfo = metadataTypeInfo;
    //    }


    //    public bool ExistsAnySpecifications()
    //    {
    //        return metadataTypeInfo.GetSpecifications().Any();
    //    }
        
    //    public bool ExistsAnyBehaviors()
    //    {
    //        return metadataTypeInfo.GetBehaviors().Any();
    //    }

    //    public string GetStringOfSubjectAttribute()
    //    {
    //        return metadataTypeInfo.GetSubjectString();
    //    }

       
    //    public bool IsStruct
    //    {
    //        get
    //        {
    //            if (metadataTypeInfo.Base != null)
    //            {
    //                return metadataTypeInfo.Base.Type.FullyQualifiedName == typeof(ValueType).FullName;
    //            }
    //            return false;
    //        }
    //    }

    //    public int GenericParametersCount
    //    {
    //        get { return metadataTypeInfo.GenericParameters.Length; }
    //    }

    //    public bool HasBehaviorAttributeName
    //    {
    //        get { return metadataTypeInfo.HasCustomAttribute(new BehaviorAttributeFullName()); } 
    //    }

    //    public bool IsAbstract
    //    {
    //        get { return metadataTypeInfo.IsAbstract; }
    //    }

    //    public bool IsSealed
    //    {
    //        get { return metadataTypeInfo.IsSealed; }
    //    }

    //}
}