using Demo.ConsoleApp.Model;

var person = new Person
{
    FirstName = "Tim",
    MiddleName = "Hello",
    LastName = "Cook"
};

var address = new Address
{
    City = "New York",
    State = "US"
};

var personString = person.ToString();
var addressString = address.ToString();

Console.WriteLine(personString);
Console.WriteLine(addressString);

Console.ReadLine();
