using MetadataExtractor;

namespace _videoChecker
{
    public class Utility
    {
        public static DateTime? ExtractCreatedOrModifiedTime (IReadOnlyList <MetadataExtractor.Directory> directories)
        {
            var xDirectory = directories.FirstOrDefault (x => x.Name.Equals ("QuickTime Movie Header", StringComparison.OrdinalIgnoreCase));

            if (xDirectory != null)
            {
                DateTime? _ExtractTime (MetadataExtractor.Directory directory, string tagName)
                {
                    var xTag = directory.Tags.FirstOrDefault (x => x.Name.Equals (tagName, StringComparison.OrdinalIgnoreCase));

                    if (xTag != null)
                    {
                        // Based on DirectoryExtensions.GetString.
                        // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/main/MetadataExtractor/DirectoryExtensions.cs

                        var xObject = directory.GetObject (xTag.Type);

                        if (xObject is DateTime xTime)
                            return xTime;

                        // Based on TagDescriptor.GetEpochTimeDescription.
                        // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/main/MetadataExtractor/TagDescriptor.cs

                        if (directory.TryGetInt64 (xTag.Type, out long xValue))
                            return DateTime.UnixEpoch.AddSeconds (xValue);
                    }

                    return null;
                }

                return _ExtractTime (xDirectory, "Created") ?? _ExtractTime (xDirectory, "Modified");
            }

            return null;
        }
    }
}
