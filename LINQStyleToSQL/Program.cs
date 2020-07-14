using System;

namespace LINQStyleToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //var group1 = new Group
            //{
            //    Name = "Group 1",
            //    Value = 10
            //};

            //var group2 = new Group
            //{
            //    Name = "Group 2",
            //    Value = 20
            //};

            //User[] users = { new User
            //{
            //    Name = "Test",
            //    Age = 20,
            //    Groups = new[] {group1, group2}
            //}, new User
            //{
            //    Name = "Test 2",
            //    Age = 10,
            //    Groups = new[] { group2 }
            //}, new User
            //{
            //    Name = "Test 3",
            //    Age = 50,
            //    Groups = new[] { group1 }
            //}};

            //Group[] groups = { group1, group2 };
            var customReader = new SQLToObjectMapper();
            var queryResult = customReader.Query<Student>(@"SELECT FirstName, LastName FROM dbo.Student");
            var results = queryResult.Select(student => student)
                .Where(student => student.FirstName == "Darius" || student.FirstName == "Samuel");

            //var results2 = groups.Select(group => group.Name)
            //    .Where(group => group.Name == "Test 3");

            //foreach (var @group in results2)
            //{
            //    Console.WriteLine(new {@group});
            //}

            //foreach (var result in results)
            //{
            //    foreach (var group in result.Groups)
            //    {
            //        Console.WriteLine(new {result, group.Name, group.Value});
            //    }
            //}
        }
    }
}
