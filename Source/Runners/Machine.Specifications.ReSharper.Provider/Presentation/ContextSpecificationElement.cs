namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;

    using Machine.Specifications.ReSharperProvider.Factories;
    using Machine.Specifications.ReSharperProvider.Shims;

    public class ContextSpecificationElement : FieldElement
    {
        readonly string _id;

        public ContextSpecificationElement(MSpecUnitTestProvider provider,
                                           IPsi psiModuleManager,
                                           ICache cacheManager,
                                           ProjectModelElementEnvoy project,
                                           ContextElement context,
                                           IClrTypeName declaringTypeName,
                                           string fieldName,
                                           bool isIgnored)
            : base(provider,
                   psiModuleManager,
                   cacheManager,
                   context,
                   project,
                   declaringTypeName,
                   fieldName,
                   isIgnored || context.Explicit)
        {
            this._id = CreateId(context, fieldName);
        }

        public ContextElement Context
        {
            get { return (ContextElement)this.Parent; }
        }

        public override string Kind
        {
            get { return "Specification"; }
        }

        public override IEnumerable<UnitTestElementCategory> Categories
        {
            get
            {
                var parent = this.Parent ?? this.Context;
                if (parent == null)
                {
                    return UnitTestElementCategory.Uncategorized;
                }

                return parent.Categories;
            }
        }

        public override string Id
        {
            get { return this._id; }
        }

        public static IUnitTestElement ReadFromXml(XmlElement parent,
                                                   IUnitTestElement parentElement,
                                                   ISolution solution,
                                                   ContextSpecificationFactory factory)
        {
            var projectId = parent.GetAttribute("projectId");
            var project = ProjectUtil.FindProjectElementByPersistentID(solution, projectId) as IProject;
            if (project == null)
            {
                return null;
            }

            var context = parentElement as ContextElement;
            if (context == null)
            {
                return null;
            }

            var typeName = parent.GetAttribute("typeName");
            var methodName = parent.GetAttribute("methodName");
            var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));

            return factory.GetOrCreateContextSpecification(context,
                                                           new ClrTypeName(typeName),
                                                           methodName,
                                                           isIgnored);
        }

        public static string CreateId(ContextElement contextElement, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldName };
            return result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
        }
    }
}
