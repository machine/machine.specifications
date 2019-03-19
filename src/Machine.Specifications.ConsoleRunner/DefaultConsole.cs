using System;

namespace Machine.Specifications.ConsoleRunner
{
    public class DefaultConsole : IConsole
    {
        public void Write(string line)
        {
            Console.Write(line);
        }

        public void Write(string line, params object[] parameters)
        {
            Console.Write(line, parameters);
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void WriteLine(string line, params object[] parameters)
        {
            Console.WriteLine(line, parameters);
        }
    }
}
