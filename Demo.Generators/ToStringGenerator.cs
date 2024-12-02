using Microsoft.CodeAnalysis; // Provides APIs for working with code analysis.
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax; // Contains syntax-specific nodes for C#.

using System.Text;
using System.Threading;

namespace Demo.Generators;

// Marks this class as a source generator.
[Generator]
public partial class ToStringGenerator : IIncrementalGenerator // Implements incremental generator for efficient code generation.
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Defines a provider that identifies class declaration nodes in the syntax tree.
        IncrementalValuesProvider<ClassDeclarationSyntax> classes = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (SyntaxNode node, CancellationToken token) =>
                {
                    // Checks if the current syntax node is a class declaration.
                    return node is ClassDeclarationSyntax;
                },
                transform: static (GeneratorSyntaxContext ctx, CancellationToken token) =>
                {
                    // Casts and returns the node as a ClassDeclarationSyntax.
                    return (ClassDeclarationSyntax)ctx.Node;
                });

        // Registers a source generator action to execute on identified class nodes.
        context
            .RegisterSourceOutput(
                source: classes,
                action: static (SourceProductionContext ctx, ClassDeclarationSyntax source) =>
                {
                    // Executes the code generation logic for each class.
                    Execute(ctx, source);
                });
    }

    // Generates source code based on the provided class declaration syntax.
    private static void Execute(
        SourceProductionContext context,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        var name = classDeclarationSyntax.Parent switch
        {
            BaseNamespaceDeclarationSyntax bp => bp.Name.ToString(), // Base class
            // NamespaceDeclarationSyntax np => np.Name.ToString(), // Derived class
            // FileScopedNamespaceDeclarationSyntax fp => fp.Name.ToString(), // Derived class
            _ => "No namespace"
        };

        var namespaceName = name;
        var className = classDeclarationSyntax.Identifier.Text; // Extracts the class name.
        var fileName = $"{namespaceName}.{className}.g.cs"; // Creates a generated file name.

        var stringBuilder = new StringBuilder();

        stringBuilder.Append($@"namespace {namespaceName};

public partial class {className}
{{
    public override string ToString()
    {{
        return $""");

        var first = true;

        // getting properties of the class
        foreach (var memberDeclarationSyntax in classDeclarationSyntax.Members)
        {
            if (memberDeclarationSyntax is PropertyDeclarationSyntax propertyDeclarationSyntax &&
                propertyDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                // checking if the member is of type property and is public
                /*
                   namespace Demo.ConsoleApp.Model; -- FileScopedNamespaceDeclarationSyntax

                   public partial class Person -- ClassDeclarationSyntax
                    {
                        public string? FirstName { get; set; } -- PropertyDeclarationSyntax && SyntaxKind.PublicKeyword
                        internal string? MiddleName { get; set; }
                        public string? LastName { get; set; } -- PropertyDeclarationSyntax && SyntaxKind.PublicKeyword
                    }
                 */
                if (first)
                {
                    first = false;
                }
                else
                {
                    stringBuilder.Append("; ");
                }
                var propertyName = propertyDeclarationSyntax.Identifier.Text;
                stringBuilder.Append($"{propertyName}: {{{propertyName}}}");
            }
        }

        stringBuilder.Append($@""";

    }}
}}
"
);

        // Adds generated code to the compilation.
        context.AddSource(hintName: fileName, source: stringBuilder.ToString());
    }
}