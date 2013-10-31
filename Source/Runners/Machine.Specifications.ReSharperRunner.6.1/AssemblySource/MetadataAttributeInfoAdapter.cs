using System;
using System.Linq;
using JetBrains.Metadata.Reader.API;

using Machine.Specifications.Sdk;

namespace XunitContrib.Runner.ReSharper.UnitTestProvider
{
    internal class MetadataAttributeInfoAdapter : IAttributeInfo
    {
        readonly IMetadataCustomAttribute attribute;

        public MetadataAttributeInfoAdapter(IMetadataCustomAttribute attribute)
        {
            this.attribute = attribute;
        }

        public T GetInstance<T>() where T : Attribute
        {
            return null;
        }

        public TValue GetPropertyValue<TValue>(string propertyName)
        {
            var values = (from property in attribute.InitializedProperties
                          where property.Property.Name == propertyName
                          select (TValue)property.Value.Value).ToList();

            // We can only reliably get the value if the constructor arguments
            // are named parameters (i.e. properties). If they're positional
            // args, we have to hope that the arg name is pretty much the same
            // as the property. This is the only way we can get TraitAttribute
            // to work
            if (values.Count == 0)
            {
                try
                {
                    var parameters = attribute.UsedConstructor.Parameters;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (string.Compare(parameters[i].Name, propertyName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            return (TValue)attribute.ConstructorArguments[i].Value;
                        }
                    }
                }
                catch (Exception)
                {
                    return default(TValue);
                }
            }

            return values.FirstOrDefault();
        }
    }
}