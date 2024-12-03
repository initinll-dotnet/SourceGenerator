using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Text;

namespace Demo.Generators.CustomGenerators.SyntaxApi;

// Declares this class as a source generator.
[Generator]
public partial class ToStringGeneratorV1 : IIncrementalGenerator // Implements incremental generation for efficient and optimized code creation.
{
    private const string GENERATOR_NAMESPACE = "Demo.Generators.CustomGenerators.SyntaxApi";
    private const string ATTRIBUTE_MARKER = "GenerateToStringV1Attribute";

    // Initializes the generator and registers syntax and source providers.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //Creates a syntax provider that identifies class declarations with specific attributes.
        var classes = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsSyntaxTarget(node), // Checks if the node is a class with attributes.
                transform: static (ctx, _) => GetSemanticTarget(ctx)) // Extracts the class if it has the target attribute.
            .Where(static (target) => target is not null); // Filters out null results.

        //Registers the main code generation action using the identified class nodes.
        context.RegisterSourceOutput(
            source: classes,
            action: static (ctx, source)
                => Execute(ctx, source!)); // Executes code generation for each valid class.

        // Registers a post-initialization step to inject the `GenerateToString` attribute definition.
        context.RegisterPostInitializationOutput(
            callback: static (ctx)
                => PostInitializationOutput(ctx));
    }

    // Checks if the syntax node is a class declaration with attributes.
    private static bool IsSyntaxTarget(SyntaxNode node)
    {
        return
            node is ClassDeclarationSyntax classDeclarationSyntax && // Ensures it's a class declaration.
            classDeclarationSyntax.AttributeLists.Count > 0; // Checks if the class has any attributes.
    }

    // Returns the class declaration if it contains the `GenerateToString` attribute.
    private static ClassDeclarationSyntax? GetSemanticTarget(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        // Iterates through the class's attribute lists.
        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                var attributeName = attributeSyntax.Name.ToString();

                // Checks if the attribute matches `GenerateToString` or `GenerateToStringAttribute`.
                if (attributeName == "GenerateToStringV1" ||
                    attributeName == $"{ATTRIBUTE_MARKER}")
                {
                    return classDeclarationSyntax; // Returns the class if it contains the target attribute.
                }
            }
        }

        return null; // Returns null if no matching attribute is found.
    }

    // Injects the `GenerateToStringAttribute` class into the compilation.
    private static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        var fileName = $"{GENERATOR_NAMESPACE}.{ATTRIBUTE_MARKER}.g.cs";
        var source = @$"namespace {GENERATOR_NAMESPACE};

internal class {ATTRIBUTE_MARKER} : System.Attribute {{ }}"; // Defines a custom attribute for marking classes.

        context.AddSource(hintName: fileName, source: source); // Adds the attribute class to the generated sources.
    }

    // Main method for generating the `ToString` method for identified classes.
    private static void Execute(
        SourceProductionContext context,
        ClassDeclarationSyntax classDeclarationSyntax)
    {
        var name = classDeclarationSyntax.Parent switch
        {
            BaseNamespaceDeclarationSyntax bp => bp.Name.ToString(), // Extracts namespace from base declaration.
            _ => "No namespace" // Default if no namespace is found.
        };

        var namespaceName = name;
        var className = classDeclarationSyntax.Identifier.Text;
        var fileName = $"{namespaceName}.{className}.g.cs"; // Constructs the output file name.

        var content =
            GetContent(namespaceName, className, classDeclarationSyntax); // Generates the `ToString` method content.

        context.AddSource(hintName: fileName, source: content); // Adds the generated source to the compilation.
    }

    // Generates the `ToString` method for a given class, including all public properties.
    private static string GetContent(string namespaceName, string className, ClassDeclarationSyntax classDeclarationSyntax)
    {
        var stringBuilder = new StringBuilder();

        // Builds the class definition and `ToString` method.
        stringBuilder.Append($@"namespace {namespaceName};

public partial class {className}
{{
    public override string ToString()
    {{
        return $""");

        var first = true;

        // Iterates through all members of the class to find public properties.
        foreach (var memberDeclarationSyntax in classDeclarationSyntax.Members)
        {
            if (memberDeclarationSyntax is PropertyDeclarationSyntax propertyDeclarationSyntax &&
                propertyDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword)) // Checks for public properties.
            {
                if (!first)
                {
                    stringBuilder.Append("; "); // Separates properties with semicolons.
                }
                first = false;
                var propertyName = propertyDeclarationSyntax.Identifier.Text;
                stringBuilder.Append($"{propertyName}: {{{propertyName}}}"); // Formats property name and value.
            }
        }

        stringBuilder.Append($@"""; }} // Closes the generated `ToString` method.
}}
");

        return stringBuilder.ToString(); // Returns the complete generated source code.
    }
}