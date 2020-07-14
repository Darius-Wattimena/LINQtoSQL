using System;

namespace LINQStyleToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var group1 = new Group
            {
                Name = "Group 1",
                Value = 10
            };

            var group2 = new Group
            {
                Name = "Group 2",
                Value = 20
            };

            User[] users = { new User
            {
                Name = "Test",
                Age = 20,
                Groups = new[] {group1, group2}
            }, new User
            {
                Name = "Test 2",
                Age = 10,
                Groups = new[] { group2 }
            }, new User
            {
                Name = "Test 3",
                Age = 50,
                Groups = new[] { group1 }
            }};

            var results = users.Select(user => user)
                .Where(user => user.Name == "Test" && user.Age == 10);
                //.Include(x => x.Groups, (groups => groups.Select(group => group.Name)));

            foreach (var result in results)
            {
                foreach (var group in result.Groups)
                {
                    Console.WriteLine(new {result, group.Name, group.Value});
                }
            }
        }
    }
}
