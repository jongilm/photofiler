// PhotoFiler Copyright (c) 2019-2020 Jonathan Gilmore

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// https://github.com/drewnoakes/metadata-extractor/wiki/GettingStarted
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jpeg;

#pragma warning disable 8321



namespace PhotoFiler
{
    internal class ExifData
    {
        // [JPEG] Image Height - 4000 pixels
        // [JPEG] Image Width - 6000 pixels
        // [Exif IFD0] Make - Canon
        // [Exif IFD0] Model - Canon EOS 750D
        // [Exif IFD0] Orientation - Top, left side (Horizontal / normal)
        // [Exif IFD0] Date/Time - 2019:11:30 19:26:59
        // [Exif SubIFD] Date/Time Original - 2019:11:30 19:26:59
        // [Exif SubIFD] Exif Image Width - 6000 pixels
        // [Exif SubIFD] Exif Image Height - 4000 pixels
        // [Exif SubIFD] Camera Owner Name -
        // [Exif SubIFD] Body Serial Number - 283072008309
        // [Exif SubIFD] Lens Model - TAMRON 18-200mm F/3.5-6.3 DiII VC B018
        // [Exif SubIFD] Lens Serial Number - 0000000000
        // [File Type] Detected File Type Name - JPEG
        // [File Type] Expected File Name Extension - jpg
        // [File] File Name - 20191130_192659_IMG_5030.JPG
        // [File] File Size - 8434775 bytes

        public static bool fVerbose = false;

        public string filePath { get; set; }

        // Attributes
        public int imageWidth { get; set; }
        public int imageHeight { get; set; }
        public string imageWidthDescription { get; set; }
        public string imageHeightDescription { get; set; }
        public string cameraMake { get; set; }
        public string cameraModel { get; set; }
        public string lensMake { get; set; }
        public string lensModel { get; set; }
        public System.DateTime? datetimeOriginal { get; set; }
        public string datetimeOriginalString { get; set; }
        public bool hasValidExifTimestamp { get; set; }
        public long pixels { get; set; }
        

        public ExifData(string filename)
        {
            filePath = filename;

            //Console.WriteLine($"Processing file: {filePath}");

            try
            {
                // This will transparently determine the file type and invoke the appropriate
                // readers. This will handle JPEG, TIFF, GIF, BMP and RAW (CRW/CR2/NEF/RW2/ORF) files
                // and extract whatever metadata is available and understood.
                var directories = ImageMetadataReader.ReadMetadata(filePath);
                if (directories == null) return;

                //PrintListOfDirectories(directories);
                //PrintAllTags(directories, "Using ImageMetadataReader");
                GetPhotoMetadata(directories);
            }
            catch (ImageProcessingException e)
            {
                PrintError(e);
            }
            catch (IOException e)
            {
                PrintError(e);
            }

        }

        void GetPhotoMetadata(IEnumerable<MetadataExtractor.Directory> directories)
        {
            imageHeight = this.GetImageHeight(directories);
            imageWidth = this.GetImageWidth(directories);
            pixels = (imageHeight * imageWidth);
            datetimeOriginal = this.GetTakenDateTime(directories);
            if (datetimeOriginal != null)
            {
                hasValidExifTimestamp = true;
                datetimeOriginalString = ((DateTime)datetimeOriginal).ToString("yyyyMMdd'_'HHmmss");
            }
            else
            { 
                hasValidExifTimestamp = false;
                datetimeOriginalString = "";
            }

            cameraMake = this.GetCameraMake(directories);
            cameraModel = this.GetCameraModel(directories);

            //lensMake = this.GetLensMake(directories);
            //lensModel = this.GetLensModel(directories);
        }

        public bool isBranded()
        {
            return (cameraMake.Length > 0 && cameraModel.Length > 0);
        }


        int GetImageWidth(IEnumerable<MetadataExtractor.Directory> directories)
        {
            int temp = 0;
            // Obtain a specific directory
            var directory = directories.OfType<JpegDirectory>().FirstOrDefault();
            if (directory == null) return 0;
            temp = directory.GetImageWidth();
            return temp;
        }

        int GetImageHeight(IEnumerable<MetadataExtractor.Directory> directories)
        {
            int temp = 0;
            // Obtain a specific directory
            var directory = directories.OfType<JpegDirectory>().FirstOrDefault();
            if (directory == null) return 0;
            temp = directory.GetImageHeight();
            return temp;
        }

