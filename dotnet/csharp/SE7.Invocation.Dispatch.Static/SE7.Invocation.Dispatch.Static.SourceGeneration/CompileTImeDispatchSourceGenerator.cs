using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace SE7.Invocation.Dispatch.Static.SourceGeneration
{
    [Generator]
    public sealed class CompileTImeDispatchSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //var nonDotNet7VersionProvider = context.CompilationProvider.Select((c, _) =>
            //{
            //    return GetTargetFramework(c);
            //});

            //context.RegisterSourceOutput(nonDotNet7VersionProvider, (spc, nonDotNetVersion) =>
            //{
            //    if (nonDotNetVersion?.Equals(".NETCoreApp,Version=v8.0", StringComparison.OrdinalIgnoreCase) ?? false)
            //    {
            //        spc.ReportDiagnostic(
            //            Diagnostic.Create(
            //                new DiagnosticDescriptor(
            //                    "SE7IDS001",
            //                    ".NET 7+ Required",
            //                    ".NET 7 or later is required to used SE7.Invocation.Dispatch.Static. Current version is {0}",
            //                    ".NET Version",
            //                    DiagnosticSeverity.Error,
            //                    true
            //                ),
            //                Location.None,
            //                nonDotNetVersion
            //            )
            //        );
            //    }
            //    else
            //    {
            //        spc.ReportDiagnostic(
            //            Diagnostic.Create(
            //                new DiagnosticDescriptor(
            //                    "SE7IDS001",
            //                    ".NET 7+ Required",
            //                    ".NET 7 or later is required to used SE7.Invocation.Dispatch.Static. Current version is {0}",
            //                    ".NET Version",
            //                    DiagnosticSeverity.Info,
            //                    true
            //                ),
            //                Location.None,
            //                nonDotNetVersion
            //            )
            //        );
            //    }
            //});

            context.RegisterPostInitializationOutput(igpic =>
            {
                igpic.AddSource(
                    "SealedAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
                            internal sealed class SealedAttribute : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );

                igpic.AddSource(
                    "AbstractAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
                            internal sealed class AbstractAttribute : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );

                igpic.AddSource(
                    "VirtualAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
                            internal sealed class VirtualAttribute : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );

                igpic.AddSource(
                    "InheritsAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
                            internal sealed class InheritsAttribute<TBase> : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );

                igpic.AddSource(
                    "ImplementsAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
                            internal sealed class ImplementsAttribute<TInterface> : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );

                igpic.AddSource(
                    "OverrideAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
                            internal sealed class OverrideAttribute : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );

#warning TODO: GENERATE unique nested BaseAttribute for each class, because each class's base class will have different contexts and paramters

                igpic.AddSource(
                    "InterfaceImplementationAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
                            internal sealed class InterfaceImplementationAttribute : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );

                igpic.AddSource(
                    "NewAttribute.g.cs",
                    SourceText.From(
                        """
                        using System;

                        namespace SE7.Invocation.Dispatch.Static
                        {
                            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
                            internal sealed class NewAttribute : Attribute
                            {
                            }
                        }
                        """,
                        Encoding.UTF8
                    )
                );
            });

            var sealedClassesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.SealedAttribute",
                static (sn, _) => sn is ClassDeclarationSyntax @class && @class.Modifiers.Any(SyntaxKind.PartialKeyword),
                static (gasc, _) =>
                {
                    return new Class(gasc.TargetSymbol.Name, Class.ClassModifiers.Sealed);
                }
            );

            var abstractClassesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.AbstractAttribute",
                static (sn, _) => sn is ClassDeclarationSyntax @class && @class.Modifiers.Any(SyntaxKind.PartialKeyword),
                static (gasc, _) =>
                {
                    var symbolDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

                    if (
#error TODO: CHECK ENTIRE INHERITANCE HIERARCHY
                        ((INamedTypeSymbol)gasc.TargetSymbol).BaseType?
                            .GetAttributes()
                            .Any(ad => ad.AttributeClass?.ToDisplayString(symbolDisplayFormat) == "SE7.Invocation.Dispatch.Static.SealedAttribute") ?? false
                    )
                    {

                    }

                    return new Class(gasc.TargetSymbol.Name, Class.ClassModifiers.Abstract);
                }
            ).Where(c => c is not null).Select((c, _) => c!);

            var virtualPropertiesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.VirtualAttribute",
                static (sn, _) => sn is PropertyDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Property(gasc.TargetSymbol.Name);
                }
            );

            var virtualMethodsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.VirtualAttribute",
                static (sn, _) => sn is MethodDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Method(gasc.TargetSymbol.Name);
                }
            );

            var inheritsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.InheritsAttribute`1",
                static (sn, _) => sn is ClassDeclarationSyntax @class && @class.Modifiers.Any(SyntaxKind.PartialKeyword),
                static (gasc, _) =>
                {
                    return new Class(gasc.TargetSymbol.Name, Class.ClassModifiers.None);
                }
            );

            var implementsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.ImplementsAttribute`1",
                static (sn, _) => sn is ClassDeclarationSyntax @class && @class.Modifiers.Any(SyntaxKind.PartialKeyword),
                static (gasc, _) =>
                {
                    return new Class(gasc.TargetSymbol.Name, Class.ClassModifiers.None);
                }
            );

            var overrideMethodsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.OverrideAttribute",
                static (sn, _) => sn is MethodDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Method(gasc.TargetSymbol.Name);
                }
            );

            var overridePropertiesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.OverrideAttribute",
                static (sn, _) => sn is PropertyDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Property(gasc.TargetSymbol.Name);
                }
            );

            var methodInterfaceImplementationsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.InterfaceImplementationAttribute",
                static (sn, _) => sn is MethodDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Method(gasc.TargetSymbol.Name);
                }
            );

            var propertyInterfaceImplementationsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.InterfaceImplementationAttribute",
                static (sn, _) => sn is PropertyDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Property(gasc.TargetSymbol.Name);
                }
            );

            var newMethodProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.NewAttribute",
                static (sn, _) => sn is MethodDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Method(gasc.TargetSymbol.Name);
                }
            );

            var newPropertyProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "SE7.Invocation.Dispatch.Static.NewAttribute",
                static (sn, _) => sn is PropertyDeclarationSyntax,
                static (gasc, _) =>
                {
                    return new Property(gasc.TargetSymbol.Name);
                }
            );
        }

        private static string? GetTargetFramework(Compilation compilation)
        {
            var targetFrameworkAttr = compilation.Assembly
                .GetAttributes()
                .FirstOrDefault(attr =>
                    attr.AttributeClass?.ToDisplayString() == "System.Runtime.Versioning.TargetFrameworkAttribute");

            // Returns a string like ".NETCoreApp,Version=v8.0" or ".NETFramework,Version=v4.8"
            return targetFrameworkAttr?.ConstructorArguments.FirstOrDefault().Value as string;
        }
    }
}
