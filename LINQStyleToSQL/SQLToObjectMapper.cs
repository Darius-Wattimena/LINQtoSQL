using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace LINQStyleToSQL
{
    public class SQLToObjectMapper
    {
        public SQLToObjectMapper()
        {
        }        

        public ICollection<T> Query<T>(string query) where T : class
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

        private T Single<T>(IDataRecord record) where T : class
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

            T newT = (T) FormatterServices.GetUninitializedObject(type);

            //T newT = new T();

            var resultT = new ExpandoObject() as IDictionary<string, Object>;
            

            foreach (var property in properties)
            {
                resultT.Add(property.Name, record[property.Name]);
            }

            return Convert((ExpandoObject) resultT, newT);

            //return (T) resultT;
        }

        public T Convert<T>(ExpandoObject source, T example)
            where T : class
        {
            IDictionary<string, object> dict = source;

            var ctor = example.GetType().GetConstructors().Single();

            var parameters = ctor.GetParameters();

            var parameterValues = new List<Object>();

            foreach (var parameter in parameters)
            {
                parameterValues.Add(dict[parameter.Name]);
            }

            return (T) ctor.Invoke(parameterValues.ToArray());
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
