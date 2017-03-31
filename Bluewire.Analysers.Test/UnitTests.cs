using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Bluewire.Analysers;

namespace Bluewire.Analysers.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        [TestMethod]
        public void EmptyFileYieldsNoDiagnostics()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        private DiagnosticResult fileNameDiagnostic = new DiagnosticResult
            {
                Id = "BluewireAnalysersMigrationStepFilePath",
                Message = "Migration step 'TypeName' is in a folder other than that required by its ScmMigrationAttribute.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 12, 22)
                        }
            };

        [TestMethod]
        public void IncorrectNamespaceYieldsNamespaceDiagnostic()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        [ScmMigrationAttribute(2017, 01, 01, 12, 12, ""Alex Davidson"")]
        public class TypeName
        {   
        }
    }" + ScmMigrationAttributeClass;
            var expected = new DiagnosticResult
            {
                Id = "BluewireAnalysersMigrationStepNamespace",
                Message = "Migration step 'TypeName' is in a namespace other than that required by its ScmMigrationAttribute.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 12, 22)
                        }
            };

            VerifyCSharpDiagnostic(test, expected, fileNameDiagnostic);
        }

        [TestMethod]
        public void CorrectNamespaceDoesNotYieldNamespaceDiagnostic()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1._2017._01
    {
        [ScmMigrationAttribute(2017, 01, 01, 12, 12, ""Alex Davidson"")]
        public class TypeName
        {   
        }
    }" + ScmMigrationAttributeClass;

            VerifyCSharpDiagnostic(test, fileNameDiagnostic);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new BluewireAnalysersAnalyzer();
        }

        private static string ScmMigrationAttributeClass = @"
        public class ScmMigrationAttribute : Attribute
        {
            public ScmMigrationAttribute(int year, int month, int day, int hour, int minute, string author) {}
        }
";
    }
}
