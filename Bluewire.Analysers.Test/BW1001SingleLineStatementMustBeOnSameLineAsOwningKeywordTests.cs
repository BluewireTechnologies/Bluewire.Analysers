using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using NUnit.Framework;
using TestHelper;

namespace Bluewire.Analysers.Test
{
    [TestFixture]
    public class BW1001SingleLineStatementMustBeOnSameLineAsOwningKeywordTests : CodeFixVerifier
    {
        [Test]
        public void EmptyFileYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining("");
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void IfBlocksWithBracesYieldNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1)
                {
                    Console.WriteLine();
                }
                if (1 == 1) {
                    Console.WriteLine();
                }
                if (1 == 1) { Console.WriteLine(); }
                if (1 == 1) {
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineIfBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1) Console.WriteLine();
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void StackedSingleLineIfBlockYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1) Console.WriteLine();            // 4
                else if (2 == 2) Console.WriteLine();       // 5
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(4, 29));
        }

        [Test]
        public void SingleLineIfBlockOnNewLineYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1)
                    Console.WriteLine();                    // 5
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(5, 21));
        }

        [Test]
        public void MultiLineIfBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1)
                {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void StackedMultiLineIfBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1)
                {
                    Console.WriteLine();
                }
                else if (2 == 2)
                {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineUsingBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                using (Disposable.Empty) Console.WriteLine();
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineUsingBlockOnNewLineYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                using (Disposable.Empty)
                    Console.WriteLine();                    // 5
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(5, 21));
        }

        [Test]
        public void StackedMultiLineUsingBlocksYieldNoDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                using (Disposable.Empty)
                using (Disposable.Empty)
                using (Disposable.Empty)
                {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void MultiLineUsingBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                using (Disposable.Empty)
                {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineWhileBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                while (1 == 1) Console.WriteLine();
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineWhileBlockOnNewLineYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                while (1 == 1)
                    Console.WriteLine();                    // 5
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(5, 21));
        }

        [Test]
        public void MultiLineWhileBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                while (1 == 1)
                {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void DoWhileStatementOnSingleLineYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                do Console.WriteLine() while (1 == 1);      // 4
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void DoWhileStatementOnTwoLinesYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                do Console.WriteLine()                     // 4
                while (1 == 1);
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(4, 20));
        }

        [Test]
        public void DoWhileStatementOnNewLineYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                do
                    Console.WriteLine()                     // 5
                while (1 == 1);
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(5, 21));
        }

        [Test]
        public void MultiLineDoWhileBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                do
                {
                    Console.WriteLine();
                }
                while (1 == 1);      // 4
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineForEachBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                foreach (var x in new List<int>()) Console.WriteLine();
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineForEachBlockOnNewLineYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                foreach (var x in new List<int>())
                    Console.WriteLine();                    // 5
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(5, 21));
        }

        [Test]
        public void MultiLineForEachBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                foreach (var x in new List<int>())
                {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineForBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                for (var i = 0; i < 10; i++) Console.WriteLine();
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineForBlockOnNewLineYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                for (var i = 0; i < 10; i++)
                    Console.WriteLine();                    // 5
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(5, 21));
        }

        [Test]
        public void MultiLineForBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void IgnoresGeneratedFile()
        {
            var code = "//<auto-generated/>\n" + ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                for (var i = 0; i < 10; i++)
                    Console.WriteLine();
            }");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new BW1001SingleLineStatementMustBeOnSameLineAsOwningKeyword();

        private DiagnosticResultLocation CreateLocation(int line, int column) =>  new DiagnosticResultLocation("Test0.cs", line + ModelProvider.LineOffset, column);
        private DiagnosticResult CreateResult(DiagnosticDescriptor rule, DiagnosticResultLocation location) => new DiagnosticResult
        {
            Id = rule.Id,
            Message = String.Format(rule.MessageFormat.ToString()),
            Severity = rule.DefaultSeverity,
            Locations = new[] { location }
        };

        private DiagnosticResult Diagnostic(int line, int column) => CreateResult(BW1001SingleLineStatementMustBeOnSameLineAsOwningKeyword.Descriptor, CreateLocation(line, column));
    }
}
