using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace _videoChecker
{
    public class VideoInfo
    {
        public static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new ()
        {
            Converters =
            {
                new FileInfoConverter ()
            },

            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
        };

        public FileInfo? FileInfo { get; set; }

        public IReadOnlyList <MetadataExtractor.Directory>? Metadata { get; set; }
    }
}
