using System.Globalization;

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
            }

            catch (Exception xException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine (xException.ToString ());
                Console.ResetColor ();
            }

            Console.Write ("Press any key to exit: ");
            Console.ReadKey (true);
            Console.WriteLine ();
        }
    }
}
