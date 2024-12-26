using System.Text.Json;
using System.Text.Json.Serialization;

namespace _videoChecker
{
    public class UintToJsonConverter: JsonConverter <uint>
    {
        public override uint Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException ();

        public override void Write (Utf8JsonWriter writer, uint value, JsonSerializerOptions options) => writer.WriteStringValue (value.ToString ("X8"));
    }
}
