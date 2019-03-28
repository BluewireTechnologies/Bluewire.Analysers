namespace Bluewire.Analysers.Test
{
    public class ModelProvider
    {
        public static string CreateSourceCodeContaining(string methodDef)
        {
            return $@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Bluewire.Common.Server.DataAccess;

    namespace ConsoleApplication1
    {{
        public class Program
        {{
{methodDef}
        }}
    }}
";
        }

        public const int LineOffset = 13;
    }
}
