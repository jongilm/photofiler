# PhotoFiler

Copyright (c) 2019-2020 Jonathan Gilmore

------------------------------------------------------------
## Overview

Windows Command line utility to analyse and optionally fix the various forms of image/photo metadata.
Specifically, it is most interested in:
* EXIF Make & Model,
* Exif width and height,
* Exif taken datetime,
* File system datetime of last write,
* File system filename and filepath.

Also, processing can be limited to camera known to me (or unknown). This is currently a hard coded list, but ultimately must be implemented in a config file.

------------------------------------------------------------
## Usage

As at v0.92...

Usage: PhotoFiler [options] <PathToPhotos>
Options:
   -h                         : Show Usage
   -v                         : Verbose
   -r                         : Recursive
   -d                         : Dummy Run
Filters:
   --ShowBranded                           : Process only images that have Camera Make and Model in the EXIF metadata
   --ShowUnbranded                         : Process only images that DO NOT have Camera Make and Model in the EXIF metadata
   --ShowKnown                             : Process all images taken with one of MY Cameras
   --ShowUnknown                           : Process all images except those taken with one MY Cameras
   --ShowFullsize                          : Process only images that appear to have their original dimensions
   --ShowNotFullsize                       : Process only images that appear to have been cropped or resized
   --ShowSpacesAndHyphens                  : Process only images that have Spaces or Hyphens in their filename
DateTime Filters:
   --ShowCorrectFiletimeVsExifTime         : Process only images that have a file timestamp that matches Exif date taken
   --ShowCorrectFilenamePrefixVsExifTime   : Process only images that have a filename prefix that matches Exif date taken
   --ShowCorrectFilenamePrefixVsFiletime   : Process only images that have a filename prefix that matches the FS file timestamp
   --ShowIncorrectFiletimeVsExifTime       : Process only images that have a file timestamp that does not match Exif date taken
   --ShowIncorrectFilenamePrefixVsExifTime : Process only images that have a filename prefix that does not match Exif date taken
   --ShowIncorrectFilenamePrefixVsFiletime : Process only images that have a filename prefix that does not match the FS file timestamp
   --ShowMalformedFilenamePrefix           : Process only images that have a filename prefix that is formatted differently to YYYYMMDD_HHMMSS
   --ShowMissingFilenamePrefix             : Process only images that have a no filename prefix, including any malformed prefix
Actions:
   --SetFiletimesFromExifTimes             : Set File timestamps from Exif timestamps
   --FixMalformedFilenamePrefix            : Fix Malformed Filename timestamp prefixes
   --AddMissingFilenamePrefix              : Add Filename timestamp prefix if not already present (and not malformed)
   --FixFilenamePrefixFromExifTime         : Set Filename timestamp prefixes from Exif timestamps
   --ForceUnderscores                      : ForceUnderscores

------------------------------------------------------------
## Contact

If you like it or find any bugs or would like to see some additional features, please do contact me at jonathan@jgilmore.eu
