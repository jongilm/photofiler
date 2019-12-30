using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhotoFiler
{
    class FilenamePrefix
    {
        public static bool fVerbose = false;

        public string fullyQualifiedFilename { get; set; }

        public string imageFilename { get; set; }
        public long imageFilesize { get; set; }
        public System.DateTime imageFiletime { get; set; }
        public string imageFiletimeString { get; set; }
        public bool hasValidFilenamePrefix { get; set; }
        public DateTime imageFilenamePrefix { get; set; }
        public string imageFilenamePrefixString { get; set; }

        public FilenamePrefix(string filename)
        {
            fullyQualifiedFilename = filename;

            imageFilename = new FileInfo(fullyQualifiedFilename).Name;
            imageFilesize = GetFilesize(fullyQualifiedFilename);
            imageFiletime = GetFiletime(fullyQualifiedFilename);
            imageFiletimeString = imageFiletime.ToString("yyyyMMdd'_'HHmmss");

            // If we don't have a timestamp in the filename, then return false
            DateTime temp;
            if (FilenamePrefixToDateTime(imageFilename, out temp ))
            {
                hasValidFilenamePrefix = true;
                imageFilenamePrefix = temp;
                imageFilenamePrefixString = imageFilenamePrefix.ToString("yyyyMMdd'_'HHmmss");
            }
            else
            {
                hasValidFilenamePrefix = false;
                imageFilenamePrefix = DateTime.MinValue;
                imageFilenamePrefixString = "";
            }
        }

        long GetFilesize(string filePath)
        {
            System.IO.FileInfo finfo = new System.IO.FileInfo(filePath);
            if (finfo == null)
            {
                Console.WriteLine($"FATAL: GetFilesize() cannot get FileInfo() for file: {filePath}");
                System.Environment.Exit(1);
            }
            long temp = finfo.Length;
            return temp;
        }

        System.DateTime GetFiletime(string filePath)
        {
            System.IO.FileInfo finfo = new System.IO.FileInfo(filePath);
            if (finfo == null)
            {
                Console.WriteLine($"FATAL: GetFiletime() cannot get FileInfo() for file: {filePath}");
                System.Environment.Exit(2);
            }
            //System.DateTime temp = finfo.CreationTime;
            System.DateTime temp = finfo.LastWriteTime;

            return temp;
        }

        public static bool FilenamePrefixToDateTime(String imageFilename, out DateTime dateTime )
        {
            // YYYYMMDD_HHMMSS
            // "yyyyMMdd'_'HHmmss"
            dateTime = DateTime.MinValue;

            if (imageFilename.Length < 14) return false;
            if (imageFilename.Substring(8,1) != "_") return false;

            try
            {
                string date_portion = imageFilename.Substring(0,4) + "/" + imageFilename.Substring(4,2) + "/" + imageFilename.Substring(6,2);
                string time_portion = imageFilename.Substring(9,2) + ":" + imageFilename.Substring(11,2) + ":" + imageFilename.Substring(13,2);
                dateTime = DateTime.Parse(date_portion + " " + time_portion);
            }
            catch (FormatException ex) // String was not recognized as a valid DateTime.
            {
                if (fVerbose)
                    Console.WriteLine($"Format Exception Handler: {ex}");
                return false;
            }
            catch (Exception ex) 
            {
                if (fVerbose)
                    Console.WriteLine($"Generic Exception Handler: {ex}");
                return false;
            }
            return true;
        }


    }
}
