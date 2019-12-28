using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoFiler
{
    class Program
    {
        public static bool fVerbose = false;
        public static bool fRecursive = true;
        public static bool fBrandedCamerasOnly = true;
        public static bool fUnbrandedCamerasOnly = false;
        public static bool fKnownCamerasOnly = true;
        public static bool fUnknownCamerasOnly = true;

        public static int numberOfFilesProcessed = 0;
        public static int numberOfFilesShown = 0;
        public static int numberOfFilesFiltered = 0;

        static void Main( string[] args )
        {
            try
            {
                string filePath = @".";
                //filePath = "20191130_192659_IMG_5030.JPG";
                //filePath = @"C:\\Users\\Jonnie\\source\\repos\\PhotoFiler\\PhotoFiler\\bin\\Debug";
                //filePath = @"C:/Users/Jonnie/source/repos/PhotoFiler/PhotoFiler/bin/Debug";
                if (args.Length > 0)
                {
                    filePath = args[0];
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
            }
            if (fVerbose)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            return;
        }


        public static void DirectorySearch(string root, int depth, bool recursive = true, bool processRootDirectoryFiles = true )
        {
            if (processRootDirectoryFiles)
            {
                Console.WriteLine(depth + ": Files in \"" + root + "\" ...");
                var rootDirectoryFiles = Directory.GetFiles(root);
                foreach (var file in rootDirectoryFiles)
                {
                    ProcessImage(file);
                } 
            }
            if (recursive)
            { 
                Console.WriteLine(depth + ": Folders in \"" + root + "\" ...");
                var subDirectories = Directory.GetDirectories(root);
                if (subDirectories?.Any() == true)
                {
                    foreach (var directory in subDirectories)
                    {
                        var files = Directory.GetFiles(directory);
                        foreach (var file in files)
                        {
                            ProcessImage(file);
                        }
                        DirectorySearch(directory, depth+1, true, false);
                    }
                }
            }
        }

        static void ProcessImage( string filePath )
        {
            if (filePath.EndsWith(".jpg",StringComparison.OrdinalIgnoreCase) == false)
                return;

            if (fVerbose)
                Console.WriteLine("Processing: [" + filePath + "] ...");

            PhotoFiler.ExifData exif1 = new ExifData(filePath);

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
                double megapixels = (double)(exif1.imageHeight * exif1.imageWidth)/(1024*1024);
                
                Console.WriteLine(  Truncate(exif1.cameraMake,20) + "\t" +
                                    Truncate(exif1.cameraModel,20) + "\t" +
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
    }

} 

