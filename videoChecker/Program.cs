using System.Globalization;
using System.Text;

namespace _videoChecker
{
    class Program
    {
        static void Main (string [] args)
        {
            try
            {
                // Required for consistent serialization.
                // MetadataExtractor relies on the current culture.
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                if (args.Length == 0)
                {
                    Console.WriteLine ("Usage: _videoChecker.exe <file1> <file2> ...");
                    return;
                }

                List <(string FilePath, VideoInfo? VideoInfo, DateTime? Time, string? ErrorMessage)> xFiles = [];

                Console.WriteLine ("Loading files...");

                foreach (string xFilePath in args)
                {
                    if (File.Exists (xFilePath) == false)
                    {
                        xFiles.Add ((xFilePath, null, null, "File not found."));
                        continue;
                    }

                    if (Path.GetFileNameWithoutExtension (xFilePath).EndsWith ("-OK", StringComparison.OrdinalIgnoreCase))
                    {
                        xFiles.Add ((xFilePath, null, null, "Already OK."));
                        continue;
                    }

                    if (VideoInfo.TryLoad (xFilePath, out VideoInfo? xVideoInfo) == false)
                    {
                        xFiles.Add ((xFilePath, null, null, "Failed to load."));
                        continue;
                    }

                    var xTime = Utility.ExtractCreatedOrModifiedTime (xVideoInfo!.Metadata!);

                    if (xTime.HasValue == false || xTime.Value < DateTime.UnixEpoch)
                    {
                        xFiles.Add ((xFilePath, xVideoInfo, xTime, "Failed to extract time."));
                        continue;
                    }

                    xFiles.Add ((xFilePath, xVideoInfo, xTime, null));
                }

                var xSortedFiles = xFiles.OrderBy (x => x.FilePath, StringComparer.OrdinalIgnoreCase).ToArray ();

                foreach (var xFile in xSortedFiles)
                {
                    Console.WriteLine ($"{(xFile.VideoInfo != null ? "Loaded" : "NOT loaded")}: {xFile.FilePath}:");

                    if (xFile.ErrorMessage != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine ($"    {xFile.ErrorMessage}");
                        Console.ResetColor ();
                    }

                    else Console.WriteLine ($"    Time: {xFile.Time!.Value:yyyy'-'MM'-'dd' 'HH':'mm':'ss}");
                }

                if (xSortedFiles.Count (x => x.VideoInfo != null) == 0)
                {
                    Console.WriteLine ("No files to process.");
                    return;
                }

                while (true)
                {
                    Console.Write ("Type OK to rename the loaded files and save their metadata or just close this window: "); // Good enough for personal use.
                    string? xInputString = Console.ReadLine ();

                    if (string.Equals (xInputString, "OK", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var xFile in xSortedFiles.Where (x => x.VideoInfo != null))
                        {
                            try
                            {
                                string xNewFilePath = Path.Join (Path.GetDirectoryName (xFile.FilePath)!, Path.GetFileNameWithoutExtension (xFile.FilePath) + "-OK" + Path.GetExtension (xFile.FilePath));
                                var xFileInfo = xFile.VideoInfo!.FileInfo!;
                                xFileInfo.Attributes = FileAttributes.Normal; // Just to be sure.
                                xFileInfo.MoveTo (xNewFilePath);
                                Console.WriteLine ($"  File renamed to: {xNewFilePath}"); // Looks better in the console.

                                string xJsonFilePath = Path.ChangeExtension (xNewFilePath, ".json");
                                File.WriteAllText (xJsonFilePath, Utility.Serialize (xFile.VideoInfo!), Encoding.UTF8);
                                Console.WriteLine ($"Metadata saved to: {xJsonFilePath}");
                            }

                            catch (Exception xException)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine ($"Failed to handle: {xFile.FilePath}");
                                Console.WriteLine ($"Exception: {xException}");
                                Console.ResetColor ();
                            }
                        }

                        break;
                    }
                }
            }

            catch (Exception xException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine (xException.ToString ());
                Console.ResetColor ();
            }

            finally
            {
                Console.Write ("Press any key to exit: ");
                Console.ReadKey (true);
                Console.WriteLine ();
            }
        }
    }
}
