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

    public class BehaviorElement : FieldElement
    {
        readonly string _id;

        public BehaviorElement(MSpecUnitTestProvider provider,
                               IPsi psiModuleManager,
                               ICache cacheManager,
            // ReSharper disable SuggestBaseTypeForParameter
                               ContextElement context,
            // ReSharper restore SuggestBaseTypeForParameter
                               ProjectModelElementEnvoy projectEnvoy,
                               IClrTypeName declaringTypeName,
                               string fieldName,
                               bool isIgnored,
                               string fieldType)
            : base(provider,
                   psiModuleManager,
                   cacheManager,
                   context,
                   projectEnvoy,
                   declaringTypeName,
                   fieldName,
                   isIgnored || context.Explicit)
        {
            this.FieldType = fieldType;
            this._id = CreateId(context, fieldType, fieldName);
        }

        public ContextElement Context
        {
            get { return (ContextElement)this.Parent; }
        }

        public string FieldType { get; private set; }

        public override string Kind
        {
            get { return "Behavior"; }
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

        public override string GetTitlePrefix()
        {
            return "behaves like";
        }

        public override void WriteToXml(XmlElement parent)
        {
            base.WriteToXml(parent);
            parent.SetAttribute("fieldType", this.FieldType);
        }

        public static IUnitTestElement ReadFromXml(XmlElement parent,
                                                   IUnitTestElement parentElement,
                                                   ISolution solution,
                                                   BehaviorFactory factory)
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
            var fieldType = parent.GetAttribute("fieldType");

            return factory.GetOrCreateBehavior(context,
                                               new ClrTypeName(typeName),
                                               methodName,
                                               isIgnored,
                                               fieldType);
        }

        public static string CreateId(ContextElement contextElement, string fieldType, string fieldName)
        {
            var result = new[] { contextElement.Id, fieldType, fieldName };
            return result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
        }
    }
}
