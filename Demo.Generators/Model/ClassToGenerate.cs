using System.Collections.Generic;

namespace Demo.Generators.Model;

internal class ClassToGenerate
{
    public ClassToGenerate(
        string namespaceName,
        string className,
        IEnumerable<string> propertyNames)
    {
        NamespaceName = namespaceName;
        ClassName = className;
        PropertyNames = propertyNames;
    }

    public string NamespaceName { get; }
    public string ClassName { get; }
    public IEnumerable<string> PropertyNames { get; }
}
