// PhotoFiler Copyright (c) 2019-2020 Jonathan Gilmore

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
        public static bool fShowSpacesAndHyphens = false;
        public static bool fShowAllProblems = false;
        public static bool fPrintAllColumns = false;

        // Actions
        public static bool fActionSetFiletimeFromExifTime = false;
        public static bool fActionFixMalformedFilenamePrefix = false;
        public static bool fActionAddMissingFilenamePrefix = false;
        public static bool fActionFixFilenamePrefixFromExifTime = false;
        public static bool fActionForceUnderscores = false;
        public static bool fActionFixAllProblems = false;
        //public static bool fActionForceSpacesAndHyphens = false;
        //public static bool fActionFileByDate = false;

        public static bool fPrintMakeAndModel = false;
        public static bool fPrintDimensions = false;
        public static bool fPrintDates = false;
        public static bool fPrintFileDetails = false;

        // Counters
        public static int numberOfFilesProcessed = 0;
        public static int numberOfFilesShown = 0;
        public static int numberOfFilesFiltered = 0;
        public static int numberOfFilesActioned = 0;

#if ARGPROCESSOR

        void argProcessorAdd(string argTagShort, string argTagLong, string argType, bool *pResult, string argDescription)
        {

        }
#endif

        static int Main( string[] args )
        {
            Console.WriteLine("PhotoFiler v1.0.2 Copyright (c) 2019-2020 Jonathan Gilmore");
            // 20200105 v0.92 - Added Nokia N73 to known cameras
            // 20201114 v1.0.1 - Show usage if no runtime params
            // 20201114 v1.0.2 - Add printing only of relevant columns. Modify forceunderscores to also rename all traversed subdirs.
            // 20201114 v1.0.3 - Add PrintAllColumns param, which also reinstitutes tabs between name and value when printing.

            try
            {
                string filePath = "";
                //filePath = "20191130_192659_IMG_5030.JPG";
                //filePath = @"C:\\Users\\Jonnie\\source\\repos\\PhotoFiler\\PhotoFiler\\bin\\Debug";
                //filePath = @"C:/Users/Jonnie/source/repos/PhotoFiler/PhotoFiler/bin/Debug";
#if ARGPROCESSOR
                argProcessorAdd("-t", "--testing"                              , "switch", &fTesting, "Testing (Development/Debugging)");
                argProcessorAdd("-h", "--help"                                 , "switch", &fShowUsage, "");
                argProcessorAdd("-v", "--verbose"                              , "switch", &fVerbose, "");
                argProcessorAdd("-r", "--recursive"                            , "switch", &fRecursive, "");
                argProcessorAdd("-d", "--dummyrun"                             , "switch", &fDummyRun, "");
                argProcessorAdd(null, "--showbranded"                          , "switch", &fShowOnlyBrandedCameras, "");
                argProcessorAdd(null, "--showunbranded"                        , "switch", &fShowOnlyUnbrandedCameras, "");
                argProcessorAdd(null, "--showknown"                            , "switch", &fShowOnlyKnownCameras, "");
                argProcessorAdd(null, "--showunknown"                          , "switch", &fShowOnlyUnknownCameras, "");
                argProcessorAdd(null, "--showfullsize"                         , "switch", &fShowOnlyDimensionsFullsize, "");
                argProcessorAdd(null, "--shownotfullsize"                      , "switch", &fShowOnlyDimensionsNotFullsize, "");
                argProcessorAdd(null, "--showcorrectfiletimevsexiftime"        , "switch", &fShowOnlyCorrectFiletimeVsExifTime, "");
                argProcessorAdd(null, "--showcorrectfilenameprefixvsexiftime"  , "switch", &fShowOnlyCorrectFilenamePrefixVsExifTime, "");
                argProcessorAdd(null, "--showcorrectfilenameprefixvsfiletime"  , "switch", &fShowOnlyCorrectFilenamePrefixVsFiletime, "");
                argProcessorAdd(null, "--showincorrectfiletimevsexiftime"      , "switch", &fShowOnlyIncorrectFiletimeVsExifTime, "");
                argProcessorAdd(null, "--showincorrectfilenameprefixvsexiftime", "switch", &fShowOnlyIncorrectFilenamePrefixVsExifTime, "");
                argProcessorAdd(null, "--showincorrectfilenameprefixvsfiletime", "switch", &fShowOnlyIncorrectFilenamePrefixVsFiletime, "");
                argProcessorAdd(null, "--showmalformedfilenameprefix"          , "switch", &fShowMalformedFilenamePrefix, "");
                argProcessorAdd(null, "--showmissingfilenameprefix"            , "switch", &fShowMissingFilenamePrefix, "");
                argProcessorAdd(null, "--showspacesandhyphens"                 , "switch", &fShowSpacesAndHyphens, "");
                argProcessorAdd(null, "--showallproblems"                      , "switch", &fShowAllProblems, "");
                argProcessorAdd(null, "--printallcolumns"                      , "switch", &fPrintAllColumns, "");
                argProcessorAdd(null, "--setfiletimesfromexiftimes"            , "switch", &fActionSetFiletimeFromExifTime, "");
                argProcessorAdd(null, "--fixmalformedfilenameprefix"           , "switch", &fActionFixMalformedFilenamePrefix, "");
                argProcessorAdd(null, "--addmissingfilenameprefix"             , "switch", &fActionAddMissingFilenamePrefix, "");
                argProcessorAdd(null, "--fixfilenameprefixfromexiftime"        , "switch", &fActionFixFilenamePrefixFromExifTime, "");
                argProcessorAdd(null, "--forceunderscores"                     , "switch", &fActionForceUnderscores, "");
              //argProcessorAdd(null, "--forcespacesandhyphens"                , "switch", &fActionForceSpacesAndHyphens, "");
              //argProcessorAdd(null, "--filebydate"                           , "switch", &fActionFileByDate, "");
                argProcessorAdd(null, "--fixallproblems"                       , "switch", &fActionFixAllProblems, "");
#else


                for (int ii = 0; ii<args.Length; ii++)
                {
                    Console.WriteLine($"args[{ii}]: {args[ii]}");
                    if (args[ii].Substring(0,1) == "-")
                    { 
                        switch (args[ii].ToLower())
                        {
                            case "-t": fTesting = true; break;
                            case "-h": fShowUsage = true; break;
                            case "-v": fVerbose = true; break;
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
                            case "--showspacesandhyphens": fShowSpacesAndHyphens = true; break;
                            case "--showallproblems": fShowAllProblems = true; break;
                            case "--printallcolumns": fPrintAllColumns = true; break;
                            case "--setfiletimesfromexiftimes": fActionSetFiletimeFromExifTime = true; break;
                            case "--fixmalformedfilenameprefix": fActionFixMalformedFilenamePrefix = true; break;
                            case "--addmissingfilenameprefix": fActionAddMissingFilenamePrefix = true; break;
                            case "--fixfilenameprefixfromexiftime": fActionFixFilenamePrefixFromExifTime = true; break;
                            case "--forceunderscores": fActionForceUnderscores = true; break;
                          //case "--forcespacesandhyphens": fActionForceSpacesAndHyphens = true; break;
                          //case "--filebydate": fActionFileByDate = true; break;
                            case "--fixallproblems": fActionFixAllProblems = true; break;
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
                    Console.WriteLine("   --ShowSpacesAndHyphens                  : Process only images that have Spaces or Hyphens in their filename");
                    Console.WriteLine("DateTime Filters:");
                    Console.WriteLine("   --ShowCorrectFiletimeVsExifTime         : Process only images that have a file timestamp that matches Exif date taken");
                    Console.WriteLine("   --ShowCorrectFilenamePrefixVsExifTime   : Process only images that have a filename prefix that matches Exif date taken");
                    Console.WriteLine("   --ShowCorrectFilenamePrefixVsFiletime   : Process only images that have a filename prefix that matches the FS file timestamp");
                    Console.WriteLine("   --ShowIncorrectFiletimeVsExifTime       : Process only images that have a file timestamp that does not match Exif date taken");
                    Console.WriteLine("   --ShowIncorrectFilenamePrefixVsExifTime : Process only images that have a filename prefix that does not match Exif date taken");
                    Console.WriteLine("   --ShowIncorrectFilenamePrefixVsFiletime : Process only images that have a filename prefix that does not match the FS file timestamp");
                    Console.WriteLine("   --ShowMalformedFilenamePrefix           : Process only images that have a filename prefix that is formatted differently to YYYYMMDD_HHMMSS");
                    Console.WriteLine("   --ShowMissingFilenamePrefix             : Process only images that have a no filename prefix, including any malformed prefix");
                    Console.WriteLine("   --ShowAllProblems                       : Process only images that have one or more known problems");
                    
                    Console.WriteLine("Actions:");
                    Console.WriteLine("   --SetFiletimesFromExifTimes             : Set File timestamps from Exif timestamps");
                    Console.WriteLine("   --FixMalformedFilenamePrefix            : Fix Malformed Filename timestamp prefixes");
                    Console.WriteLine("   --AddMissingFilenamePrefix              : Add Filename timestamp prefix if not already present (and not malformed)");
                    Console.WriteLine("   --FixFilenamePrefixFromExifTime         : Set Filename timestamp prefixes from Exif timestamps");
                    Console.WriteLine("   --ForceUnderscores                      : ForceUnderscores");
                    //Console.WriteLine("   --ForceSpacesAndHyphens    : ForceSpacesAndHyphens");
                    //Console.WriteLine("   --FileByDate               : FileByDate");
                    //Console.WriteLine("   --FixAll                   : fActionAddMissingFilenamePrefix, fSetFiletimesFromExifTimes, fActionForceUnderscores");
                    Console.WriteLine("   --FixAllProblems                        : Fix all detected problems");
                    return 0;
                }
#endif
                if (fVerbose)
                {
                    ExifData.fVerbose = true;
                    FilenamePrefix.fVerbose = true;
                }
                if (fShowAllProblems)
                {
                    fShowOnlyUnbrandedCameras = true;
                    fShowOnlyUnknownCameras = true;
                    fShowOnlyDimensionsNotFullsize = true;
                    fShowOnlyCorrectFilenamePrefixVsExifTime = true;
                    fShowOnlyCorrectFilenamePrefixVsFiletime = true;
                    fShowOnlyIncorrectFiletimeVsExifTime = true;
                    fShowOnlyIncorrectFilenamePrefixVsExifTime = true;
                    fShowOnlyIncorrectFilenamePrefixVsFiletime = true;
                    fShowMalformedFilenamePrefix = true;
                    fShowMissingFilenamePrefix = true;
                }
                if (fActionFixAllProblems)
                {
                    fActionSetFiletimeFromExifTime = true;
                    fActionFixMalformedFilenamePrefix = true;
                    fActionAddMissingFilenamePrefix = true;
                    fActionFixFilenamePrefixFromExifTime = true;
                }
                if (fPrintAllColumns)
                {
                    fPrintMakeAndModel = true;
                    fPrintDimensions = true;
                    fPrintDates = true;
                    fPrintFileDetails = true;
                }
                else
                { 
                    if (fShowOnlyUnbrandedCameras) { fPrintMakeAndModel = true; }
                    if (fShowOnlyUnknownCameras) { fPrintMakeAndModel = true; }
                    if (fShowOnlyDimensionsNotFullsize) { fPrintDimensions = true; }
                    if (fShowOnlyCorrectFilenamePrefixVsExifTime) { fPrintDates = true; fPrintFileDetails = true; }
                    if (fShowOnlyCorrectFilenamePrefixVsFiletime) { fPrintDates = true; fPrintFileDetails = true; }
                    if (fShowOnlyIncorrectFiletimeVsExifTime) { fPrintDates = true; }
                    if (fShowOnlyIncorrectFilenamePrefixVsExifTime) { fPrintDates = true; fPrintFileDetails = true; }
                    if (fShowOnlyIncorrectFilenamePrefixVsFiletime) { fPrintDates = true; fPrintFileDetails = true; }
                    if (fShowMalformedFilenamePrefix) { fPrintFileDetails = true; }
                    if (fShowMissingFilenamePrefix) { fPrintFileDetails = true; }


                    if (fActionSetFiletimeFromExifTime) { fPrintDates = true; }
                    if (fActionFixMalformedFilenamePrefix) { fPrintFileDetails = true; }
                    if (fActionAddMissingFilenamePrefix) { fPrintDates = true; fPrintFileDetails = true; }
                    if (fActionFixFilenamePrefixFromExifTime) { fPrintDates = true; fPrintFileDetails = true; }
                    if (fActionForceUnderscores) { fPrintFileDetails = true; }
                    //if (fActionForceSpacesAndHyphens) { }
                    //if (fActionFileByDate) { }
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

        public static string MaybeForceUnderscores(string theThing, bool isFile)
        {
            if (!fActionForceUnderscores)
               return theThing;

            if (isFile)
            {
                if (!File.Exists(theThing))
                    return theThing;
            }
            else // isDirectory
            {
                if (!Directory.Exists(theThing))
                    return theThing;
            }

            string newfullyQualifiedFilename = theThing;
            newfullyQualifiedFilename = FilenamePrefix.ReplaceSubstringInBasename(newfullyQualifiedFilename, "-ish", "_ish");
            newfullyQualifiedFilename = FilenamePrefix.ReplaceSubstringInBasename(newfullyQualifiedFilename, " - ", "__");
            newfullyQualifiedFilename = FilenamePrefix.ReplaceSubstringInBasename(newfullyQualifiedFilename, "-", "__");
            newfullyQualifiedFilename = FilenamePrefix.ReplaceSubstringInBasename(newfullyQualifiedFilename, " ", "_");
            newfullyQualifiedFilename = FilenamePrefix.ReplaceSubstringInBasename(newfullyQualifiedFilename, ".", "_");
            newfullyQualifiedFilename = FilenamePrefix.ReplaceSubstringInBasename(newfullyQualifiedFilename, ",", "_");
            // If a change was indeed made
            if (newfullyQualifiedFilename != theThing)
            {
                if (fDummyRun)
                {
                    Console.WriteLine($"DUMMY: ForceUnderscores({theThing}) ==> {newfullyQualifiedFilename}");
                }
                else
                {
                    Console.WriteLine($"ACTION: ForceUnderscores({theThing}) ==> {newfullyQualifiedFilename}");
                    try
                    {
                        if (isFile)
                            System.IO.File.Move(theThing, newfullyQualifiedFilename);
                        else
                            System.IO.Directory.Move(theThing, newfullyQualifiedFilename);
                    }
                    catch (System.IO.IOException ex)
                    {
                        // Cannot create a file when that file already exists.
                        Console.WriteLine($"ERROR: ForceUnderscores failed to rename/move file/dir ({theThing} ==> {newfullyQualifiedFilename}) (already exists?): {ex}");
                        // Keep going.
                        return theThing;
                    }
                }
                return newfullyQualifiedFilename;
            }
            //fForceSpacesAndHyphens

            return theThing;
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
                        string newName = MaybeForceUnderscores(subDir, false);
                        DirectorySearch(newName, depth+1, recursive);
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
            if (fShowSpacesAndHyphens && !fnp1.filenameContainsSpaceOrHyphen)
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
                string nameValueDelimiter;
                if (fPrintAllColumns)
                    nameValueDelimiter = "\t";
                else
                    nameValueDelimiter = " ";
                Console.Write("\t");
                Console.Write("FILE:" + nameValueDelimiter);
                if (fPrintMakeAndModel)
                { 
                Console.Write("Ma:" + nameValueDelimiter + Truncate(exif1.cameraMake,22) + "\t");
                Console.Write("Mo:" + nameValueDelimiter + Truncate(exif1.cameraModel,22) + "\t");
                }
                if (fPrintDimensions)
                {
                    Console.Write("HxW:" + nameValueDelimiter + exif1.imageHeight + "*" + exif1.imageWidth + "\t");
                    Console.Write("MP:" + nameValueDelimiter + String.Format("{0,8:0.0000}", megapixels) + "\t");
                }
                if (fPrintDates)
                {
                    Console.Write("Ex:" + nameValueDelimiter + Truncate(exif1.datetimeOriginalString,15) + "\t");
                    Console.Write("Ft:" + nameValueDelimiter + Truncate(fnp1.imageFiletimeString,15) + "\t");
                    Console.Write("Fp:" + nameValueDelimiter + Truncate(fnp1.imageFilenamePrefixString,15) + "\t");
                }
                if (fPrintFileDetails)
                {
                    Console.Write("Fs:" + nameValueDelimiter + fnp1.imageFilesize + "\t");
                    Console.Write("Fn:" + nameValueDelimiter + fnp1.imageFilename + "\t");
                }
                Console.Write("Fq:" + nameValueDelimiter + fnp1.fullyQualifiedFilename );
                Console.WriteLine("");
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
                        Console.WriteLine($"ERROR: AddMissingFilenamePrefix failed to rename/move file ({fnp1.fullyQualifiedFilename} ==> {newFilename}) (already exists?): {ex}");
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
                // If a repair was indeed made
                if (newfullyQualifiedFilename != fnp1.fullyQualifiedFilename)
                {
                    if (fDummyRun)
                    {
                        Console.WriteLine($"DUMMY: FixMalformedFilenamePrefix({fnp1.fullyQualifiedFilename}) ==> {newfullyQualifiedFilename}");
                    }
                    else
                    { 
                        Console.WriteLine($"ACTION: FixMalformedFilenamePrefix({fnp1.fullyQualifiedFilename}) ==> {newfullyQualifiedFilename}");
                        try
                        {
                            System.IO.File.Move(fnp1.fullyQualifiedFilename, newfullyQualifiedFilename);
                        }
                        catch(System.IO.IOException ex)
                        {
                            // Cannot create a file when that file already exists.
                            Console.WriteLine($"ERROR: FixMalformedFilenamePrefix failed to rename/move file ({fnp1.fullyQualifiedFilename} ==> {newfullyQualifiedFilename}) (already exists?): {ex}");
                            // Keep going.
                        }
                    }
                    numberOfFilesActioned++;
                }
            }
            
            if ( fActionFixFilenamePrefixFromExifTime &&
                 exif1.hasValidExifTimestamp &&
                 fnp1.hasValidFilenamePrefix &&
                 (fnp1.imageFilenamePrefixString != exif1.datetimeOriginalString) &&
                 File.Exists(fnp1.fullyQualifiedFilename) ) 
            {
                string newfullyQualifiedFilename = FilenamePrefix.ReplaceSubstringInBasename(fnp1.fullyQualifiedFilename,fnp1.imageFilenamePrefixString, exif1.datetimeOriginalString);
                // If a change was indeed made
                if (newfullyQualifiedFilename != fnp1.fullyQualifiedFilename)
                {
                    if (fDummyRun)
                    {
                        Console.WriteLine($"DUMMY: FixFilenamePrefixFromExifTime({fnp1.fullyQualifiedFilename}) ==> {newfullyQualifiedFilename}");
                    }
                    else
                    { 
                        Console.WriteLine($"ACTION: FixFilenamePrefixFromExifTime({fnp1.fullyQualifiedFilename}) ==> {newfullyQualifiedFilename}");
                        try
                        {
                            System.IO.File.Move(fnp1.fullyQualifiedFilename, newfullyQualifiedFilename);
                        }
                        catch(System.IO.IOException ex)
                        {
                            // Cannot create a file when that file already exists.
                            Console.WriteLine($"ERROR: FixFilenamePrefixFromExifTime failed to rename/move file ({fnp1.fullyQualifiedFilename} ==> {newfullyQualifiedFilename}) (already exists?): {ex}");
                            // Keep going.
                        }
                    }
                    numberOfFilesActioned++;
                }
            }
            string newName = MaybeForceUnderscores(fnp1.fullyQualifiedFilename, true);
            if (newName != fnp1.fullyQualifiedFilename)
            {
                fnp1.fullyQualifiedFilename = newName;
                numberOfFilesActioned++;
            }
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

            if (make == "Eastman Kodak Company"   && model == "DC210 Zoom (V05.00)") return true;  // First digital family camera
            if (make == "FUJIFILM"                && model == "FinePix A610"       ) return true;  // Old trusty family camera
            if (make == "Vivitar"                 && model == "ViviCam 3835"       ) return true;  // Clemy's orange camera?
            if (make == "Panasonic"               && model == "NV-GS120"           ) return true;  // DV Video Camera ?
            if (make == "Nokia"                   && model == "N73"                ) return true;  // Nokia N73
            if (make == "Nokia"                   && model == "E90"                ) return true;  // Nokia E90 Communicator mobile phone
            if (make == "SAMSUNG"                 && model == "GT-S7275R"          ) return true;  // Samsung Galaxy Ace3 mobile phone ?
            if (make == "SAMSUNG"                 && model == "GT-I8160"           ) return true;  // Samsung Galaxy Ace 2 mobile phone
            if (make == "Sony"                    && model == "E6653"              ) return true;  // Sony Z5 mobile phone
            if (make == "Panasonic"               && model == "DMC-TZ1"            ) return true;  // Panasonic DMC-TZ1
            if (make == "Canon"                   && model == "Canon EOS 500D"     ) return true;
            if (make == "Canon"                   && model == "Canon EOS 750D"     ) return true;
            if (make == "Google"                  && model == "Pixel 3a"           ) return true;  // Google Pixel 3a mobile phone
          //if (make == "Canon"                   && model == "MG4200 series"      ) return true;  // Scanner ? // Date of scan is not too useful
            return false;
        }

        public static bool isFullSizeImage (string make, string model, long pixels)
        {
            make = make.Trim();
            model = model.Trim();

            if (make == "Canon"                   && model == "Canon EOS 500D"      && pixels == (3000*2000) ) return true;   // MP:  5.7220
            if (make == "Canon"                   && model == "Canon EOS 500D"      && pixels == (4752*3168) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 500D"      && pixels == (4898*3265) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 750D"      && pixels == (6000*4000) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 750D"      && pixels == (6000*3368) ) return true;
            if (make == "Canon"                   && model == "Canon EOS 750D"      && pixels == (5328*4000) ) return true;
            if (make == "Eastman Kodak Company"   && model == "DC210 Zoom (V05.00)" && pixels == (1152* 864) ) return true;
            if (make == "FUJIFILM"                && model == "FinePix A610"        && pixels == (2848*2136) ) return true;
            if (make == "Google"                  && model == "Pixel 3a"            && pixels == (4032*3024) ) return true;
            if (make == "Google"                  && model == "Pixel 3a"            && pixels == (4000*2000) ) return true;   // MP:  7.6294
            if (make == "Google"                  && model == "Pixel 3a"            && pixels == (3264*2448) ) return true;
            if (make == "Nokia"                   && model == "E90"                 && pixels == (2048*1536) ) return true;   // MP:  3.0000
            if (make == "Nokia"                   && model == "N73"                 && pixels == (2048*1536) ) return true;   // MP:  3.0000
            if (make == "Panasonic"               && model == "DMC-TZ1"             && pixels == (2560*1920) ) return true;
            if (make == "Panasonic"               && model == "DMC-TZ1"             && pixels == (2048*1536) ) return true;
            if (make == "Panasonic"               && model == "NV-GS120"            && pixels == (1520*1152) ) return true;
            if (make == "SAMSUNG"                 && model == "GT-I8160"            && pixels == (2560*1920) ) return true;   // MP:  4.6875
            if (make == "SAMSUNG"                 && model == "GT-I8160"            && pixels == (2592*1944) ) return true;
            if (make == "SAMSUNG"                 && model == "GT-S7275R"           && pixels == (2560*1536) ) return true;   // MP:  3.7500
            if (make == "SAMSUNG"                 && model == "GT-S7275R"           && pixels == (2560*1920) ) return true;
            if (make == "Sony"                    && model == "E6653"               && pixels == (2592*1458) ) return true;
            if (make == "Sony"                    && model == "E6653"               && pixels == (5333*3000) ) return true;   // MP: 15.2578
            if (make == "Sony"                    && model == "E6653"               && pixels == (4618*3464) ) return true;   // MP: 15.2557
            if (make == "Sony"                    && model == "E6653"               && pixels == (3840*2160) ) return true;   // MP:  7.9102
            if (make == "Sony"                    && model == "E6653"               && pixels == (5520*4140) ) return true;   // MP: 21.7941
            if (make == "Sony"                    && model == "E6653"               && pixels == (5984*3366) ) return true;
            if (make == "Vivitar"                 && model == "ViviCam 3835"        && pixels == (1280* 960) ) return true;   // MP:  1.1719
            if (make == "Vivitar"                 && model == "ViviCam 3835"        && pixels == (1600*1200) ) return true;
            if (make == "Vivitar"                 && model == "ViviCam 3835"        && pixels == (2272*1704) ) return true;   // MP:  3.6921
          //if (make == "Canon"                   && model == "MG4200 series"       && pixels == (2096*1500) ) return true;   // Date of scan is not too useful

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

