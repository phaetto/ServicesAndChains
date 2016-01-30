namespace Chains.Play
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [Serializable]
    public abstract class SerializableSpecification
    {
        private class FieldsAndPropertiesForClass
        {
            public IEnumerable<FieldInfo> fields;
            public IEnumerable<PropertyInfo> properties;
        }

        private static Dictionary<string, FieldsAndPropertiesForClass> reflectionCaching =
            new Dictionary<string, FieldsAndPropertiesForClass>();

        public abstract int DataStructureVersionNumber { get; }

        private void PopulateFieldsAndProperties()
        {
            var entry = new FieldsAndPropertiesForClass
                        {
                            properties =
                                GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(x => x.GetSetMethod(true) != null)
                                .Where(x => x.GetIndexParameters().Length == 0)
                                .Where(x => x.PropertyType.IsSerializable)
                                .ToList(),
                            fields =
                                GetType()
                                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                                .Where(x => x.FieldType.IsSerializable)
                                .ToList()
                        };

            lock (reflectionCaching)
            {
                reflectionCaching[GetType().FullName] = entry;
            }
        }

        private IEnumerable<PropertyInfo> Properties
        {
            get
            {
                var thisFullTypeName = GetType().FullName;

                if (!reflectionCaching.ContainsKey(thisFullTypeName))
                {
                    PopulateFieldsAndProperties();
                }

                return reflectionCaching[thisFullTypeName].properties;
            }
        }

        private IEnumerable<FieldInfo> Fields
        {
            get
            {
                var thisFullTypeName = GetType().FullName;

                if (!reflectionCaching.ContainsKey(thisFullTypeName))
                {
                    PopulateFieldsAndProperties();
                }

                return reflectionCaching[thisFullTypeName].fields;
            }
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(
                this,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public string SerializeToJsonForCommandPrompt()
        {
            return SerializeToJson().Replace('\"', '\'');
        }

        public static string SerializeManyToJson(SerializableSpecification[] specs)
        {
            return JsonConvert.SerializeObject(
                specs,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static string SerializeManyToJsonForCommandPrompt(SerializableSpecification[] specs)
        {
            return SerializeManyToJson(specs).Replace('\"', '\'');
        }

        public string SerializeToBase64String()
        {
            return Convert.ToBase64String(SerializeWithBinaryFormatter());
        }

        public static string SerializeManyToBase64String(SerializableSpecification[] specs)
        {
            return Convert.ToBase64String(SerializeManyWithBinaryFormatter(specs));
        }

        public byte[] SerializeWithBinaryFormatter()
        {
            var dict = GetAsDictionary();
            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, dict);

            return memoryStream.ToArray();
        }

        public static byte[] SerializeManyWithBinaryFormatter(SerializableSpecification[] specs)
        {
            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, specs.Select(x => x.GetAsDictionary()).ToArray());

            return memoryStream.ToArray();
        }

        public void LoadValues(IDictionary<string, object> values)
        {
            if (values.ContainsKey("DataStructureVersionNumber"))
            {
                var dataVersion = (int)Convert.ChangeType(values["DataStructureVersionNumber"], typeof(int));
                if (dataVersion != DataStructureVersionNumber)
                {
                    ResolveVersionConflict(values, dataVersion);
                }
            }

            foreach (var field in Fields)
            {
                if (!values.ContainsKey(field.Name))
                {
                    continue;
                }

                var data = values[field.Name];
                if (Equals(data, null))
                {
                    continue;
                }

                if (data.GetType() == typeof(JArray))
                {
                    var jarray = data as JArray;
                    data = NormalizeArrayTypes(field.FieldType, data, jarray);
                }

                if (data.GetType() == typeof(JObject) && GetType() == typeof(ExecutableActionSpecification))
                {
                    var jcontainer = data as JObject;
                    var executableActionSpecification = this as ExecutableActionSpecification;
                    data = jcontainer.ToObject(ExecutionChain.FindType(executableActionSpecification.DataType));
                }

                try
                {
                    field.SetValue(this, data);
                }
                catch (ArgumentException)
                {
                    if (field.FieldType == typeof(Guid))
                    {
                        field.SetValue(this, Guid.Parse(data as string));
                    }
                    else
                    {
                        field.SetValue(this, Convert.ChangeType(data, field.FieldType));
                    }
                }
            }

            foreach (var property in Properties)
            {
                if (!values.ContainsKey(property.Name))
                {
                    continue;
                }

                var data = values[property.Name];
                if (Equals(data, null))
                {
                    continue;
                }

                if (data.GetType() == typeof(JArray))
                {
                    var jarray = data as JArray;
                    data = NormalizeArrayTypes(property.PropertyType, data, jarray);
                }

                if (data.GetType() == typeof(JObject) && GetType() == typeof(ExecutableActionSpecification))
                {
                    var jcontainer = data as JObject;
                    var executableActionSpecification = this as ExecutableActionSpecification;
                    data = jcontainer.ToObject(ExecutionChain.FindType(executableActionSpecification.DataType));
                }

                try
                {
                    property.SetValue(this, data, null);
                }
                catch (ArgumentException)
                {
                    if (property.PropertyType == typeof(Guid))
                    {
                        property.SetValue(this, Guid.Parse(data as string), null);
                    }
                    else
                    {
                        property.SetValue(this, Convert.ChangeType(data, property.PropertyType), null);
                    }
                }
            }
        }

        private static object NormalizeArrayTypes(Type type, object data, JArray jarray)
        {
            var elementType = type.GetElementType();

            if (elementType == null)
            {
                return jarray.ToObject(type);
            }

            switch (elementType.FullName)
            {
                case "System.String":
                    data = jarray.ToArray().Select(x => x.ToObject<string>()).ToArray();
                    break;
                case "System.Int32":
                    data = jarray.ToArray().Select(x => x.ToObject<int>()).ToArray();
                    break;
                case "System.Boolean":
                    data = jarray.ToArray().Select(x => x.ToObject<bool>()).ToArray();
                    break;
                case "System.Object":
                    data = jarray.ToArray().Select(x => x.ToObject(elementType)).ToArray();
                    break;
                default:
                    data = jarray.ToObject(type);
                    break;
            }

            return data;
        }

        public ExecutableActionSpecification AsSpecification(string session = null, string apiKey = null)
        {
            return new ExecutableActionSpecification
                   {
                       Data = this,
                       DataType = GetType().FullName,
                       Session = session,
                       ApiKey = apiKey
                   };
        }

        private Dictionary<string, object> GetAsDictionary()
        {
            var propDict = Properties.ToDictionary(x => x.Name, x => x.GetValue(this, null));
            var fieldDict = Fields.ToDictionary(x => x.Name, x => x.GetValue(this));
            var dict = propDict.Concat(fieldDict).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return dict;
        }

        protected virtual void ResolveVersionConflict(IDictionary<string, object> values, int dataVersion)
        {
        }
    }
}
