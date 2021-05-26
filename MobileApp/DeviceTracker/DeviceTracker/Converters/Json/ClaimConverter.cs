using System;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceTracker.Converters.Json
{
    public class ClaimConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Claim));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string type = (string)jo[nameof(Claim.Type)];
            string value = (string)jo[nameof(Claim.Value)];
            string valueType = (string)jo[nameof(Claim.ValueType)];
            string issuer = (string)jo[nameof(Claim.Issuer)];
            string originalIssuer = (string)jo[nameof(Claim.OriginalIssuer)];
            return new Claim(type, value, valueType, issuer, originalIssuer);
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var claim = (Claim)value;
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Claim.Type));
            writer.WriteValue(claim.Type);
            writer.WritePropertyName(nameof(Claim.Value));
            writer.WriteValue(claim.Value);
            writer.WritePropertyName(nameof(Claim.ValueType));
            writer.WriteValue(claim.ValueType);
            writer.WritePropertyName(nameof(Claim.Issuer));
            writer.WriteValue(claim.Issuer);
            writer.WritePropertyName(nameof(Claim.OriginalIssuer));
            writer.WriteValue(claim.OriginalIssuer);
            writer.WriteEndObject();
        }
    }
}
