using Demo.Generators.CustomGenerators.SemanticModel;

namespace Demo.ConsoleApp.Model;

[GenerateToStringV2Attribute]
public partial class Person
{
    public string? FirstName { get; set; }
    internal string? MiddleName { get; set; }
}

public partial class Person
{
    public string? LastName { get; set; }
}

public partial class Person
{
    public int Age { get; set; }
}

//[GenerateToStringAttribute]
//public partial class Person
//{
//    public string? FirstName { get; set; }
//    internal string? MiddleName { get; set; }
//    public string? LastName { get; set; }

//    // Implemented via soure generator
//    //public override string ToString()
//    //{
//    //    return $"FirstName:{FirstName}; LastName:{LastName}";
//    //}

//    //public override string ToString()
//    //{
//    //     // // Using Reflection

//    //    var stringBuilder = new StringBuilder();

//    //    var isFirstRecord = true;

//    //    foreach (var propertyInfo in GetType().GetProperties())
//    //    {
//    //        if (isFirstRecord)
//    //        {
//    //            isFirstRecord = false;
//    //        }
//    //        else
//    //        {
//    //            stringBuilder.Append("; ");
//    //        }

//    //        var propertyName = propertyInfo.Name;
//    //        var propertyValue = propertyInfo.GetValue(this);
//    //        stringBuilder.Append($"{propertyName}:{propertyValue}");            
//    //    }

//    //    return stringBuilder.ToString();
//    //}
//}