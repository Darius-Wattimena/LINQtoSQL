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

            var results = users.Select(user => new {user.Name, user.Groups})
                //.Where(user => user.Name == "Test")
                .Include(x => x.Groups, (groups => groups.Select(group => group.Name)));

            foreach (var result in results)
            {
                foreach (var group in result.Item2)
                {
                    Console.WriteLine(new {result.Item1, group});
                }
            }
        }

        class User
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Group[] Groups { get; set; }
        }

        class Group
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
