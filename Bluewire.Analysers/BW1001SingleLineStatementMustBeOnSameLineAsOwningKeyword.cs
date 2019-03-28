using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bluewire.Analysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BW1001SingleLineStatementMustBeOnSameLineAsOwningKeyword : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BW1001";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            DiagnosticId,
            "Single-line statement must be on same line as owning keyword",
            "Single-line statement must be on same line as owning keyword",
            "Bluewire.Analysers.Layout", DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The statement associated with a keyword such as 'if' or 'using' must either be a multi-line block with braces or reside entirely on the same line as the keyword.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxTreeAction(AnalyzeTree);
        }

        private void AnalyzeTree(SyntaxTreeAnalysisContext context)
        {
            var visitor = new Visitor(context.ReportDiagnostic);
            var root = context.Tree.GetRoot(context.CancellationToken);
            visitor.Visit(root);
        }

        private class Visitor : CSharpSyntaxVisitor
        {
            private readonly Action<Diagnostic> report;

            public Visitor(Action<Diagnostic> report)
            {
                this.report = report;
            }

            private void Fail(SyntaxNode statement) => report(Diagnostic.Create(Descriptor, statement.GetLocation()));

            private void ValidateIfSingleLine(SyntaxNode containingStatement, SyntaxNode statement)
            {
                if (statement is BlockSyntax) return;
                var containingStatementSpan = containingStatement.GetLocation().GetLineSpan();
                if (containingStatementSpan.StartLinePosition.Line != containingStatementSpan.EndLinePosition.Line)
                {
                    Fail(statement);
                }
            }

            public override void DefaultVisit(SyntaxNode node)
            {
                foreach (var child in node.ChildNodesAndTokens())
                {
                    if (child.IsNode) Visit(child.AsNode());
                }
            }

            public override void VisitIfStatement(IfStatementSyntax node)
            {
                ValidateIfSingleLine(node, node.Statement);
                base.VisitIfStatement(node);
            }

            public override void VisitUsingStatement(UsingStatementSyntax node)
            {
                if (node.Statement is UsingStatementSyntax) return;
                ValidateIfSingleLine(node, node.Statement);
                base.VisitUsingStatement(node);
            }

            public override void VisitWhileStatement(WhileStatementSyntax node)
            {
                ValidateIfSingleLine(node, node.Statement);
                base.VisitWhileStatement(node);
            }

            public override void VisitDoStatement(DoStatementSyntax node)
            {
                ValidateIfSingleLine(node, node.Statement);
                base.VisitDoStatement(node);
            }

            public override void VisitForEachStatement(ForEachStatementSyntax node)
            {
                ValidateIfSingleLine(node, node.Statement);
                base.VisitForEachStatement(node);
            }

            public override void VisitForStatement(ForStatementSyntax node)
            {
                ValidateIfSingleLine(node, node.Statement);
                base.VisitForStatement(node);
            }
        }
    }
}
