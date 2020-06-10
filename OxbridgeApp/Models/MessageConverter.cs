using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OxbridgeApp.Models
{
    public class MessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(IMessage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return serializer.Deserialize(reader, typeof(object));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            serializer.Serialize(writer, value);
        }
    }
}
