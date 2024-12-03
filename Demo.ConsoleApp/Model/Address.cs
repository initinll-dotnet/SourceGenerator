using Demo.Generators.CustomGenerators.SyntaxApi;

namespace Demo.ConsoleApp.Model;

[GenerateToStringV1Attribute]
public partial class Address
{
    public string? City { get; set; }
    public string? State { get; set; }
}
