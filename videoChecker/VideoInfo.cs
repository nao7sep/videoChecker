using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace videoChecker
{
    public class VideoInfo
    {
        public static JsonSerializerOptions DefaultJsonSerializerOptions { get; } = new ()
        {
            Converters =
            {
                new FileInfoToJsonConverter ()
            },

            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
        };

        public static int FileStreamBufferSize { get; } = 4 * 1024 * 1024;

        public static bool TryLoad (string filePath, out VideoInfo? videoInfo)
        {
            try
            {
                VideoInfo xVideoInfo = new ();
                xVideoInfo.FileInfo = new (filePath);
                xVideoInfo.CheckedAtUtc = DateTime.UtcNow;
                xVideoInfo.Metadata = MetadataExtractor.ImageMetadataReader.ReadMetadata (filePath);

                // CRC32

                // Uses .NET's built-in CRC32 class that: Provides an implementation of the CRC-32 algorithm, as used in ITU-T V.42 and IEEE 802.3.
                // https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.crc32
                // WinRAR and 7-Zip generate exactly the same CRC32 hashes as this implementation.

                Crc32 xCrc32 = new ();
                using FileStream xFileStream = File.OpenRead (filePath);
                byte [] xBuffer = new byte [FileStreamBufferSize];
                int xReadByteCount;

                while ((xReadByteCount = xFileStream.Read (xBuffer, 0, FileStreamBufferSize)) > 0)
                    xCrc32.Append (xBuffer.AsSpan (0, xReadByteCount));

                xVideoInfo.Crc32Hash = xCrc32.GetCurrentHashAsUInt32 ();

                // MD5

                // Processed in chunks.
                // https://source.dot.net/#System.Security.Cryptography/System/Security/Cryptography/MD5.cs
                // https://source.dot.net/#System.Security.Cryptography/System/Security/Cryptography/LiteHashProvider.cs
                // Tested with: https://emn178.github.io/online-tools/md5_checksum.html

                using MD5 xMd5 = MD5.Create ();
                xFileStream.Position = 0;
                xVideoInfo.Md5Hash = xMd5.ComputeHash (xFileStream);

                videoInfo = xVideoInfo;
                return true;
            }

            catch
            {
                videoInfo = null;
                return false;
            }
        }

        public FileInfo? FileInfo { get; set; }

        public DateTime? CheckedAtUtc { get; set; }

        [JsonConverter (typeof (UintToJsonConverter))]
        public uint? Crc32Hash { get; set; }

        [JsonConverter (typeof (ByteArrayToJsonConverter))]
        public byte []? Md5Hash { get; set; }

        public IReadOnlyList <MetadataExtractor.Directory>? Metadata { get; set; }
    }
}
