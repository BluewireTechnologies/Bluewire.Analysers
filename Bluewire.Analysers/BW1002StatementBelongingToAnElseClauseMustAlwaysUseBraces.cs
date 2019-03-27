using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bluewire.Analysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BW1002StatementBelongingToAnElseClauseMustAlwaysUseBraces : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BW1002";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            DiagnosticId,
            "Statement belonging to an 'else' clause must always use braces",
            "Statement belonging to an 'else' clause must always use braces",
            "Bluewire.Analysers.Layout", DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The statement belonging to an 'else' clause must always be contained in braces.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxTreeAction(AnalyzeTree);
        }

        private void AnalyzeTree(SyntaxTreeAnalysisContext obj)
        {
            var visitor = new Visitor(obj.ReportDiagnostic);
            var root = obj.Tree.GetRoot(obj.CancellationToken);
            visitor.Visit(root);
        }

        private class Visitor : CSharpSyntaxVisitor
        {
            private readonly Action<Diagnostic> report;

            public Visitor(Action<Diagnostic> report)
            {
                this.report = report;
            }

            public override void DefaultVisit(SyntaxNode node)
            {
                foreach (var child in node.ChildNodesAndTokens())
                {
                    if (child.IsNode) Visit(child.AsNode());
                }
            }

            public override void VisitElseClause(ElseClauseSyntax node)
            {
                if (node.Statement is BlockSyntax) return;
                if (node.Statement is IfStatementSyntax) return;
                report(Diagnostic.Create(Descriptor, node.Statement.GetLocation()));
                base.VisitElseClause(node);
            }
        }
    }
}
