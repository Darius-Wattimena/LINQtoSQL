using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace LINQStyleToSQL
{
    public class SQLToObjectMapper
    {
        public SQLToObjectMapper()
        {
        }        

        public ICollection<T> Query<T>(string query) where T : class, new()
        {
            var connectionString = GetConnectionString();

            var TCollection = new List<T>();

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TCollection.Add(Single<T>(reader));
                    }
                    reader.NextResult();
                }
                connection.Close();
            }
            return TCollection;
        }

        private T Single<T>(IDataRecord record) where T : class, new()
        {
            if (record == null)
            {
                throw new ArgumentNullException("record is null");
            }

            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (properties.Length < 1)
            {
                throw new ArgumentException("reflection failed, T has no readable public properties");
            }

            T newT = new T();

            foreach (var property in properties)
            {
                if (property != null & property.CanWrite)
                {
                    property.SetValue(newT, record[property.Name]);
                }
            }
            return newT;
        }

        private string GetQuery()
        {
            return @"SELECT FirstName, LastName FROM dbo.Student;";
        }

        private string GetConnectionString()
        {
            return ConnectionString.Path;
        }
    }
}
