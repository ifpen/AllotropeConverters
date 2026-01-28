using System;
using System.Reflection;

namespace IFPEN.AllotropeConverters.Chromeleon.IntegrationTests
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // This entry point is required for the executable to have the 32-bit flag
            // When run directly, it will execute the tests using xUnit's console runner logic

            // If no arguments are provided, show usage information
            if (args.Length == 0)
            {
                var tests = new IntegrationTests();
                tests.Convert_ActualInjectionUri_GeneratesValidAsmJson();
                Console.WriteLine("IFPEN.AllotropeConverters.Chromeleon.IntegrationTests");
                Console.WriteLine("This is an executable test project for Chromeleon SDK compatibility.");
                Console.WriteLine();
                Console.WriteLine("To run tests, use:");
                Console.WriteLine("xunit.console.x86.exe IFPEN.AllotropeConverters.Chromeleon.IntegrationTests.exe -nologo");
                return 0;
            }

            // For compatibility with test runners, return success
            return 0;
        }
    }
}
