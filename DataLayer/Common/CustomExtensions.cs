using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace DataLayer.Common
{
    public static class CustomExtension 
    {
        
        public static List<T> ToListObject_ENCYPT<T>(this DataTable table) where T : class, new()
        {
            var util = new EncryptDecrypt_New();
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            if (table.Columns.Contains(prop.Name))
                            {
                                if (row[prop.Name] != DBNull.Value)
                                {
                                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);

                                    propertyInfo.SetValue(obj, ChangeType(   util.Encrypt( row[prop.Name].ToString(), true) , propertyInfo.PropertyType), null);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message, ex.InnerException);
                            //continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }


        public static List<T> ToListObject<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            if (table.Columns.Contains(prop.Name))
                            {
                                if (row[prop.Name] != DBNull.Value)
                                {
                                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);

                                    propertyInfo.SetValue(obj, ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message, ex.InnerException);
                            //continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static List<T> ToListObjectNew<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            if (table.Columns.Contains(prop.Name))
                            {
                                if (row[prop.Name] != DBNull.Value)
                                {
                                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);

                                    if (prop.Name == "FromTime" || prop.Name == "ToTime")
                                    {
                                        propertyInfo.SetValue(obj, ChangeType(Convert.ToDateTime(row[prop.Name].ToString()), propertyInfo.PropertyType), null);
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(obj, ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message, ex.InnerException);
                            //continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T ToModelObject<T>(this DataTable table) where T : class, new()
        {
            try
            {
                T list = new T();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            if (table.Columns.Contains(prop.Name))
                            {
                                if (row[prop.Name] != DBNull.Value)
                                {
                                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                                    //propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                    propertyInfo.SetValue(obj, ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                                }
                            }
                        }
                        catch
                        {
                            //throw new ApplicationException(ex.Message, ex.InnerException);
                            continue;
                        }
                    }

                    list = obj;
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }

            if (conversionType.IsGenericType &&
              conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }


        public static string ListToXml<T>(this IList<T> data, string rootname) where T : class, new()
        {
            XElement xmlelements = new XElement(
                  rootname,
                  data.Select(i=> new XElement("object", i))
                );
            return xmlelements.ToString();
        }

    }
}
