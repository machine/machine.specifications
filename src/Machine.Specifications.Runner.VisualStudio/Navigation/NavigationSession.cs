using System.Linq;
using System.Reflection.Emit;
using Machine.Specifications.Runner.VisualStudio.Reflection;

namespace Machine.Specifications.Runner.VisualStudio.Navigation
{
    public class NavigationSession : INavigationSession
    {
        private readonly AssemblyData assembly;

        public NavigationSession(string assemblyPath)
        {
            assembly = AssemblyData.Read(assemblyPath);
        }

        public NavigationData GetNavigationData(string typeName, string fieldName)
        {
            var type = assembly.Types.FirstOrDefault(x => x.TypeName == typeName);
            var method = type?.Constructors.FirstOrDefault();

            if (method == null)
            {
                return NavigationData.Unknown;
            }

            var instruction = method.Instructions
                .Where(x => x.OperandType == OperandType.InlineField)
                .FirstOrDefault(x => x.Name == fieldName);

            while (instruction != null)
            {
                var sequencePoint = method.GetSequencePoint(instruction);

                if (sequencePoint != null && !sequencePoint.IsHidden)
                {
                    return new NavigationData(sequencePoint.FileName, sequencePoint.StartLine);
                }

                instruction = instruction.Previous;
            }

            return NavigationData.Unknown;
        }

        public void Dispose()
        {
            assembly.Dispose();
        }
    }
}
