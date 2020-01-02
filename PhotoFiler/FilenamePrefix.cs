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
        public static bool fDebugging = false;

        public string fullyQualifiedFilename { get; set; }

        public string imageFilename { get; set; }
        public long imageFilesize { get; set; }
        public System.DateTime imageFiletime { get; set; }
        public string imageFiletimeString { get; set; }
        public bool hasValidFilenamePrefix { get; set; }
        public DateTime imageFilenamePrefix { get; set; }
        public string imageFilenamePrefixString { get; set; }
        public bool filenameStartsWithYYYY { get; set; }

        public FilenamePrefix(string filename)
        {
            fullyQualifiedFilename = filename;

            imageFilename = new FileInfo(fullyQualifiedFilename).Name;
            imageFilesize = GetFilesize(fullyQualifiedFilename);
            imageFiletime = GetFiletime(fullyQualifiedFilename);
            imageFiletimeString = imageFiletime.ToString("yyyyMMdd'_'HHmmss");

            filenameStartsWithYYYY = FilenameStartsWithYYYY(imageFilename);

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
                    Console.WriteLine($"ERROR: FilenamePrefixToDateTime: {ex}");
                return false;
            }
            catch (Exception ex) 
            {
                if (fVerbose)
                    Console.WriteLine($"ERROR: FilenamePrefixToDateTime: {ex}");
                return false;
            }
            return true;
        }

        public static bool FilenameStartsWithYYYY (string imageFilename)
        { 
            // YYYYMMDD_blahblahblah
            // YYYYxxxx_blahblahblah
            // YYYYMMxx_blahblahblah
            // YYYYMMDD_xxxxxx_blahblahblah
            // YYYY_MM_DD_blahblahblah
            // YYYY_MM_DD_hh_mm_ss_blahblahblah
            // YYYY_MM_DD_xx_xx_xx_blahblahblah

            bool fFilenameStartsWithYYYY = false;
            int YYYY = 0;

            if (imageFilename.Length >= 4)
            { 
                int number;
                bool isParsable = Int32.TryParse(imageFilename.Substring(0,4), out number);
                if (isParsable)
                {
                    if (number>=1900 && number<=2099)
                    {
                        YYYY = number;
                        fFilenameStartsWithYYYY = true;
                    }
                }
            }
            return fFilenameStartsWithYYYY;
        }

        public bool hasMalformedFilenamePrefix()
        {
            if (filenameStartsWithYYYY && !hasValidFilenamePrefix)
            {
                return true;
            }
            return false;
        }

        public static string ModifyFilenameWithPrefixAndSuffix(string fullyQualifiedFilename, string prefix, string suffix)
        {
            string directory = Path.GetDirectoryName(fullyQualifiedFilename);
            string basename = Path.GetFileNameWithoutExtension(fullyQualifiedFilename);
            string extension = Path.GetExtension(fullyQualifiedFilename);
            if (prefix != null)
            {
                basename = String.Concat(prefix, basename);
            }
            if (suffix != null)
            {
                basename = String.Concat(basename, suffix);
            }
            string newfilename = String.Concat(basename, extension);
            string newfullyQualifiedFilename = Path.Combine(directory, newfilename);
            return newfullyQualifiedFilename;
        }

        private static bool IsValidYear(string str, int start, out int YYYY)
        {
            return IsValidNumber(str, start, 4, 1900, 2099, out YYYY);
        }

        private static bool IsValidMonth(string str, int start, out int MM)
        {
            return IsValidNumber(str, start, 2, 1, 12, out MM);
        }
        private static bool IsValidDay(string str, int start, int YYYY, int MM, out int DD)
        {
            return IsValidNumber(str, start, 2, 1, DateTime.DaysInMonth(YYYY,MM), out DD);
        }
        private static bool IsValidHour(string str, int start, out int hh)
        {
            return IsValidNumber(str, start, 2, 0, 23, out hh);
        }
        private static bool IsValidMinute(string str, int start, out int dd)
        {
            return IsValidNumber(str, start, 2, 0, 59, out dd);
        }
        private static bool IsValidSecond(string str, int start, out int ss)
        {
            return IsValidNumber(str, start, 2, 0, 59, out ss);
        }
        private static bool IsValidNumber(string str, int start, int length, int minValue, int maxValue, out int result)
        {
            result = 0;
            if (str.Length >= start+length)
            { 
                string substr = str.Substring(start,length);
                int number;
                bool isParsable = Int32.TryParse(substr, out number);
                if (isParsable)
                {
                    if (number>=minValue && number<=maxValue)
                    {
                        result = number;
                        if (fDebugging)
                            Console.WriteLine($"INFO: IsValidNumber({str}, {start}, {length} [{substr}], {minValue}, {maxValue}) = true: {result}");
                        return true;
                    }
                }
                else
                {
                    if (AllCharactersAreTheSame(substr, 'x') || AllCharactersAreTheSame(substr, 'X'))
                    {
                        result = 1;
                        if (fDebugging)
                            Console.WriteLine($"INFO: IsValidNumber({str}, {start}, {length} [{substr}], {minValue}, {maxValue}) = forced: {result}");
                        return true;
                    }
                }
                if (fDebugging)
                    Console.WriteLine($"INFO: IsValidNumber({str}, {start}, {length} [{substr}], {minValue}, {maxValue}) = false");
            }
            return false;
        }
        public static bool AllCharactersAreTheSame(string s, char whichchar)
        {
            return s.Length == 0 || s.All(ch => ch == whichchar);
        }
        private static bool IsValidSeperator(string str, int start)
        {
            switch (str.Substring(start,1))
            {
                case "-":
                case "/":
                case ".":
                case ":":
                case ",":
                case " ":
                case "_":
                    return true;
            }
            return false;
        }

        public static bool IsValid_YYYY_xx_xx(string basename, int start, out int YYYY, out int MM, out int DD)
        {
            YYYY = 1;
            MM = 1;
            DD = 1;
            // Test for YYYY-MM-DD (10)  (including YYYY-xx-xx etc)
            if (IsValidYear(basename, start, out YYYY) &&        // 4  [0..3]
                IsValidSeperator(basename, start+4) &&           // 1  [4]
                IsValidMonth(basename, start+5, out MM) &&       // 2  [5..6]
                IsValidSeperator(basename, start+7) &&           // 1  [7]
                IsValidDay(basename, start+8, YYYY, MM, out DD)) // 2  [8..9]
            {
                return true;
            }
            return false;
        }

        public static bool IsValid_xx_xx_xx(string basename, int start, out int hh, out int mm, out int ss)
        {
            hh = 12;
            mm = 0;
            ss = 0;
            // Test for YYYY-MM-DD (10)  (including YYYY-xx-xx etc)
            if (IsValidSeperator(basename, start) &&         // 1 [10]
                IsValidHour(basename, start+1, out hh) &&    // 2 [11..12]
                IsValidSeperator(basename, start+3) &&       // 1 [13]
                IsValidMinute(basename, start+4, out mm) &&  // 2 [14..15]
                IsValidSeperator(basename, start+6) &&       // 1 [16]
                IsValidSecond(basename, start+7, out ss))    // 2 [17..18]
            {
                return true;
            }
            return false;
        }
        public static bool IsValid_YYYYxxxx(string basename, int start, out int YYYY, out int MM, out int DD)
        {
            YYYY = 1;
            MM = 1;
            DD = 1;
            // Test for YYYYMMDD (8)  (including YYYYxxxx etc)
            if (IsValidYear(basename, start, out YYYY) &&      // 4 [0..3]
                IsValidMonth(basename, start+4, out MM) &&       // 2 [4..5]
                IsValidDay(basename, start+6, YYYY, MM, out DD)) // 2 [6..7]
            {
                return true;
            }
            return false;
        }

        public static bool IsValid_xxxxxx(string basename, int start, out int hh, out int mm, out int ss)
        {
            hh = 12;
            mm = 0;
            ss = 0;
            // Test for YYYYMMDD-hhmmss (15)  (including YYYYxxxx-xxxxxx etc)
            if (IsValidSeperator(basename, start) &&          // 1 [8]
                IsValidHour(basename, start+1, out hh) &&     // 2 [9..10]]
                IsValidMinute(basename, start+3, out mm) &&   // 2 [11..12]
                IsValidSecond(basename, start+5, out ss))     // 2 [13..14]
            {
                return true;
            }
            return false;
        }

        public static bool IsValid_xxxxxx_withoutseparator(string basename, int start, out int hh, out int mm, out int ss)
        {
            hh = 12;
            mm = 0;
            ss = 0;
            // Test for YYYYMMDDhhmmss (14)  (including YYYYxxxxxxxxxx etc)
            if (IsValidHour(basename, start, out hh) &&     // 2 [8..9]]
                IsValidMinute(basename, start+2, out mm) &&   // 2 [10..11]
                IsValidSecond(basename, start+4, out ss))     // 2 [12..13]
            {
                return true;
            }
            return false;
        }

        public static string RepairFilenamePrefix(string fullyQualifiedFilename)
        {
            string directory = Path.GetDirectoryName(fullyQualifiedFilename);
            string basename = Path.GetFileNameWithoutExtension(fullyQualifiedFilename);
            string extension = Path.GetExtension(fullyQualifiedFilename);

            string newbasename = basename;
            int YYYY;
            int MM;
            int DD;
            int hh;
            int mm;
            int ss;
            DateTime datetime;

            // Test for YYYY-MM-DD (10)  (including YYYY-xx-xx etc)
            if (IsValid_YYYY_xx_xx(basename, 0, out YYYY, out MM, out DD))
            {
                if (IsValid_xx_xx_xx(basename, 10, out hh, out mm, out ss)) // Test for -hh-mm-ss (19)  (including -xx-xx-xx etc)
                {
                    // YYYY-MM-DD-hh-mm-ss [19]
                    if (fDebugging)
                        Console.WriteLine($"INFO: RepairFilenamePrefix({YYYY},{MM},{DD},{hh},{mm},{ss})");
                    datetime = new DateTime(YYYY,MM,DD,hh,mm,ss);
                    newbasename =  String.Concat(datetime.ToString("yyyyMMdd'_'HHmmss"), basename.Substring(19));
                }
                else if (IsValid_xxxxxx(basename, 10, out hh, out mm, out ss)) // Test for -hhmmss (15)  (including -xxxxxx etc)
                {
                    // YYYY-MM-DD-hhmmss [17]
                    if (fDebugging)
                        Console.WriteLine($"INFO: RepairFilenamePrefix({YYYY},{MM},{DD},{hh},{mm},{ss})");
                    datetime = new DateTime(YYYY,MM,DD,hh,mm,ss);
                    newbasename =  String.Concat(datetime.ToString("yyyyMMdd'_'HHmmss"), basename.Substring(17));
                }
                else
                { 
                    // YYYY-MM-DD [10]
                    if (fDebugging)
                        Console.WriteLine($"INFO: RepairFilenamePrefix({YYYY},{MM},{DD} (DATE ONLY))");
                    datetime = new DateTime(YYYY,MM,DD,12,0,0);
                    newbasename =  String.Concat(datetime.ToString("yyyyMMdd'_'HHmmss"), basename.Substring(10));
                }
            }
            else
            {
                // Test for YYYYMMDD (8)  (including YYYYxxxx etc)
                if (IsValid_YYYYxxxx(basename, 0, out YYYY, out MM, out DD))
                {
                    if (IsValid_xx_xx_xx(basename, 8, out hh, out mm, out ss)) // Test for -hh-mm-ss (17)  (including -xx-xx-xx etc)
                    {
                        // YYYYMMDD-hh-mm-ss [17]
                        if (fDebugging)
                            Console.WriteLine($"INFO: RepairFilenamePrefix({YYYY},{MM},{DD},{hh},{mm},{ss})");
                        datetime = new DateTime(YYYY,MM,DD,hh,mm,ss);
                        newbasename =  String.Concat(datetime.ToString("yyyyMMdd'_'HHmmss"), basename.Substring(17));
                    }
                    else if (IsValid_xxxxxx(basename, 8, out hh, out mm, out ss)) // Test for -hhmmss (15)  (including -xxxxxx etc)
                    {
                        // YYYYMMDD-hhmmss [15]
                        if (fDebugging)
                            Console.WriteLine($"INFO: RepairFilenamePrefix({YYYY},{MM},{DD},{hh},{mm},{ss})");
                        datetime = new DateTime(YYYY,MM,DD,hh,mm,ss);
                        newbasename =  String.Concat(datetime.ToString("yyyyMMdd'_'HHmmss"), basename.Substring(15));
                    }
                    else if (IsValid_xxxxxx_withoutseparator(basename, 8, out hh, out mm, out ss)) // Test for hhmmss (14)  (including xxxxxx etc)
                    {
                        // YYYYMMDDhhmmss [14]
                        if (fDebugging)
                            Console.WriteLine($"INFO: RepairFilenamePrefix({YYYY},{MM},{DD},{hh},{mm},{ss})");
                        datetime = new DateTime(YYYY,MM,DD,hh,mm,ss);
                        newbasename =  String.Concat(datetime.ToString("yyyyMMdd'_'HHmmss"), basename.Substring(14));
                    }
                    else
                    { 
                        // YYYYMMDD [8]
                        if (fDebugging)
                            Console.WriteLine($"INFO: RepairFilenamePrefix({YYYY},{MM},{DD} (DATE ONLY))");
                        datetime = new DateTime(YYYY,MM,DD,12,0,0);
                        newbasename =  String.Concat(datetime.ToString("yyyyMMdd'_'HHmmss"), basename.Substring(8));
                    }
                }
            }
            string newfilename = String.Concat(newbasename, extension);
            string newfullyQualifiedFilename = Path.Combine(directory, newfilename);
            return newfullyQualifiedFilename;
        }
    }
}
