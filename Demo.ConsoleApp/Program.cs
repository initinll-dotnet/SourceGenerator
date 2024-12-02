using Demo.ConsoleApp.Model;

var person = new Person
{
    FirstName = "Tim",
    //MiddleName = "Hello",
    LastName = "Cook"
};

var personString = person.ToString();

Console.WriteLine(personString);

Console.ReadLine();
