using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bluewire.Analysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BluewireAnalysersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BluewireAnalysers";

        private static readonly DiagnosticDescriptor NamespaceRule = new DiagnosticDescriptor(
            "BluewireAnalysersMigrationStepNamespace",
            "Migration step is in the wrong namespace",
            "Migration step '{0}' is in a namespace other than that required by its ScmMigrationAttribute.",
            "Naming", DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Migration steps should be contained within a namespace matching their year and month.");
        private static readonly DiagnosticDescriptor FilePathRule = new DiagnosticDescriptor(
            "BluewireAnalysersMigrationStepFilePath",
            "Migration step is in the wrong folder",
            "Migration step '{0}' is in a folder other than that required by its ScmMigrationAttribute.",
            "Naming", DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Migration steps should be contained within a folder matching their year and month.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(NamespaceRule, FilePathRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var attributes = context.Symbol.GetAttributes();
            var scmMigrationAttribute = attributes.FirstOrDefault(a => a.AttributeClass.Name == "ScmMigrationAttribute");
            if (scmMigrationAttribute == null) return;

            int month, year;
            if (!TryGetParameterValueByName(scmMigrationAttribute, "year", out year)) return;
            if (!TryGetParameterValueByName(scmMigrationAttribute, "month", out month)) return;

            VerifyNamespace(context, year, month);
            VerifyFilePath(context, year, month);
        }

        private static void VerifyNamespace(SymbolAnalysisContext context, int year, int month)
        {
            if (Regex.IsMatch(context.Symbol.ContainingNamespace.ToDisplayString(), $@"\b[^\d]*{year}\.[^\d]*0*{month}\b")) return;

            context.ReportDiagnostic(Diagnostic.Create(NamespaceRule, context.Symbol.Locations[0], context.Symbol.Name));
        }

        private static void VerifyFilePath(SymbolAnalysisContext context, int year, int month)
        {
            foreach (var location in context.Symbol.Locations)
            {
                if (!location.IsInSource) continue;
                if (ContainsYearAndMonth(location.SourceTree.FilePath, year, month)) continue;

                context.ReportDiagnostic(Diagnostic.Create(FilePathRule, location, context.Symbol.Name));
            }
        }

        public static bool ContainsYearAndMonth(string path, int year, int month)
        {
            using (var segments = path.Split('\\').AsEnumerable().GetEnumerator())
            {
                if (!Find(segments, year)) return false;
                return Find(segments, month);
            }
        }

        private static bool Find(IEnumerator<string> iter, int value)
        {
            while (iter.MoveNext())
            {
                int number;
                if (int.TryParse(iter.Current, out number))
                {
                    if (value == number) return true;
                }
            }
            return false;
        }

        private static bool TryGetParameterValueByName(AttributeData attr, string name, out int value)
        {
            value = 0;
            for (var i = 0; i < attr.ConstructorArguments.Length; i ++)
            {
                var arg = attr.AttributeConstructor.Parameters.ElementAtOrDefault(i);
                if (arg == null) break;
                if (StringComparer.OrdinalIgnoreCase.Equals(arg.Name, name))
                {
                    var argValue = attr.ConstructorArguments[i].Value;
                    try
                    {
                        value = Convert.ToInt32(argValue);
                        return true;
                    }
                    catch (InvalidCastException)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
