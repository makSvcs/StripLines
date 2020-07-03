using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System;


namespace RedSequoia.StripToLines
{
    public enum MatchMode
    {
        Exact,
        //Regex
    }

    public class Settings
    {
        public string SourceFile { get; set; }
        public string DestFile { get; set; }
        public string StartMatch { get; set; }
        public string EndMatch { get; set; }
        public bool IncludeMarkers { get; set; }
        //public MatchMode MatchMode { get; set; }
        public bool AppendToFile { get; set; }


        public Settings()
        {
            IncludeMarkers = true;
            AppendToFile = true;
        }


        // Parse the arguments in to a settings object
        public static Settings Parse(string[] arguments)
        {
            if (arguments.Length < 4)
                throw new ArgumentException("Incorrect number of arguments");

            if (!File.Exists(arguments[0]))
                throw new ArgumentException("Path for the input file does not exist");           

            if (arguments[0].Trim().ToUpperInvariant().StartsWith(arguments[1].Trim().ToUpperInvariant()))
                throw new ArgumentException("Cannot overwrite the source file");


            Settings newsets = new Settings();
            newsets.SourceFile = arguments[0];
            newsets.DestFile = arguments[1];
            newsets.StartMatch = arguments[2];
            newsets.EndMatch = arguments[3];

            if (arguments.Length > 4)
                newsets.IncludeMarkers = ((arguments[4].Equals("yes", StringComparison.OrdinalIgnoreCase)) || (arguments[4].Equals("true", StringComparison.OrdinalIgnoreCase)));

/*
            if (arguments.Length > 5)
            {
                try
                {
                    string matchClean = arguments[4].ToLowerInvariant();
                    matchClean = Char.ToUpper(matchClean[0]) + matchClean.Substring(1);
                    newsets.MatchMode = (MatchMode)Enum.Parse(typeof(MatchMode), matchClean);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException(String.Format("Invalid Match Mode of '{0}' specified", arguments[4]));
                }
            }
            else
                newsets.MatchMode = MatchMode.Exact;
*/

            return newsets;
        }
    }
}
