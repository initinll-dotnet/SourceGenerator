namespace Demo.ConsoleApp.Model;

public class Person
{
    public string? FirstName { get; set; }
    //public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    public override string ToString()
    {
        return $"FirstName:{FirstName}; LastName:{LastName}";

        /*
         Using Reflection

        var stringBuilder = new StringBuilder();

        var isFirstRecord = true;

        foreach (var propertyInfo in GetType().GetProperties())
        {
            if (isFirstRecord)
            {
                isFirstRecord = false;
            }
            else
            {
                stringBuilder.Append("; ");
            }

            var propertyName = propertyInfo.Name;
            var propertyValue = propertyInfo.GetValue(this);
            stringBuilder.Append($"{propertyName}:{propertyValue}");            
        }

        return stringBuilder.ToString();
         
         */
    }
}

