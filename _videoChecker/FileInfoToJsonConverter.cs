using System.Text.Json;
using System.Text.Json.Serialization;

namespace _videoChecker
{
    public class FileInfoToJsonConverter: JsonConverter <FileInfo>
    {
        // Directory causes circular reference.
        // Directory paths may contain sensitive information.
        public static string [] IgnoredPropertyNames { get; } = ["Directory", "DirectoryName", "FullName"];

        public override FileInfo Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException ();

        public override void Write (Utf8JsonWriter writer, FileInfo value, JsonSerializerOptions options)
        {
            writer.WriteStartObject ();

            foreach (var xProperty in value.GetType ().GetProperties ())
            {
                if (IgnoredPropertyNames.Contains (xProperty.Name, StringComparer.OrdinalIgnoreCase) == false)
                {
                    var xValue = xProperty.GetValue (value);

                    // Should act like JsonIgnoreCondition.WhenWritingNull.
                    if (xValue != null)
                    {
                        writer.WritePropertyName (xProperty.Name);
                        JsonSerializer.Serialize (writer, xValue, xProperty.PropertyType);
                    }
                }
            }

            writer.WriteEndObject ();
        }
    }
}
