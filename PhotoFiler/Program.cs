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
        public static bool fDummyRun = false;
        // Filters
        public static bool fShowOnlyBrandedCameras = false;
        public static bool fShowOnlyUnbrandedCameras = false;
        public static bool fShowOnlyKnownCameras = false;
        public static bool fShowOnlyUnknownCameras = false;
        public static bool fShowOnlyDimensionsFullsize = false;
        public static bool fShowOnlyDimensionsNotFullsize = false;

        public static bool fShowOnlyIncorrectFiletimeVsExifTime = false;
        public static bool fShowOnlyIncorrectFilenamePrefixVsExifTime = false;
        public static bool fShowOnlyIncorrectFilenamePrefixVsFiletime = false;
        public static bool fShowOnlyCorrectFiletimeVsExifTime = false;
        public static bool fShowOnlyCorrectFilenamePrefixVsExifTime = false;
        public static bool fShowOnlyCorrectFilenamePrefixVsFiletime = false;
        public static bool fShowMalformedFilenamePrefix = false;
        public static bool fShowMissingFilenamePrefix = false;
        // Actions
        public static bool fActionSetFiletimeFromExifTime = false;
        public static bool fActionFixMalformedFilenamePrefix = false;
        public static bool fActionAddMissingFilenamePrefix = false;
        //public static bool fActionForceUnderscores = false;
        //public static bool fActionForceSpacesAndHyphens = false;
        //public static bool fActionFileByDate = false;
        // Counters
        public static int numberOfFilesProcessed = 0;
        public static int numberOfFilesShown = 0;
        public static int numberOfFilesFiltered = 0;
        public static int numberOfFilesActioned = 0;

        static int Main( string[] args )
        {
            Console.WriteLine("PhotoFiler v0.80 Copyright (c) 2019 Jonathan Gilmore");
            try
            {
                string filePath = @".";
                //filePath = "20191130_192659_IMG_5030.JPG";
                //filePath = @"C:\\Users\\Jonnie\\source\\repos\\PhotoFiler\\PhotoFiler\\bin\\Debug";
                //filePath = @"C:/Users/Jonnie/source/repos/PhotoFiler/PhotoFiler/bin/Debug";
                for (int ii = 0; ii<args.Length; ii++)
                {
                    Console.WriteLine($"args[{ii}]: {args[ii]}");
                    if (args[ii].Substring(0,1) == "-")
                    { 
                        switch (args[ii].ToLower())
                        {
                            case "-t": fTesting = true; break;
                            case "-h": fShowUsage = true; break;
                            case "-v": fVerbose = true; ExifData.fVerbose = true; FilenamePrefix.fVerbose = true; break;
                            case "-r": fRecursive = true; break;
                            case "-d": fDummyRun = true; break;

                            case "--showbranded": fShowOnlyBrandedCameras = true; break;
                            case "--showunbranded": fShowOnlyUnbrandedCameras = true; break;
                            case "--showknown": fShowOnlyKnownCameras = true; break;
                            case "--showunknown": fShowOnlyUnknownCameras = true; break;
                            case "--showfullsize": fShowOnlyDimensionsFullsize = true; break;
                            case "--shownotfullsize": fShowOnlyDimensionsNotFullsize = true; break;
                            case "--showcorrectfiletimevsexiftime": fShowOnlyCorrectFiletimeVsExifTime = true; break;
                            case "--showcorrectfilenameprefixvsexiftime": fShowOnlyCorrectFilenamePrefixVsExifTime = true; break;
                            case "--showcorrectfilenameprefixvsfiletime": fShowOnlyCorrectFilenamePrefixVsFiletime = true; break;
                            case "--showincorrectfiletimevsexiftime": fShowOnlyIncorrectFiletimeVsExifTime = true; break;
                            case "--showincorrectfilenameprefixvsexiftime": fShowOnlyIncorrectFilenamePrefixVsExifTime = true; break;
                            case "--showincorrectfilenameprefixvsfiletime": fShowOnlyIncorrectFilenamePrefixVsFiletime = true; break;
                            case "--showmalformedfilenameprefix": fShowMalformedFilenamePrefix = true; break;
                            case "--showmissingfilenameprefix": fShowMissingFilenamePrefix = true; break;

                            case "--setfiletimesfromexiftimes": fActionSetFiletimeFromExifTime = true; break;
                            case "--fixmalformedfilenameprefix": fActionFixMalformedFilenamePrefix = true; break;
                            case "--addmissingfilenameprefix": fActionAddMissingFilenamePrefix = true; break;
                            //case "--forceunderscores": fActionForceUnderscores = true; break;
                            //case "--forcespacesandhyphens": fActionForceSpacesAndHyphens = true; break;
                            //case "--filebydate": fActionFileByDate = true; break;
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
                    Console.WriteLine("   -d                         : Dummy Run");
                    Console.WriteLine("Filters:");
                    Console.WriteLine("   --ShowBranded                           : Process only images that have Camera Make and Model in the EXIF metadata");
                    Console.WriteLine("   --ShowUnbranded                         : Process only images that DO NOT have Camera Make and Model in the EXIF metadata");
                    Console.WriteLine("   --ShowKnown                             : Process all images taken with one of MY Cameras");
                    Console.WriteLine("   --ShowUnknown                           : Process all images except those taken with one MY Cameras");
                    Console.WriteLine("   --ShowFullsize                          : Process only images that appear to have their original dimensions");
                    Console.WriteLine("   --ShowNotFullsize                       : Process only images that appear to have been cropped or resized");
                    Console.WriteLine("DateTime Filters:");
                    Console.WriteLine("   --ShowCorrectFiletimeVsExifTime         : Process only images that have a file timestamp that matches Exif date taken");
                    Console.WriteLine("   --ShowCorrectFilenamePrefixVsExifTime   : Process only images that have a filename prefix that matches Exif date taken");
                    Console.WriteLine("   --ShowCorrectFilenamePrefixVsFiletime   : Process only images that have a filename prefix that matches the FS file timestamp");
                    Console.WriteLine("   --ShowIncorrectFiletimeVsExifTime       : Process only images that have a file timestamp that does not match Exif date taken");
                    Console.WriteLine("   --ShowIncorrectFilenamePrefixVsExifTime : Process only images that have a filename prefix that does not match Exif date taken");
                    Console.WriteLine("   --ShowIncorrectFilenamePrefixVsFiletime : Process only images that have a filename prefix that does not match the FS file timestamp");
                    Console.WriteLine("   --ShowMalformedFilenamePrefix           : Process only images that have a filename prefix that is formatted differently to YYYYMMDD_HHMMSS");
                    Console.WriteLine("   --ShowMissingFilenamePrefix             : Process only images that have a no filename prefix, including any malformed prefix");
                    Console.WriteLine("Actions:");
                    Console.WriteLine("   --SetFiletimesFromExifTimes             : Set File timestamps from Exif timestamps");
                    Console.WriteLine("   --FixMalformedFilenamePrefix            : Fix Malformed Filename timestamp prefixes");
                    Console.WriteLine("   --AddMissingFilenamePrefix              : Add Filename timestamp prefix if not already present (and not malformed)");
                    //Console.WriteLine("   --ForceUnderscores         : ForceUnderscores");
                    //Console.WriteLine("   --ForceSpacesAndHyphens    : ForceSpacesAndHyphens");
                    //Console.WriteLine("   --FileByDate               : FileByDate");
                    //Console.WriteLine("   --FixAll                   : fActionAddMissingFilenamePrefix, fSetFiletimesFromExifTimes, fForceUnderscores");
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
                Console.WriteLine("numberOfFilesActioned  = " + numberOfFilesActioned );
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
            try
            { 
                if (filePath.EndsWith(".jpg",StringComparison.OrdinalIgnoreCase) == false)
                    return;
            }
            catch (System.UnauthorizedAccessException ex)
            {
                if (fVerbose)
                    Console.WriteLine($"ERROR: ProcessImage: {ex}");
                Console.WriteLine("ERROR: UnauthorizedAccessException [" + filePath + "]. Skipping.");
                return;
            }
            if (fVerbose)
                Console.WriteLine("Processing: [" + filePath + "] ...");

            if (filePath.Length >= 260)
            {
                Console.WriteLine("ERROR: File path too long [" + filePath + "]. Skipping.");
                return;
            }

            ////////////////////////////////
            // Get image metadata and file attrs
            ////////////////////////////////
            PhotoFiler.ExifData exif1 = new ExifData(filePath);
            PhotoFiler.FilenamePrefix fnp1 = new FilenamePrefix(filePath);

            // Testing
            //if (fnp1.imageFilename == "SCN_0004_photo_used_at_Andre_and_Debbies_25th_Anniv.jpg.jpg" ||
            //    fnp1.imageFilename == "DCP03364.jpg")
            //{
            //    Console.WriteLine($"DEBUGGING: isFiletimeCorrect() - Inconsistent metadata");
            //}


            ////////////////////////////////
            // Apply filters
            ////////////////////////////////
            numberOfFilesProcessed++;
            if (fShowOnlyBrandedCameras && !exif1.isBranded())
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyUnbrandedCameras && exif1.isBranded())
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyKnownCameras && !isKnownCamera(exif1.cameraMake, exif1.cameraModel))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyUnknownCameras && isKnownCamera(exif1.cameraMake, exif1.cameraModel))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyDimensionsFullsize && !isFullSizeImage(exif1.cameraMake, exif1.cameraModel, exif1.pixels))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyDimensionsNotFullsize && isFullSizeImage(exif1.cameraMake, exif1.cameraModel, exif1.pixels))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyIncorrectFiletimeVsExifTime && (!exif1.hasValidExifTimestamp || isFiletimeCorrect(exif1, fnp1)))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyIncorrectFilenamePrefixVsExifTime && (!fnp1.hasValidFilenamePrefix || !exif1.hasValidExifTimestamp || isFilenamePrefixCorrect(exif1.datetimeOriginal, fnp1 )) )
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyIncorrectFilenamePrefixVsFiletime && (!fnp1.hasValidFilenamePrefix || isFilenamePrefixCorrect(fnp1.imageFiletime, fnp1 )))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyCorrectFiletimeVsExifTime && !(!exif1.hasValidExifTimestamp || isFiletimeCorrect(exif1, fnp1)))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyCorrectFilenamePrefixVsExifTime && !(!fnp1.hasValidFilenamePrefix || !exif1.hasValidExifTimestamp || isFilenamePrefixCorrect(exif1.datetimeOriginal, fnp1 )))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowOnlyCorrectFilenamePrefixVsFiletime && !(!fnp1.hasValidFilenamePrefix || isFilenamePrefixCorrect(fnp1.imageFiletime, fnp1 )))
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowMalformedFilenamePrefix && !fnp1.hasMalformedFilenamePrefix())
            {
                numberOfFilesFiltered++;
                return;
            }
            if (fShowMissingFilenamePrefix && (!exif1.hasValidExifTimestamp || fnp1.filenameStartsWithYYYY || fnp1.hasValidFilenamePrefix))
            {
                numberOfFilesFiltered++;
                return;
            }
            numberOfFilesShown++;

            ////////////////////////////////
            // Print result to console
            ////////////////////////////////
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
                double megapixels = (double)(exif1.pixels)/(1024*1024);
                Console.WriteLine(  "Ma:" + Truncate(exif1.cameraMake,22) + "\t" +
                                    "Mo:" + Truncate(exif1.cameraModel,22) + "\t" +
                                    "HxW:" + exif1.imageHeight + "x" + exif1.imageWidth + "\t" + 
                                    "MP:" + String.Format("{0,8:0.0000}", megapixels) + "\t" + 
                                    "Ex:" + Truncate(exif1.datetimeOriginalString,15) + "\t" + 
                                    "Ft:" + Truncate(fnp1.imageFiletimeString,15) + "\t" + 
                                    "Fp:" + Truncate(fnp1.imageFilenamePrefixString,15) + "\t" + 
                                    "Fs:" + fnp1.imageFilesize + "\t" + 
                                    "Fn:" + fnp1.imageFilename + "\t" + 
                                    "Fn:" + fnp1.fullyQualifiedFilename );
            }

            ////////////////////////////////
            // Perform any actions
            ////////////////////////////////
            if ( fActionSetFiletimeFromExifTime &&
                 exif1.hasValidExifTimestamp &&
                 isFiletimeCorrect(exif1, fnp1) == false &&
                 File.Exists(fnp1.fullyQualifiedFilename) ) 
            {
                if (fDummyRun)
                {
                    Console.WriteLine($"DUMMY: SetLastWriteTime({fnp1.fullyQualifiedFilename}, {exif1.datetimeOriginalString}) (was {fnp1.imageFiletimeString})");
                }
                else
                { 
                    Console.WriteLine($"ACTION: SetLastWriteTime({fnp1.fullyQualifiedFilename}, {exif1.datetimeOriginalString}) (was {fnp1.imageFiletimeString})");
                    File.SetLastWriteTime(fnp1.fullyQualifiedFilename, (DateTime)exif1.datetimeOriginal);
                }
                numberOfFilesActioned++;
            }
            if (fActionAddMissingFilenamePrefix &&
                (fnp1.filenameStartsWithYYYY == false) &&
                (fnp1.hasValidFilenamePrefix == false) &&
                (exif1.hasValidExifTimestamp == true) &&
                File.Exists(fnp1.fullyQualifiedFilename) )
            {
                string newFilename = FilenamePrefix.ModifyFilenameWithPrefixAndSuffix(fnp1.fullyQualifiedFilename, (exif1.datetimeOriginalString + "_"), null);
                if (fDummyRun)
                {
                    Console.WriteLine($"DUMMY: AddFilenamePrefix({fnp1.fullyQualifiedFilename}, {exif1.datetimeOriginalString}) ==> {newFilename}");
                }
                else
                { 
                    Console.WriteLine($"ACTION: AddFilenamePrefix({fnp1.fullyQualifiedFilename}, {exif1.datetimeOriginalString}) ==> {newFilename}");
                    try
                    {
                        System.IO.File.Move(fnp1.fullyQualifiedFilename, newFilename);
                    }
                    catch(System.IO.IOException ex)
                    {
                        // Cannot create a file when that file already exists.
                        Console.WriteLine($"ERROR: Unable to rename/move file ({fnp1.fullyQualifiedFilename} ==> {newFilename}) (already exists?): {ex}");
                        // Keep going.
                    }
                }
                numberOfFilesActioned++;
            }
            if ( fActionFixMalformedFilenamePrefix &&
                (fnp1.filenameStartsWithYYYY == true) &&
                (fnp1.hasValidFilenamePrefix == false) &&
                File.Exists(fnp1.fullyQualifiedFilename) )
            {
                //if (fnp1.imageFilesize == 5419638)
                //    Console.WriteLine($"Breakpoint");

                string newfullyQualifiedFilename = FilenamePrefix.RepairFilenamePrefix(fnp1.fullyQualifiedFilename);
                if (fDummyRun)
                {
                    Console.WriteLine($"DUMMY: FixFilenamePrefix({fnp1.fullyQualifiedFilename}) ==> {newfullyQualifiedFilename}");
                }
                else
                { 
                    Console.WriteLine($"ACTION: FixFilenamePrefix({fnp1.fullyQualifiedFilename}) ==> {newfullyQualifiedFilename}");
                    try
                    {
                        System.IO.File.Move(fnp1.fullyQualifiedFilename, newfullyQualifiedFilename);
                    }
                    catch(System.IO.IOException ex)
                    {
                        // Cannot create a file when that file already exists.
                        Console.WriteLine($"ERROR: Unable to rename/move file ({fnp1.fullyQualifiedFilename} ==> {newfullyQualifiedFilename}) (already exists?): {ex}");
                        // Keep going.
                    }
                }
                numberOfFilesActioned++;
            }
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

        public static bool RoughlyEquals(DateTime time, DateTime timeWithWindow, int windowInSeconds)
        {
            long delta = (long)((TimeSpan)(timeWithWindow - time)).TotalSeconds;
            return Math.Abs(delta) < windowInSeconds;
        }

        public static bool isFiletimeCorrect(ExifData exif1, FilenamePrefix fnp1)
        {
            int window = 10;
            DateTime temp1;

            // If we don't have an exif datetime, then we have no way of knowing that the filetime is correct.
            if (exif1.hasValidExifTimestamp==false) return false;
            if (exif1.datetimeOriginal==null) return false;

            // If the filetime is null, then we have bigger problems
            if (fnp1.imageFiletime==null) return false;

            if (RoughlyEquals((DateTime)exif1.datetimeOriginal, (DateTime)fnp1.imageFiletime, window)) return true;

            temp1 = (DateTime)fnp1.imageFiletime;
            temp1 = temp1.AddHours(1);
            if (RoughlyEquals((DateTime)exif1.datetimeOriginal, temp1, window)) return true;

            temp1 = (DateTime)fnp1.imageFiletime;
            temp1 = temp1.AddHours(-1);
            if (RoughlyEquals((DateTime)exif1.datetimeOriginal, temp1, window)) return true;

            return false;
        }

        public static bool isFilenamePrefixCorrect(DateTime? refDateTime, FilenamePrefix fnp1 )
        {
            // If we don't have a timestamp in the filename, then return false
            if (fnp1.hasValidFilenamePrefix==false) return false;

            // If we don't have a refdatetime, then we do not know if any time in the filename is correct.
            if (refDateTime==null) return false;

            int window = 10;
            DateTime temp1;

            if (RoughlyEquals((DateTime)refDateTime, fnp1.imageFilenamePrefix, window)) return true;

            temp1 = fnp1.imageFilenamePrefix;
            temp1 = temp1.AddHours(1);
            if (RoughlyEquals((DateTime)refDateTime, temp1, window)) return true;

            temp1 = fnp1.imageFilenamePrefix;
            temp1 = temp1.AddHours(-1);
            if (RoughlyEquals((DateTime)refDateTime, temp1, window)) return true;

            return false;
        }

    }

} 