        string GetImageWidthDescr(IEnumerable<MetadataExtractor.Directory> directories)
        {
            string temp = "";
            // Obtain a specific directory
            var directory = directories.OfType<JpegDirectory>().FirstOrDefault();
            if (directory == null) return "";

            // Create a descriptor
            var descriptor = new JpegDescriptor(directory);
            if (descriptor == null) return "";
            // Get tag description
            temp = descriptor.GetImageHeightDescription();
            if (temp == null) return "";
            return temp;
        }

        string GetImageHeightDescr(IEnumerable<MetadataExtractor.Directory> directories)
        {
            string temp = "";
            // Obtain a specific directory
            var directory = directories.OfType<JpegDirectory>().FirstOrDefault();
            if (directory == null) return "";
            // Create a descriptor
            var descriptor = new JpegDescriptor(directory);
            if (descriptor == null) return "";
            // Get tag description
            temp = descriptor.GetImageHeightDescription();
            if (temp == null) return "";
            return temp;
        }

        string GetCameraMake(IEnumerable<MetadataExtractor.Directory> directories)
        {
            int tagValue = ExifDirectoryBase.TagMake;
            var directory = directories.OfType<ExifDirectoryBase>().FirstOrDefault();
            if (directory == null) return "";
            StringValue stringval = directory.GetStringValue(tagValue);
            if (stringval.Bytes == null) return "";
            string result = stringval.ToString();
            if (result == null) return "";
            return result;
        }

        string GetCameraModel(IEnumerable<MetadataExtractor.Directory> directories)
        {
            int tagValue = ExifDirectoryBase.TagModel;
            var directory = directories.OfType<ExifDirectoryBase>().FirstOrDefault();
            if (directory == null) return "";
            StringValue stringval = directory.GetStringValue(tagValue);
            if (stringval.Bytes == null) return "";
            string result = stringval.ToString();
            if (result == null) return "";
            return result;
        }

        string GetLensMake(IEnumerable<MetadataExtractor.Directory> directories)
        {
            int tagValue = ExifDirectoryBase.TagLensMake;
            var directory = directories.OfType<ExifDirectoryBase>().FirstOrDefault();
            if (directory == null) return "";
            StringValue stringval = directory.GetStringValue(tagValue);
            if (stringval.Bytes == null) return "";
            string result = stringval.ToString();
            if (result == null) return "";
            return result;
        }

        string GetLensModel(IEnumerable<MetadataExtractor.Directory> directories)
        {
            int tagValue = ExifDirectoryBase.TagLensModel;
            var directory = directories.OfType<ExifDirectoryBase>().FirstOrDefault();
            if (directory == null) return "";
            StringValue stringval = directory.GetStringValue(tagValue);
            if (stringval.Bytes == null) return "";
            string result = stringval.ToString();
            if (result == null) return "";
            return result;
        }

        DateTime? GetTakenDateTime(IEnumerable<MetadataExtractor.Directory> directories)
        {
            // Obtain the Exif SubIFD directory
            var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory == null) return null;
            // Query the tag's value
            if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
                return dateTime;
            return null;
        }

        string GetExposureProgramDescription(IEnumerable<MetadataExtractor.Directory> directories)
        {
            // Obtain a specific directory
            var directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory == null) return null;
            // Create a descriptor
            var descriptor = new ExifSubIfdDescriptor(directory);
            if (descriptor == null) return null;
            // Get tag description
            return descriptor.GetExposureProgramDescription();
        }


        void PrintListOfDirectories(IEnumerable<MetadataExtractor.Directory> directories)
        {
            // Extraction gives us potentially many directories
            foreach (var directory in directories)
            {
                Console.WriteLine("   directory.Name: " + directory.Name + " ***");
                // Each directory may also contain error messages
                foreach (var error in directory.Errors)
                {
                    Console.Error.WriteLine("   ERROR: " + error);
                }
            }
        }

        // Write all extracted values to stdout
        void PrintAllTags(IEnumerable<MetadataExtractor.Directory> directories, string method)
        {
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------");
            Console.Write(' '); Console.WriteLine(method);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine();

            // Extraction gives us potentially many directories
            foreach (var directory in directories)
            {
                Console.WriteLine("*********************************** directory.Name: " + directory.Name + " ***");
                // Each directory stores values in tags
                foreach (var tag in directory.Tags)
                {
                    Console.WriteLine(tag);
                }

                // Each directory may also contain error messages
                foreach (var error in directory.Errors)
                {
                    Console.Error.WriteLine("ERROR: " + error);
                }
            }
        }

        void PrintError(Exception exception) => Console.Error.WriteLine($"EXCEPTION: {exception}");
    };
}

