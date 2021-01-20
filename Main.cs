using System.IO;
using System.Text;
using System;


// To strip content between two lines
namespace TallSequoia.StripToLines
{
    public class UI
    {
        public static void Main(string[] args)
        {
            Settings set;
            try
            {
                set = Settings.Parse(args);
            }
            catch (System.ArgumentException ex)
            {
                ShowHelp(ex.Message);
                Environment.Exit(-1);           
                return;     
            }


            // Do the processing

            string readLine = String.Empty;
            bool inMatch = false;
            int linePos = 0;

            using(FileStream sourceStream = new FileStream(set.SourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Encoding inputEncoding = GetFileEncoding(sourceStream);

                try
                {
                    using (StreamWriter destWriter = new StreamWriter(set.DestFile, false, inputEncoding))
                    {
                        StreamReader sourceReader = new StreamReader(sourceStream, inputEncoding, true, 128);

                        while((readLine = sourceReader.ReadLine()) != null)
                        {
                            linePos++;

                            if (!inMatch)
                            {
                                // See if it matches the initial string
                                inMatch = (readLine.Equals(set.StartMatch));

                                if (inMatch)
                                {
                                    Console.WriteLine(" Start match line found at line " + linePos);

                                    if (set.IncludeMarkers)
                                        destWriter.WriteLine(readLine);
                                }
                            }
                            else
                            {
                                // See if it matches the end string (case-sensitive)
                                if (readLine.Equals(set.EndMatch))
                                {
                                    Console.WriteLine(" End match line found at line " + linePos);

                                    if (set.IncludeMarkers)
                                        destWriter.WriteLine(readLine);

                                    destWriter.Flush();
                                    destWriter.Close();

                                    sourceStream.Close();

                                    Environment.Exit(1);           
                                    return;     
                                }

                                // Write the line to the destination
                                destWriter.WriteLine(readLine);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("A BAD THING HAPPENED: " + ex.Message);
                }
            }
        }



        /// <summary>
        /// Detects the byte order mark of a file and returns an appropriate encoding for the file.
        /// </summary>
        /// <param name="sourceStream">Initialised stream to read</param>
        /// <returns>File encoding</returns>
        // Source: https://weblog.west-wind.com/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader, but modified to work with a stream and a (weak) try/catch
        static Encoding GetFileEncoding(Stream sourceStream)
        {
            // Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            try
            {
                // Read the first five bytes
                byte[] buffer = new byte[5];
                sourceStream.Read(buffer, 0, 5);

                // Rewind to the start
                sourceStream.Position = 0;
            

                if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                    enc = Encoding.UTF8;
                else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                    enc = Encoding.Unicode;
                else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                    enc = Encoding.UTF32;
                else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                    enc = Encoding.UTF7;
            }
            catch {}

            return enc;
        }


        static void ShowHelp(string reason)
        {
            Console.WriteLine("");
            Console.WriteLine("HELP: StripToLines");
            Console.WriteLine("");

            if (reason != null && reason.Length > 0)
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(" " + reason);
                Console.WriteLine("");                
            }

            Console.WriteLine("PURPOSE:");
            Console.WriteLine(" Strip a file to only contain between two lines");
            Console.WriteLine("");
            Console.WriteLine("PARAMETERS:");
            Console.WriteLine(" All parameters need to be in quotation marks");
            Console.WriteLine(" * Full file path to read from");
            Console.WriteLine(" * Full file path to read to");
            Console.WriteLine(" * Start pattern to match");
            Console.WriteLine(" * End pattern to match");
            Console.WriteLine(" * (optional) Include the start and end patterns - defaults to yes");
            //Console.WriteLine(" * (optional) Match mode - EXACT or REGEX - defaults to EXACT");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLE:");
            Console.WriteLine(" \"in.txt\" \"out.txt\" \"---START MARKER---\" \"---END MARKER---\" yes");
            Console.WriteLine("");
        }
    }    
}
