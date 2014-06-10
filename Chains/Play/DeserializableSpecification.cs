namespace Chains.Play
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using Newtonsoft.Json;

    public abstract class DeserializableSpecification<T> : SerializableSpecification
        where T : SerializableSpecification, new()
    {
        public static T DeserializeFromJson(string data)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            return MapObject(dict);
        }

        public static T[] DeserializeManyFromJson(string data)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(data);
            return dict.Select(MapObject).ToArray();
        }

        public static T DeserializeFromBase64String(string data)
        {
            return DeserializeWithBinaryFormatter(Convert.FromBase64String(data));
        }

        public static T[] DeserializeManyFromBase64String(string data)
        {
            return DeserializeManyWithBinaryFormatter(Convert.FromBase64String(data));
        }

        public static T DeserializeWithBinaryFormatter(byte[] data)
        {
            var binaryFormatter = new BinaryFormatter();

            var dict = (IDictionary<string, object>)binaryFormatter.Deserialize(new MemoryStream(data));

            return MapObject(dict);
        }

        public static T[] DeserializeManyWithBinaryFormatter(byte[] data)
        {
            var binaryFormatter = new BinaryFormatter();

            var dict = (IDictionary<string, object>[])binaryFormatter.Deserialize(new MemoryStream(data));

            return dict.Select(MapObject).ToArray();
        }

        private static T MapObject(IDictionary<string, object> dict)
        {
            var model = new T();
            model.LoadValues(dict);
            return model;
        }
    }
}