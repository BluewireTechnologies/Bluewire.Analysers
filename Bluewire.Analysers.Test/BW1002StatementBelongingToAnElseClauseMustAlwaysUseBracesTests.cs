﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using TestHelper;
using NUnit.Framework;

namespace Bluewire.Analysers.Test
{
    [TestFixture]
    public class BW1002StatementBelongingToAnElseClauseMustAlwaysUseBracesTests : CodeFixVerifier
    {
        [Test]
        public void EmptyFileYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining("");
            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void IfBlocksWithoutElseYieldNoDiagnostics()
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
                if (1 == 1) Console.WriteLine();
                if (1 == 1) {
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void SingleLineElseBlockYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1) Console.WriteLine();
                else Console.WriteLine();           // 5
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(5, 22));
        }

        [Test]
        public void SingleLineElseBlockOnNewLineYieldsDiagnostic()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1) Console.WriteLine();
                else
                    Console.WriteLine();            // 6
            }");

            VerifyCSharpDiagnostic(code, Diagnostic(6, 21));
        }

        [Test]
        public void MultiLineElseBlockYieldsNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1) Console.WriteLine();
                else
                {
                    Console.WriteLine();
                }
                if (1 == 1) Console.WriteLine();
                else {
                    Console.WriteLine();
                }
            }");

            VerifyCSharpDiagnostic(code);
        }

        [Test]
        public void MultiLineStackedIfElseBlocksYieldNoDiagnostics()
        {
            var code = ModelProvider.CreateSourceCodeContaining(@"
            public void Test()
            {
                if (1 == 1) Console.WriteLine();
                else if (2 == 2)
                {
                    Console.WriteLine();
                }
                if (1 == 1) Console.WriteLine();
                else if (2 == 2) {
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
                if (1 == 1) Console.WriteLine();
                else Console.WriteLine();           // 5
            }");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new BW1002StatementBelongingToAnElseClauseMustAlwaysUseBraces();

        private DiagnosticResultLocation CreateLocation(int line, int column) =>  new DiagnosticResultLocation("Test0.cs", line + ModelProvider.LineOffset, column);
        private DiagnosticResult CreateResult(DiagnosticDescriptor rule, DiagnosticResultLocation location) => new DiagnosticResult
        {
            Id = rule.Id,
            Message = String.Format(rule.MessageFormat.ToString()),
            Severity = rule.DefaultSeverity,
            Locations = new[] { location }
        };

        private DiagnosticResult Diagnostic(int line, int column) => CreateResult(BW1002StatementBelongingToAnElseClauseMustAlwaysUseBraces.Descriptor, CreateLocation(line, column));
    }
}