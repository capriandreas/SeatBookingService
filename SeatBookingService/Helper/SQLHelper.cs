using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SeatBookingService.Helper
{
    public class SQLHelper : ISQLHelper
    {
        private string _serverUrl;
        public SQLHelper(string serverUrl)
        {
            this._serverUrl = serverUrl;
        }

        public async Task<List<T>> queryList<T>(string query, Dictionary<string, object> param) where T : new()
        {
            //string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
            MySqlConnection mycon = new MySqlConnection(_serverUrl);

            await mycon.OpenAsync();

            try
            {
                var result = new List<T>();
                await using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    if (param != null && param.Count > 0)
                        foreach (var key in param.Keys)
                            myCommand.Parameters.AddWithValue(key, param.GetValueOrDefault(key) == null ? DBNull.Value : param.GetValueOrDefault(key));

                    await using (var reader = await myCommand.ExecuteReaderAsync())
                        result = await convertDataReaderToObj<T>(reader);
                }

                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                await mycon.CloseAsync();
            }
        }

        async Task<List<T>> convertDataReaderToObj<T>(MySqlDataReader reader) where T : new()
        {
            Type myType = typeof(T);
            List<PropertyInfo> myField = new List<PropertyInfo>(myType.GetTypeInfo().DeclaredProperties);
            List<T> result = new List<T>();
            Dictionary<string, FieldMappingInfo> fieldMapping = new Dictionary<string, FieldMappingInfo>();

            var columns = reader.GetColumnSchema();
            foreach (var column in columns)
            {
                var field = myField.FirstOrDefault(e => e.Name.Equals(column.ColumnName));
                if (field != null)
                {
                    var fieldType = field.PropertyType;
                    Type typeName = null;
                    var mapInfo = new FieldMappingInfo(field, typeName);
                    //type.GetGenericTypeDefinition() == typeof(Nullable<>)

                    if (fieldType.IsGenericType && fieldType.Name.Contains("Nullable"))
                    {
                        var genericType = field.PropertyType.GenericTypeArguments[0];
                        mapInfo.type = genericType;
                        mapInfo.nullable = true;
                    }
                    else
                    {
                        mapInfo.type = fieldType;
                    }
                    fieldMapping.Add(field.Name, mapInfo);
                }
            }

            while (await reader.ReadAsync())
            {
                T newobject = new T();
                foreach (var column in fieldMapping.Keys)
                {
                    if (reader.IsDBNull(column))
                    {
                        if (fieldMapping[column].nullable)
                            fieldMapping[column].propInfo.SetValue(newobject, null);
                    }
                    else
                    {
                        try
                        {
                            if (fieldMapping[column].type == typeof(string))
                            {
                                var value = reader.GetString(column);
                                fieldMapping[column].propInfo.SetValue(newobject, value);
                            }
                            else if (fieldMapping[column].type == typeof(long))
                            {
                                var value = reader.GetInt64(column);
                                if (fieldMapping[column].nullable)
                                    fieldMapping[column].propInfo.SetValue(newobject, (long?)value);
                                else
                                    fieldMapping[column].propInfo.SetValue(newobject, value);
                            }
                            else if (fieldMapping[column].type == typeof(int))
                            {
                                var value = reader.GetInt32(column);
                                if (fieldMapping[column].nullable)
                                    fieldMapping[column].propInfo.SetValue(newobject, (int?)value);
                                else
                                    fieldMapping[column].propInfo.SetValue(newobject, value);
                            }
                            else if (fieldMapping[column].type == typeof(bool))
                            {
                                var value = reader.GetBoolean(column);
                                if (fieldMapping[column].nullable)
                                    fieldMapping[column].propInfo.SetValue(newobject, (bool?)value);
                                else
                                    fieldMapping[column].propInfo.SetValue(newobject, value);
                            }
                            else if (fieldMapping[column].type == typeof(double))
                            {
                                var value = reader.GetDouble(column);
                                if (fieldMapping[column].nullable)
                                    fieldMapping[column].propInfo.SetValue(newobject, (double?)value);
                                else
                                    fieldMapping[column].propInfo.SetValue(newobject, value);
                            }
                            else if (fieldMapping[column].type == typeof(decimal))
                            {
                                var value = reader.GetDecimal(column);
                                if (fieldMapping[column].nullable)
                                    fieldMapping[column].propInfo.SetValue(newobject, (decimal?)value);
                                else
                                    fieldMapping[column].propInfo.SetValue(newobject, value);
                            }

                            else if (fieldMapping[column].type == typeof(string[]))
                            {
                                var value = reader.GetFieldValue<string[]>(column);
                                if (fieldMapping[column].nullable)
                                    fieldMapping[column].propInfo.SetValue(newobject, new string[0]);
                                else
                                    fieldMapping[column].propInfo.SetValue(newobject, value);
                            }
                            else if (fieldMapping[column].type == typeof(DateTime))
                            {
                                //var value = reader.GetTimeStamp(reader.GetOrdinal(column));
                                var value = reader.GetDateTime(reader.GetOrdinal(column));
                                if (fieldMapping[column].nullable)
                                    fieldMapping[column].propInfo.SetValue(newobject, (DateTime?)value);
                                else
                                    fieldMapping[column].propInfo.SetValue(newobject, value);
                            }
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine("Error when convert column " + column);
                            throw;
                        }

                    }

                }
                result.Add(newobject);
            }

            return result;
        }
    }

    public class FieldMappingInfo
    {
        public PropertyInfo propInfo { get; set; }
        public Type type { get; set; }
        public bool nullable { get; set; }

        public FieldMappingInfo(PropertyInfo propInfo, Type type)
        {
            this.propInfo = propInfo;
            this.type = type;
        }
    }
}
