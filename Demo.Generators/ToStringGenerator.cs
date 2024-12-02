using Microsoft.CodeAnalysis; // Provides APIs for working with code analysis.
using Microsoft.CodeAnalysis.CSharp.Syntax; // Contains syntax-specific nodes for C#.

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
        var className = classDeclarationSyntax.Identifier.Text; // Extracts the class name.
        var fileName = $"{className}.g.cs"; // Creates a generated file name.

        // Adds generated code to the compilation.
        context.AddSource(hintName: fileName, source: "// Generated!");
    }
}