using System;

using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.ConsoleRunner.Outputs
{
    public class ColorOutput : IOutput
    {
        readonly IOutput _inner;

        public ColorOutput(IOutput inner)
        {
            _inner = inner;
        }

        public void RunStart()
        {
            _inner.RunStart();
        }

        public void RunEnd()
        {
            _inner.RunEnd();
        }

        public void AssemblyStart(AssemblyInfo assembly)
        {
            _inner.AssemblyStart(assembly);
        }

        public void AssemblyEnd(AssemblyInfo assembly)
        {
            _inner.AssemblyEnd(assembly);
        }

        public void ContextStart(ContextInfo context)
        {
            _inner.ContextStart(context);
        }

        public void ContextEnd(ContextInfo context)
        {
            _inner.ContextEnd(context);
        }

        public void SpecificationStart(SpecificationInfo specification)
        {
        }

        public void Passing(SpecificationInfo specification)
        {
            Color(ConsoleColor.Green, () =>
            {
                _inner.SpecificationStart(specification);
                _inner.Passing(specification);
            });
        }

        public void NotImplemented(SpecificationInfo specification)
        {
            Color(ConsoleColor.Gray, () =>
            {
                _inner.SpecificationStart(specification);
                _inner.NotImplemented(specification);
            });
        }

        public void Ignored(SpecificationInfo specification)
        {
            Color(ConsoleColor.Yellow, () =>
            {
                _inner.SpecificationStart(specification);
                _inner.Ignored(specification);
            });
        }

        public void Failed(SpecificationInfo specification, Result result)
        {
            Color(ConsoleColor.Red, () =>
            {
                _inner.SpecificationStart(specification);
                _inner.Failed(specification, result);
            });
        }

        static void Color(ConsoleColor color, Action action)
        {
            try
            {
                Console.ForegroundColor = color;
                action();
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}