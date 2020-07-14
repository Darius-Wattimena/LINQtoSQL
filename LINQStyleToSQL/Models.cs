using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LINQStyleToSQL
{

    public class QueryableCollection<T> : ICollection<T>
    {
        public ICollection<T> Data { get; set; }
        public QueryableCollection<T> Source { get; set; }

        public QueryBuilder Builder { get; set; }

        public int Count => Data.Count;

        public bool IsReadOnly => Data.IsReadOnly;

        public QueryableCollection(ICollection<T> data)
        {
            Data = data;
            Builder = new QueryBuilder();
        }
        public QueryableCollection(QueryableCollection<T> instance, SQLCommand command)
        {
            Data = instance.Data;
            Builder = instance.Builder;
            Builder.AddCommand(command);
            Source = instance;
        }

        public void Add(T item)
        {
            Data.Add(item);
        }

        public void Clear()
        {
            Data.Clear();
        }

        public bool Contains(T item)
        {
            return Data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return Data.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }

    public class QueryBuilder
    {
        public static List<SQLCommand> QueuedCommands { get; set; }

        public QueryBuilder()
        {
            QueuedCommands = new List<SQLCommand>();
        }

        public void AddCommand(SQLCommand command)
        {
            QueuedCommands.Add(command);
        }

        public QueryableCollection<T> Build<T>() where T: class, new()
        {
            var query = "";

            foreach (var command in QueuedCommands)
            {
                var exp = ((LambdaExpression)command.Expression);
                var part = exp.Body.ToString();
                var first = true;

                Console.WriteLine(exp);
                Console.WriteLine(part);
                switch (command.Type)
                {
                    case SqlCommandType.SELECT:
                        foreach (var parameter in exp.Parameters)
                        {
                            if (part.ToUpper() == typeof(T).Name.ToUpper())
                            {
                                query += "SELECT *";
                            }
                            else
                            {
                                if (part.EndsWith(")"))
                                {
                                    part = part.Substring(part.IndexOf("(") + 1);
                                    part = part.Substring(0, part.IndexOf(")"));
                                    part = part.Replace(parameter.Name + ".", parameter.Type.Name + ".")
                                        .Replace("AndAlso", "&&");
                                    var splitParts = part.Split(",");

                                    foreach (var splitPart in splitParts)
                                    {
                                        var parsedStringPart = splitPart.Substring(0, splitPart.IndexOf(" =")).ToUpper();

                                        Console.WriteLine(splitPart);
                                        Console.WriteLine(parsedStringPart);

                                        if (first)
                                        {
                                            query += "SELECT " + parsedStringPart;
                                            first = false;
                                        }
                                        else
                                        {
                                            query += "," + parsedStringPart;
                                        }
                                    }
                                }
                                else
                                {
                                    part = part.Replace(parameter.Name + ".", parameter.Type.Name + ".");

                                    var parsedStringPart = part.Split(".")[1].ToUpper();

                                    Console.WriteLine(parsedStringPart);

                                    if (first)
                                    {
                                        query += "SELECT " + parsedStringPart;
                                        first = false;
                                    }
                                    else
                                    {
                                        query += "," + parsedStringPart;
                                    }
                                }
                            }

                            query += " ";
                        }

                        break;
                    case SqlCommandType.WHERE:
                        foreach (var parameter in exp.Parameters)
                        {
                            part = part.Replace(parameter.Name + ".", parameter.Type.Name + ".")
                                .Replace("(", "")
                                .Replace(")", "")
                                .Replace("AndAlso", "&&");

                            var splitParts = part.Split("&& ");

                            foreach (var splitPart in splitParts)
                            {
                                var parsedStringPart = splitPart.Replace(" == ", "=")
                                    .Replace(parameter.Type.Name + ".", "").ToUpper().Replace("\"", "'");

                                Console.WriteLine(splitPart);
                                Console.WriteLine(parsedStringPart);

                                if (first)
                                {
                                    query += "WHERE " + parsedStringPart;
                                    first = false;
                                }
                                else
                                {
                                    query += "&& " + parsedStringPart;
                                }
                            }
                        }
                        break;
                    case SqlCommandType.FROM:
                        var name = typeof(T).Name;

                        query += "FROM " + "dbo." + name + " ";
                        break;
                }
            }

            //TODO execute query
            Console.WriteLine(query);
            var queryResult = new List<T>();
            var customReader = new SQLToObjectMapper();
            var x = customReader.Query<T>(query);
            return new QueryableCollection<T>(x);
        }
    }

    public class SQLCommand
    {
        public Expression Expression { get; set; }
        public SqlCommandType Type { get; set; }
    }
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public enum SqlCommandType
    {
        WHERE,
        SELECT,
        FROM
    }

    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Group[] Groups { get; set; }
    }

    public class Group
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
