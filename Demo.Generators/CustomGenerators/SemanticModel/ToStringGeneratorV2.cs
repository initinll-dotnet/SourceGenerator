using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;
using System.Text;

namespace Demo.Generators.CustomGenerators.SemanticModel;

// Declares this class as a source generator.
[Generator]
public partial class ToStringGeneratorV2 : IIncrementalGenerator // Implements incremental generation for efficient and optimized code creation.
{
    private const string GENERATOR_NAMESPACE = "Demo.Generators.CustomGenerators.SemanticModel";
    private const string ATTRIBUTE_MARKER = "GenerateToStringV2Attribute";

    // Initializes the generator and registers syntax and source providers.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsSyntaxTarget(node), // Checks if the node is a class with attributes.
                transform: static (ctx, _) => GetSemanticTarget(ctx)) // Extracts the class if it has the target attribute.
            .Where(static (target) => target is not null); // Filters out null results.

        // Registers the main code generation action using the identified class nodes.
        context.RegisterSourceOutput(
                source: classes,
                action: static (ctx, source)
                    => Execute(ctx, source)); // Executes code generation for each valid class.

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

    private static ClassToGenerate? GetSemanticTarget(GeneratorSyntaxContext context)
    {
        // Syntax API
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        // Semantic Model
        var classSymbol = context
            .SemanticModel
            .GetDeclaredSymbol(classDeclarationSyntax);

        var attributeSymbol = context
            .SemanticModel
            .Compilation
            .GetTypeByMetadataName($"{GENERATOR_NAMESPACE}.{ATTRIBUTE_MARKER}");

        if (classSymbol is not null && attributeSymbol is not null)
        {
            foreach (var attributeData in classSymbol.GetAttributes())
            {
                if (attributeSymbol.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                {
                    var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
                    var className = classSymbol.Name;
                    var propertyNames = new List<string>();

                    foreach (var memberSymbol in classSymbol.GetMembers())
                    {
                        if (memberSymbol.Kind == SymbolKind.Property &&
                            memberSymbol.DeclaredAccessibility == Accessibility.Public)
                        {
                            propertyNames.Add(memberSymbol.Name);
                        }
                    }

                    return new ClassToGenerate(namespaceName, className, propertyNames);
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

    private static Dictionary<string, int> _countPerFileName = new();

    private static void Execute(
        SourceProductionContext context,
        ClassToGenerate? classToGenerate)
    {
        if (classToGenerate is null)
        {
            return;
        }

        var namespaceName = classToGenerate.NamespaceName;
        var className = classToGenerate.ClassName;
        var fileName = $"{namespaceName}.{className}.g.cs";

        if (_countPerFileName.ContainsKey(fileName))
        {
            _countPerFileName[fileName]++;
        }
        else
        {
            _countPerFileName.Add(fileName, 1);
        }

        var content =
            GetContent(namespaceName, className, fileName, classToGenerate); // Generates the `ToString` method content.

        context.AddSource(hintName: fileName, source: content); // Adds the generated source to the compilation.
    }

    private static string GetContent(string namespaceName, string className, string fileName, ClassToGenerate classToGenerate)
    {
        var stringBuilder = new StringBuilder();

        // Builds the class definition and `ToString` method.
        stringBuilder.Append($@"// Generation count: {_countPerFileName[fileName]}
namespace {namespaceName};

public partial class {className}
{{
    public override string ToString()
    {{
        return $""");

        var first = true;

        // Iterates through all members of the class to find public properties.
        foreach (var propertyName in classToGenerate.PropertyNames)
        {
            if (!first)
            {
                stringBuilder.Append("; "); // Separates properties with semicolons.
            }
            first = false;

            stringBuilder.Append($"{propertyName}:{{{propertyName}}}"); // Formats property name and value.
        }

        stringBuilder.Append($@"""; }} // Closes the generated `ToString` method.
}}
");

        return stringBuilder.ToString(); // Returns the complete generated source code.
    }
}