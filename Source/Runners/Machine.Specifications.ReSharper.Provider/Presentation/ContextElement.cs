namespace Machine.Specifications.ReSharperProvider.Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.UnitTestFramework;
    using JetBrains.Util;

    using Machine.Specifications.ReSharperProvider.Factories;
    using Machine.Specifications.ReSharperProvider.Shims;
    using Machine.Specifications.Utility.Internal;

    public class ContextElement : Element, ISerializableElement
    {
        readonly string _assemblyLocation;
        readonly IEnumerable<UnitTestElementCategory> _categories;
        readonly string _id;
        readonly string _subject;

        public ContextElement(MSpecUnitTestProvider provider,
                              IPsi psiModuleManager,
                              ICache cacheManager,
                              ProjectModelElementEnvoy projectEnvoy,
                              IClrTypeName typeName,
                              string assemblyLocation,
                              string subject,
                              IEnumerable<string> tags,
                              bool isIgnored)
            : base(provider, psiModuleManager, cacheManager, null, projectEnvoy, typeName, isIgnored)
        {
            this._id = CreateId(subject, this.TypeName.FullName, tags);
            this._assemblyLocation = assemblyLocation;
            this._subject = subject;

            if (tags != null)
            {
                this._categories = UnitTestElementCategory.Create(new JetHashSet<string>(tags));
            }
        }

        public override string ShortName
        {
            get { return this.Kind + this.GetPresentation(); }
        }

        public string AssemblyLocation
        {
            get { return this._assemblyLocation; }
        }

        public override string Kind
        {
            get { return "Context"; }
        }

        public override IEnumerable<UnitTestElementCategory> Categories
        {
            get { return this._categories; }
        }

        public override string Id
        {
            get { return this._id; }
        }

        public void WriteToXml(XmlElement parent)
        {
            parent.SetAttribute("projectId", this.GetProject().GetPersistentID());
            parent.SetAttribute("typeName", this.TypeName.FullName);
            parent.SetAttribute("assemblyLocation", this.AssemblyLocation);
            parent.SetAttribute("isIgnored", this.Explicit.ToString());
            parent.SetAttribute("subject", this._subject);
        }

        public override string GetPresentation()
        {
            return this.GetSubject() + this.GetTypeClrName().ShortName.ToFormat();
        }

        string GetSubject()
        {
            if (String.IsNullOrEmpty(this._subject))
            {
                return null;
            }

            return this._subject + ", ";
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return this.GetDeclaredType();
        }

        public static IUnitTestElement ReadFromXml(XmlElement parent,
                                                   ISolution solution,
                                                   ContextFactory factory)
        {
            var projectId = parent.GetAttribute("projectId");
            var project = ProjectUtil.FindProjectElementByPersistentID(solution, projectId) as IProject;
            if (project == null)
            {
                return null;
            }

            var typeName = parent.GetAttribute("typeName");
            var assemblyLocation = parent.GetAttribute("assemblyLocation");
            var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));
            var subject = parent.GetAttribute("subject");

            return factory.GetOrCreateContext(assemblyLocation,
                                              project,
                                              new ClrTypeName(typeName),
                                              subject,
                                              EmptyArray<string>.Instance,
                                              isIgnored);
        }

        public static string CreateId(string subject, string typeName, IEnumerable<string> tags)
        {
            string tagsAsString = null;
            if (tags != null)
            {
                tagsAsString = tags.AggregateString("", "|", (builder, tag) => builder.Append(tag));
            }
            var result = new[] { subject, typeName, tagsAsString };
            return result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
        }
    }
}