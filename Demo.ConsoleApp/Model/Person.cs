﻿namespace Demo.ConsoleApp.Model;

public partial class Person
{
    public string? FirstName { get; set; }
    internal string? MiddleName { get; set; }
    public string? LastName { get; set; }

    // Implemented via soure generator
    //public override string ToString()
    //{
    //    return $"FirstName:{FirstName}; LastName:{LastName}";
    //}

    //public override string ToString()
    //{
    //     // // Using Reflection

    //    var stringBuilder = new StringBuilder();

    //    var isFirstRecord = true;

    //    foreach (var propertyInfo in GetType().GetProperties())
    //    {
    //        if (isFirstRecord)
    //        {
    //            isFirstRecord = false;
    //        }
    //        else
    //        {
    //            stringBuilder.Append("; ");
    //        }

    //        var propertyName = propertyInfo.Name;
    //        var propertyValue = propertyInfo.GetValue(this);
    //        stringBuilder.Append($"{propertyName}:{propertyValue}");            
    //    }

    //    return stringBuilder.ToString();
    //}
}