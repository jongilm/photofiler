using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoFiler
{
    class Program
    {
        // Switches
        public static bool fTesting = false;
        public static bool fShowUsage = false;
        public static bool fVerbose = false;
        public static bool fRecursive = false;
        // Filters
        public static bool fBrandedCamerasOnly = false;
        public static bool fUnbrandedCamerasOnly = false;
        public static bool fKnownCamerasOnly = false;
        public static bool fUnknownCamerasOnly = false;
        public static bool fDimensionsFullsizeOnly = false;
        public static bool fDimensionsNotFullsizeOnly = false;

        public static bool fIncorrectFiletimeVsExifTimeOnly = false;
        public static bool fIncorrectFilenamePrefixVsExifTimeOnly = false;
        public static bool fIncorrectFilenamePrefixVsFiletimeOnly = false;
        public static bool fCorrectFiletimeVsExifTimeOnly = false;
        public static bool fCorrectFilenamePrefixVsExifTimeOnly = false;
        public static bool fCorrectFilenamePrefixVsFiletimeOnly = false;
        
        // Actions
        public static bool fFixFiletimes = false;
        public static bool fForceUnderscores = false;
        public static bool fForceSpacesAndHyphens = false;
        public static bool fFileByDate = false;
        // Counters
        public static int numberOfFilesProcessed = 0;
        public static int numberOfFilesShown = 0;
        public static int numberOfFilesFiltered = 0;

        static int Main( string[] args )
        {
            try
            {
                string filePath = @".";
                //filePath = "20191130_192659_IMG_5030.JPG";
                //filePath = @"C:\\Users\\Jonnie\\source\\repos\\PhotoFiler\\PhotoFiler\\bin\\Debug";
                //filePath = @"C:/Users/Jonnie/source/repos/PhotoFiler/PhotoFiler/bin/Debug";
                for (int ii = 0; ii<args.Length; ii++)
                {
                    if (args[ii].Substring(0,1) == "-")
                    { 
                        switch (args[ii].ToLower())
                        {
                            case "-t": fTesting = true; break;
                            case "-h": fShowUsage = true; break;
                            case "-v": fVerbose = true; ExifData.fVerbose = true; FilenamePrefix.fVerbose = true; break;
                            case "-r": fRecursive = true; break;
                            case "--branded": fBrandedCamerasOnly = true; break;
                            case "--unbranded": fUnbrandedCamerasOnly = true; break;
                            case "--known": fKnownCamerasOnly = true; break;
                            case "--unknown": fUnknownCamerasOnly = true; break;
                            case "--fullsize": fDimensionsFullsizeOnly = true; break;
                            case "--notfullsize": fDimensionsNotFullsizeOnly = true; break;
                            case "--correctfiletimevsexiftime": fCorrectFiletimeVsExifTimeOnly = true; break;
                            case "--correctfilenameprefixvsexiftime": fCorrectFilenamePrefixVsExifTimeOnly = true; break;
                            case "--correctfilenameprefixvsfiletime": fCorrectFilenamePrefixVsFiletimeOnly = true; break;
                            case "--incorrectfiletimevsexiftime": fIncorrectFiletimeVsExifTimeOnly = true; break;
                            case "--incorrectfilenameprefixvsexiftime": fIncorrectFilenamePrefixVsExifTimeOnly = true; break;
                            case "--incorrectfilenameprefixvsfiletime": fIncorrectFilenamePrefixVsFiletimeOnly = true; break;
                            case "--fixfiletimes": fFixFiletimes = true; break;
                            case "--forceunderscores": fForceUnderscores = true; break;
                            case "--forcespacesandhyphens": fForceSpacesAndHyphens = true; break;
                            case "--filebydate": fFileByDate = true; break;
                            default: Console.WriteLine($"Unrecognised option: {args[ii]}"); return -1;
                        }
                    }
                    else
                    {
                        filePath = args[ii];
                    }
                }
                if (filePath.Length == 0)
                {
                    fShowUsage = true;
                }
                if (fShowUsage)
                {
                    Console.WriteLine("Usage: PhotoFiler [options] <PathToPhotos>");
                    Console.WriteLine("Options:");
                    Console.WriteLine("   -h                         : Show Usage");
                    Console.WriteLine("   -v                         : Verbose");
                    Console.WriteLine("   -r                         : Recursive");
                    Console.WriteLine("Filters:");
                    Console.WriteLine("   --Branded                  : Process only images that have Camera Make and Model in the EXIF metadata");
                    Console.WriteLine("   --Unbranded                : Process only images that DO NOT have Camera Make and Model in the EXIF metadata");
                    Console.WriteLine("   --Known                    : Process all images taken with one of MY Cameras");
                    Console.WriteLine("   --Unknown                  : Process all images except those taken with one MY Cameras");
                    Console.WriteLine("   --Fullsize                 : Process only images that appear to have their original dimensions");
                    Console.WriteLine("   --NotFullsize              : Process only images that appear to have been cropped or resized");
                    Console.WriteLine("   --CorrectFiletimeVsExifTime      : Process only images that have a file timestamp that matches Exif date taken");
                    Console.WriteLine("   --CorrectFilenamePrefixVsExifTime   : Process only images that have a filename prefix that matches Exif date taken");
                    Console.WriteLine("   --CorrectFilenamePrefixVsFiletime : Process only images that have a filename prefix that matches the FS file timestamp");
                    Console.WriteLine("   --IncorrectFiletimes         : Process only images that have a file timestamp that does not match Exif date taken");
                    Console.WriteLine("   --IncorrectFilenamePrefixVsExifTime : Process only images that have a filename prefix that does not match Exif date taken");
                    Console.WriteLine("   --IncorrectFilenamePrefixVsFiletime : Process only images that have a filename prefix that does not match the FS file timestamp");
                    //Console.WriteLine("Actions:");
                    //Console.WriteLine("   --FixFilenamePrefixes      : Add/Fix Filename timestamp prefixes");
                    //Console.WriteLine("   --FixFiletimes          : FixFiletimes");
                    //Console.WriteLine("   --ForceUnderscores         : ForceUnderscores");
                    //Console.WriteLine("   --ForceSpacesAndHyphens    : ForceSpacesAndHyphens");
                    //Console.WriteLine("   --FileByDate               : FileByDate");
                    //Console.WriteLine("   --FixAll                   : FixFiletimes, ForceUnderscores");
                    return 0;
                }
                if (fTesting)
                {
                    DateTime imageFilenamePrefix;
                    String imageFilename = "20101311_121314_blahblah.jpg";
                    bool rc = FilenamePrefix.FilenamePrefixToDateTime(imageFilename, out imageFilenamePrefix );
                    if (rc)
                    {
                        Console.WriteLine($"{imageFilename} ==> {imageFilenamePrefix}");
                    }
                    else
                    {
                        Console.WriteLine($"{imageFilename} ==> {rc}");
                    }
                    return 0;
                }
                if (fVerbose)
                    Console.WriteLine("Processing images in \"" + filePath + "\"");

                DirectorySearch(filePath, 0, fRecursive);
                Console.WriteLine("numberOfFilesProcessed = " + numberOfFilesProcessed );
                Console.WriteLine("numberOfFilesShown     = " + numberOfFilesShown );
                Console.WriteLine("numberOfFilesFiltered  = " + numberOfFilesFiltered );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"FATAL: {ex}");
                return -1;
            }
            //if (fVerbose)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            return 0;
        }


        public static void DirectorySearch(string thisDir, int depth, bool recursive = true )
        {
            // Process each file in this folder
            if (fVerbose)
                Console.WriteLine(depth + ": Files in \"" + thisDir + "\" ...");
            var files1 = Directory.GetFiles(thisDir);
            foreach (var file in files1)
            {
                ProcessImage(file);
            } 

            // Then process each subfolder in this folder
            if (recursive)
            { 
                if (fVerbose) 
                    Console.WriteLine(depth + ": Folders in \"" + thisDir + "\" ...");
                var subDirs = Directory.GetDirectories(thisDir);
                if (subDirs?.Any() == true)
                {
                    foreach (var subDir in subDirs)
                    {
                        DirectorySearch(subDir, depth+1, recursive);
                    }
                }
            }
        }

        public static void ProcessImage( string filePath )
        {
            if (filePath.EndsWith(".jpg",StringComparison.OrdinalIgnoreCase) == false)
                return;

            if (fVerbose)
                Console.WriteLine("Processing: [" + filePath + "] ...");

            PhotoFiler.ExifData exif1 = new ExifData(filePath);
            PhotoFiler.FilenamePrefix fnp1 = new FilenamePrefix(filePath);

            long pixels = (exif1.imageHeight * exif1.imageWidth);

            numberOfFilesProcessed++;
            if (fBrandedCamerasOnly && (exif1.cameraMake == "" || exif1.cameraModel == ""))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fUnbrandedCamerasOnly && (exif1.cameraMake != "" || exif1.cameraModel != ""))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fKnownCamerasOnly && !isKnownCamera(exif1.cameraMake, exif1.cameraModel))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fUnknownCamerasOnly && isKnownCamera(exif1.cameraMake, exif1.cameraModel))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fDimensionsFullsizeOnly && !isFullSizeImage(exif1.cameraMake, exif1.cameraModel, pixels))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fDimensionsNotFullsizeOnly && isFullSizeImage(exif1.cameraMake, exif1.cameraModel, pixels))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fIncorrectFiletimeVsExifTimeOnly && isFiletimeCorrect(exif1.datetimeOriginal, fnp1.imageFiletime ))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fIncorrectFilenamePrefixVsExifTimeOnly && isFilenamePrefixCorrect(exif1.datetimeOriginal, fnp1 ))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fIncorrectFilenamePrefixVsFiletimeOnly && isFilenamePrefixCorrect(fnp1.imageFiletime, fnp1 ))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fCorrectFiletimeVsExifTimeOnly && !isFiletimeCorrect(exif1.datetimeOriginal, fnp1.imageFiletime ))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fCorrectFilenamePrefixVsExifTimeOnly && !isFilenamePrefixCorrect(exif1.datetimeOriginal, fnp1 ))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fCorrectFilenamePrefixVsFiletimeOnly && !isFilenamePrefixCorrect(fnp1.imageFiletime, fnp1 ))
            {
                numberOfFilesFiltered++;
                return;
            }
            numberOfFilesShown++;

            if (fVerbose)
            {
                Console.WriteLine("   cameraMake/Model: " + exif1.cameraMake + "/" + exif1.cameraModel);
                //Console.WriteLine("   lensMake/Model: " + exif1.lensMake + "/" + exif1.lensModel);
                Console.WriteLine("   imageHeight x Width: " + exif1.imageHeight + " x " + exif1.imageWidth);
                Console.WriteLine("   datetimeOriginal: " + exif1.datetimeOriginalString);
                Console.WriteLine("   imageFiletime: " + fnp1.imageFiletimeString);
                Console.WriteLine("   imageFilenamePrefix: " + fnp1.imageFilenamePrefixString);
                Console.WriteLine("   imageFilesize: " + fnp1.imageFilesize);
                Console.WriteLine("   imageFilename: " + fnp1.imageFilename);
            }
            else
            {
                double megapixels = (double)(pixels)/(1024*1024);
                Console.WriteLine(  "Ma:" + Truncate(exif1.cameraMake,22) + "\t" +
                                    "Mo:" + Truncate(exif1.cameraModel,22) + "\t" +
                                    "HxW:" + exif1.imageHeight + "x" + exif1.imageWidth + "\t" + 
                                    "MP:" + String.Format("{0,8:0.0000}", megapixels) + "\t" + 
                                    "Ex:" + Truncate(exif1.datetimeOriginalString,15) + "\t" + 
                                    "Ft:" + Truncate(fnp1.imageFiletimeString,15) + "\t" + 
                                    "Fp:" + Truncate(fnp1.imageFilenamePrefixString,15) + "\t" + 
                                    "Fs:" + fnp1.imageFilesize + "\t" + 
                                    "Fn:" + fnp1.imageFilename);
            }
            //fFixFiletimes
            //fForceUnderscores
            //fForceSpacesAndHyphens
            //fFileByDate

        }

        public static string Truncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }
            else if (source.Length < length)
            {
                source = source.PadRight(length);
            }
            return source;
        }

        public static bool isKnownCamera (string make, string model)
        {
            make = make.Trim();
            model = model.Trim();

            if (make == "Eastman Kodak Company"   && model == "DC210 Zoom (V05.00)") return true; // First digital family camera
            if (make == "FUJIFILM"                && model == "FinePix A610") return true;  // Old trusty family camera
            if (make == "Vivitar"                 && model == "ViviCam 3835") return true;  // Clemy's orange camera?
            if (make == "Panasonic"               && model == "NV-GS120") return true;      // DV Video Camera ?
            if (make == "Canon"                   && model == "MG4200 series") return true; // Scanner ?
            if (make == "Nokia"                   && model == "E90") return true;        // Nokia E90 Communicator mobile phone
            if (make == "SAMSUNG"                 && model == "GT-S7275R") return true;  // Samsung Galaxy Ace3 mobile phone ?
            if (make == "SAMSUNG"                 && model == "GT-I8160") return true;   // Samsung Galaxy Ace 2 mobile phone
            if (make == "Sony"                    && model == "E6653") return true;      // Sony Z5 mobile phone
            if (make == "Panasonic"               && model == "DMC-TZ1") return true;    // Panasonic DMC-TZ1
            if (make == "Canon"                   && model == "Canon EOS 500D") return true;
            if (make == "Canon"                   && model == "Canon EOS 750D") return true;
            if (make == "Google"                  && model == "Pixel 3a") return true;   // Google Pixel 3a mobile phone
            return false;
        }

        public static bool isFullSizeImage (string make, string model, long pixels)
        {
            make = make.Trim();
            model = model.Trim();

            if (make == "Eastman Kodak Company"   && model == "DC210 Zoom (V05.00)" && pixels == (1152*864) ) return true;
            if (make == "FUJIFILM"                && model == "FinePix A610"        && pixels == (2848*2136) ) return true;
            if (make == "Vivitar"                 && model == "ViviCam 3835"        && pixels == (1600*1200) ) return true;
            if (make == "Panasonic"               && model == "NV-GS120"            && pixels == (1520*1152) ) return true;
            if (make == "Canon"                   && model == "MG4200 series"       && pixels == (2096*1500) ) return true;
            if (make == "Nokia"                   && model == "E90"                 && pixels == (2048*1536) ) return true;
            if (make == "SAMSUNG"                 && model == "GT-S7275R"           && pixels == (2560*1920) ) return true;
            if (make == "SAMSUNG"                 && model == "GT-I8160"            && pixels == (2592*1944) ) return true;
            if (make == "Sony"                    && model == "E6653"               && pixels == (5984*3366) ) return true;
            if (make == "Sony"                    && model == "E6653"               && pixels == (2592*1458) ) return true;
            if (make == "Panasonic"               && model == "DMC-TZ1"             && pixels == (2560*1920) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 500D"      && pixels == (4752*3168) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 750D"      && pixels == (6000*4000) ) return true;
            if (make == "Google"                  && model == "Pixel 3a"            && pixels == (4032*3024) ) return true;
            if (make == "Google"                  && model == "Pixel 3a"            && pixels == (3264*2448) ) return true;
            return false;
        }

        public static bool RoughlyEquals(DateTime time, DateTime timeWithWindow, int windowInSeconds, int frequencyInSeconds)
        {
            long delta = (long)((TimeSpan)(timeWithWindow - time)).TotalSeconds % frequencyInSeconds;
            delta = delta > windowInSeconds ? frequencyInSeconds - delta : delta;
            return Math.Abs(delta) < windowInSeconds;
        }

        public static bool isFiletimeCorrect(DateTime? datetimeOriginal, DateTime? imageFiletime )
        {
            int window = 10;
            int freq = 60 * 60 * 2; // 2 hours;
            DateTime temp1;

            // If we don't have an exif datetime, then we have to assume that the filetime is correct.
            if (datetimeOriginal==null) return true;
            // If the filetime is null, then we have bigger problems
            if (imageFiletime==null) return false;

            if (RoughlyEquals((DateTime)datetimeOriginal, (DateTime)imageFiletime, window, freq)) return true;
            temp1 = ((DateTime)imageFiletime).AddHours(1);
            if (RoughlyEquals((DateTime)datetimeOriginal, temp1, window, freq)) return true;
            temp1 = ((DateTime)imageFiletime).AddHours(-1);
            if (RoughlyEquals((DateTime)datetimeOriginal, temp1, window, freq)) return true;

            return false;
        }

        public static bool isFilenamePrefixCorrect(DateTime? datetimeOriginal, FilenamePrefix fnp1 )
        {
            // If we don't have a timestamp in the filename, then return false
            if (fnp1.hasValidFilenamePrefix==false) return false;

            // If we don't have an exif datetime, then we have to assume that any time in the filename is correct.
            if (datetimeOriginal==null) return true;

            int window = 10;
            int freq = 60 * 60 * 2; // 2 hours;
            DateTime temp1;

            if (RoughlyEquals((DateTime)datetimeOriginal, fnp1.imageFilenamePrefix, window, freq)) return true;
            temp1 = fnp1.imageFilenamePrefix.AddHours(1);
            if (RoughlyEquals((DateTime)datetimeOriginal, temp1, window, freq)) return true;
            temp1 = fnp1.imageFilenamePrefix.AddHours(-1);
            if (RoughlyEquals((DateTime)datetimeOriginal, temp1, window, freq)) return true;

            return false;
        }

    }

} 

