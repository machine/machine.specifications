using System;
using System.Collections.Generic;

using JetBrains.ReSharper.TaskRunnerFramework.UnitTesting;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
    internal class UnitTestElement: IUnitTestElement
    {
        public string TypeName { get; protected set; }
        public string AssemblyLocation { get; private set; }

        public UnitTestElement(IUnitTestRunnerProvider provider, string typeName, string shortName, string assemblyLocation)
        {
            Provider = provider;
            TypeName = typeName;
            ShortName = shortName;
            AssemblyLocation = assemblyLocation;

            Children = new List<IUnitTestElement>();
            State = UnitTestElementState.Valid;
        }

        public bool Equals(IUnitTestElement other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other.GetType() == GetType())
            {
                var element = (UnitTestElement)other;
                return other.ShortName == ShortName && other.Provider == Provider
                    && element.AssemblyLocation == AssemblyLocation;
            }
            return false;
        }

        public IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
        {
            // TODO: HADI
            var unitTestTasks = new List<UnitTestTask>();

            //                        {
            //                            new UnitTestTask(null, new XunitTestAssemblyTask(AssemblyLocation)),
            //                            new UnitTestTask(this, new XunitTestClassTask(AssemblyLocation, TypeName, Explicit))
            //                        };
            //
            return unitTestTasks;
        }

        public string Id
        {
            get { return TypeName; }
        }

        public IUnitTestRunnerProvider Provider { get; private set; }
        public IUnitTestElement Parent { get; set; }
        public ICollection<IUnitTestElement> Children { get; private set; }
        public string ShortName { get; private set; }
        
        public bool Explicit
        {
            get { return false; }
        }

        public UnitTestElementState State { get; set; }
    }
}