using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Generators.CustomGenerators.SymbolApi;

internal class ClassToGenerate : IEquatable<ClassToGenerate?>
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

    public override bool Equals(object? obj)
    {
        return Equals(obj as ClassToGenerate);
    }

    public bool Equals(ClassToGenerate? other)
    {
        return other is not null &&
               NamespaceName == other.NamespaceName &&
               ClassName == other.ClassName &&
               PropertyNames.SequenceEqual(other.PropertyNames);
    }

    public override int GetHashCode()
    {
        int hashCode = -39551721;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NamespaceName);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClassName);
        return hashCode;
    }
}
