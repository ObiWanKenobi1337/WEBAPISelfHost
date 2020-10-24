using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SelfHostApi.Models;

namespace SelfHostApi
{
    public class CustomJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BaseAbstractModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);

            try
            {
                switch ((ETypes) Enum.Parse(typeof(ETypes), jObj["Type"].Value<string>(), true))
                {
                    case ETypes.ConnectionSettings:
                        return jObj.ToObject<ConnectionSettings>(serializer);
                    case ETypes.DeviceSettings:
                        return jObj.ToObject<DeviceSettings>(serializer);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                //log exception
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}