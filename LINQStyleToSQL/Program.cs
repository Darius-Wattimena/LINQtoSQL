using System;

namespace LINQStyleToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Group[] groups = { group1, group2 };
            var customReader = new SQLToObjectMapper();
            var queryResult = customReader.Query<Student>(@"SELECT FirstName, LastName FROM dbo.Student");
            var results = queryResult.Select(student => new {student.FirstName, student.LastName})
                .Where(student => student.FirstName == "Darius");

            //var results2 = groups.Select(group => group.Name)
            //    .Where(group => group.Name == "Test 3");

            //foreach (var @group in results2)
            //{
            //    Console.WriteLine(new {@group});
            //}

            foreach (var result in results)
            {
                Console.WriteLine(new { result });
            }
        }
    }
}
