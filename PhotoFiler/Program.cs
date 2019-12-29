using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoFiler
{
    class Program
    {
        public static bool fShowUsage = false;
        public static bool fVerbose = false;
        public static bool fRecursive = false;
        public static bool fBrandedCamerasOnly = false;
        public static bool fUnbrandedCamerasOnly = false;
        public static bool fKnownCamerasOnly = false;
        public static bool fUnknownCamerasOnly = false;
        public static bool fDimensionsFullsizeOnly = false;
        public static bool fDimensionsNotFullsizeOnly = false;

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
                    switch (args[ii])
                    {
                        case "-h": fShowUsage = true; break;
                        case "-v": fVerbose = true; break;
                        case "-r": fRecursive = true; break;
                        case "--branded": fBrandedCamerasOnly = true; break;
                        case "--unbranded": fUnbrandedCamerasOnly = true; break;
                        case "--known": fKnownCamerasOnly = true; break;
                        case "--unknown": fUnknownCamerasOnly = true; break;
                        case "--fullsize": fDimensionsFullsizeOnly = true; break;
                        case "--notfullsize": fDimensionsNotFullsizeOnly = true; break;
                        default: filePath = args[ii]; break;
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
                    Console.WriteLine("   -h         : fShowUsage");
                    Console.WriteLine("   -v         : fVerbose");
                    Console.WriteLine("   -r         : fRecursive");
                    Console.WriteLine("   --branded  : fBrandedCamerasOnly");
                    Console.WriteLine("   --unbranded: fUnbrandedCamerasOnly");
                    Console.WriteLine("   --known    : fKnownCamerasOnly");
                    Console.WriteLine("   --unknown  : fUnknownCamerasOnly");
                    Console.WriteLine("   --fullsize : fDimensionsFullsizeOnly");
                    Console.WriteLine("   --notfullsize : fDimensionsNotFullsizeOnly");
                    return -1;
                }
                if (fVerbose)
                    Console.WriteLine("Processing images in \"" + filePath + "\"");

                DirectorySearch(filePath, 0, fRecursive, true);
                Console.WriteLine("numberOfFilesProcessed = " + numberOfFilesProcessed );
                Console.WriteLine("numberOfFilesShown     = " + numberOfFilesShown );
                Console.WriteLine("numberOfFilesFiltered  = " + numberOfFilesFiltered );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"FATAL: {ex}");
                return -1;
            }
            if (fVerbose)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            return 0;
        }


        public static void DirectorySearch(string root, int depth, bool recursive = true, bool processRootDirectoryFiles = true )
        {
            if (processRootDirectoryFiles)
            {
                if (fVerbose)
                    Console.WriteLine(depth + ": Files in \"" + root + "\" ...");
                var rootDirectoryFiles = Directory.GetFiles(root);
                foreach (var file in rootDirectoryFiles)
                {
                    ProcessImage(file);
                } 
            }
            if (recursive)
            { 
                if (fVerbose) 
                    Console.WriteLine(depth + ": Folders in \"" + root + "\" ...");
                var subDirs = Directory.GetDirectories(root);
                if (subDirs?.Any() == true)
                {
                    foreach (var subDir in subDirs)
                    {
                        var files = Directory.GetFiles(subDir);
                        foreach (var file in files)
                        {
                            ProcessImage(file);
                        }
                        DirectorySearch(subDir, depth+1, true, false);
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
            numberOfFilesShown++;

            if (fVerbose)
            {
                Console.WriteLine("   cameraMake: " + exif1.cameraMake);
                Console.WriteLine("   cameraModel: " + exif1.cameraModel);
                //Console.WriteLine("   lensMake: " + exif1.lensMake);
                //Console.WriteLine("   lensModel: " + exif1.lensModel);
                Console.WriteLine("   imageHeight: " + exif1.imageHeight);
                Console.WriteLine("   imageWidth: " + exif1.imageWidth);
                Console.WriteLine("   datetimeOriginal: " + exif1.datetimeOriginalString);
                Console.WriteLine("   imageFiletime: " + exif1.imageFiletimeString);
                Console.WriteLine("   imageFilesize: " + exif1.imageFilesize);
                Console.WriteLine("   imageFilename: " + exif1.imageFilename);
            }
            else
            {
                double megapixels = (double)(pixels)/(1024*1024);
                Console.WriteLine(  Truncate(exif1.cameraMake,22) + "\t" +
                                    Truncate(exif1.cameraModel,22) + "\t" +
                                    exif1.imageHeight + "\t" + 
                                    exif1.imageWidth + "\t" + 
                                    String.Format("{0,8:0.0000}", megapixels) + "\t" + 
                                    exif1.datetimeOriginalString + "\t" + 
                                    exif1.imageFiletimeString + "\t" + 
                                    exif1.imageFilesize + "\t" + 
                                    exif1.imageFilename);
            }

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
            if (make == "Eastman Kodak Company"   && model == "DC210 Zoom (V05.00)" && pixels == (864*1152) ) return true;
            if (make == "FUJIFILM"                && model == "FinePix A610"        && pixels == (2136*2848) ) return true;
            if (make == "Vivitar"                 && model == "ViviCam 3835"        && pixels == (1200*1600) ) return true;
            if (make == "Panasonic"               && model == "NV-GS120"            && pixels == (1152*1520) ) return true;
            if (make == "Canon"                   && model == "MG4200 series"       && pixels == (1500*2096) ) return true;
            if (make == "Nokia"                   && model == "E90"                 && pixels == (1536*2048) ) return true;
            if (make == "SAMSUNG"                 && model == "GT-S7275R"           && pixels == (2560*1920) ) return true;
            if (make == "SAMSUNG"                 && model == "GT-I8160"            && pixels == (2592*1944) ) return true;
            if (make == "Sony"                    && model == "E6653"               && pixels == (3366*5984) ) return true;
            if (make == "Sony"                    && model == "E6653"               && pixels == (2592*1458) ) return true;
            if (make == "Panasonic"               && model == "DMC-TZ1"             && pixels == (1920*2560) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 500D"      && pixels == (3168*4752) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 750D"      && pixels == (4000*6000) ) return true;
            if (make == "Google"                  && model == "Pixel 3a"            && pixels == (4032*3024) ) return true;
            return false;
        }
    }

} 

