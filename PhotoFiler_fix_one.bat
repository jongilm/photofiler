@echo off
rem Target Folder (Remember to include/keep trailing backslash)
rem set LOG_DIR=J:\tmp\logs\
set LOG_DIR=%USERPROFILE%\tmp\logs\
rem set LOG_DIR=%USERPROFILE%\Documents\tmp\logs\
if not exist %LOG_DIR% mkdir %LOG_DIR%

set PHOTOFILER_EXE=c:\Users\Jonnie\source\repos\PhotoFiler\bin\Release\PhotoFiler.exe

@echo on
call :MAKE_FILENAME %LOG_DIR% PhotoFiler .txt
SET LOGFILENAME=%tmp_filename%
CALL :SHOW "%1"
CALL :FIX "%1"
CALL :SHOW "%1"
goto :EOF

:FIX
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowSpacesAndHyphens                  --ForceUnderscores              >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowMissingFilenamePrefix             --AddMissingFilenamePrefix      >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowMalformedFilenamePrefix           --FixMalformedFilenamePrefix    >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowIncorrectFilenamePrefixVsExiftime --FixFilenamePrefixFromExifTime >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowIncorrectFiletimeVsExifTime       --SetFiletimesFromExifTimes     >> %LOGFILENAME%
goto :EOF

:SHOW
@rem %PHOTOFILER_EXE% %1 -r --showbranded                                     
@rem %PHOTOFILER_EXE% %1 -r --showunbranded                                   
@rem %PHOTOFILER_EXE% %1 -r --showknown                                       
@rem %PHOTOFILER_EXE% %1 -r --showunknown                                     
@rem %PHOTOFILER_EXE% %1 -r --showfullsize                                    
@rem %PHOTOFILER_EXE% %1 -r --shownotfullsize                                 
@rem %PHOTOFILER_EXE% %1 -r --showcorrectfiletimevsexiftime                   
@rem %PHOTOFILER_EXE% %1 -r --showcorrectfilenameprefixvsexiftime             
@rem %PHOTOFILER_EXE% %1 -r --showcorrectfilenameprefixvsfiletime             
@rem %PHOTOFILER_EXE% %1 -r --showincorrectfiletimevsexiftime                 
@rem %PHOTOFILER_EXE% %1 -r --showincorrectfilenameprefixvsexiftime           
@rem %PHOTOFILER_EXE% %1 -r --showincorrectfilenameprefixvsfiletime           
@rem %PHOTOFILER_EXE% %1 -r --showmalformedfilenameprefix                     
@rem %PHOTOFILER_EXE% %1 -r --showmissingfilenameprefix                       

rem ***************************************************************************************************************************************************
rem * PhotoFiler v0.91 Copyright (c) 2019-2020 Jonathan Gilmore
rem * args[0]: -h
rem * Usage: PhotoFiler [options] <PathToPhotos>
rem * Options:
rem *    -h                         : Show Usage
rem *    -v                         : Verbose
rem *    -r                         : Recursive
rem *    -d                         : Dummy Run
rem * Filters:
rem *    --ShowBranded                           : Process only images that have Camera Make and Model in the EXIF metadata
rem *    --ShowUnbranded                         : Process only images that DO NOT have Camera Make and Model in the EXIF metadata
rem *    --ShowKnown                             : Process all images taken with one of MY Cameras
rem *    --ShowUnknown                           : Process all images except those taken with one MY Cameras
rem *    --ShowFullsize                          : Process only images that appear to have their original dimensions
rem *    --ShowNotFullsize                       : Process only images that appear to have been cropped or resized
rem *    --ShowSpacesAndHyphens                  : Process only images that have Spaces or Hyphens in their filename
rem * DateTime Filters:
rem *    --ShowCorrectFiletimeVsExifTime         : Process only images that have a file timestamp that matches Exif date taken
rem *    --ShowCorrectFilenamePrefixVsExifTime   : Process only images that have a filename prefix that matches Exif date taken
rem *    --ShowCorrectFilenamePrefixVsFiletime   : Process only images that have a filename prefix that matches the FS file timestamp
rem *    --ShowIncorrectFiletimeVsExifTime       : Process only images that have a file timestamp that does not match Exif date taken
rem *    --ShowIncorrectFilenamePrefixVsExifTime : Process only images that have a filename prefix that does not match Exif date taken
rem *    --ShowIncorrectFilenamePrefixVsFiletime : Process only images that have a filename prefix that does not match the FS file timestamp
rem *    --ShowMalformedFilenamePrefix           : Process only images that have a filename prefix that is formatted differently to YYYYMMDD_HHMMSS
rem *    --ShowMissingFilenamePrefix             : Process only images that have a no filename prefix, including any malformed prefix
rem * Actions:
rem *    --SetFiletimesFromExifTimes             : Set File timestamps from Exif timestamps
rem *    --FixMalformedFilenamePrefix            : Fix Malformed Filename timestamp prefixes
rem *    --AddMissingFilenamePrefix              : Add Filename timestamp prefix if not already present (and not malformed)
rem *    --FixFilenamePrefixFromExifTime         : Set Filename timestamp prefixes from Exif timestamps
rem *    --ForceUnderscores                      : ForceUnderscores
rem ***************************************************************************************************************************************************
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowIncorrectFiletimeVsExifTime          >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowIncorrectFilenamePrefixVsExiftime    >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowIncorrectFilenamePrefixVsFiletime    >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowMissingFilenamePrefix                >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowMalformedFilenamePrefix              >> %LOGFILENAME%
%PHOTOFILER_EXE% %1 -r --ShowKnown --ShowSpacesAndHyphens                     >> %LOGFILENAME%
rem %PHOTOFILER_EXE% %1 -r --ShowKnown --ShowNotFullsize                          >> %LOGFILENAME%
goto :EOF

:MAKE_FILENAME
@set tmp_path=%1
@set tmp_suffix=%2
@set tmp_extension=%3
@
@rem --Get datetime in format YYYYMMDD_hhmmss---BEGIN------------------------
@rem SET tmp_timestamp=%DATE:~6,4%%DATE:~3,2%%DATE:~0,2%_%TIME:~-11,2%%TIME:~-8,2%%TIME:~-5,2%
@set tmp_hour=%TIME:~-11,2%
@if "%tmp_hour:~0,1%" == " " set tmp_hour=0%tmp_hour:~1,1%
@set tmp_min=%TIME:~-8,2%
@if "%tmp_min:~0,1%" == " " set tmp_min=0%tmp_min:~1,1%
@set tmp_secs=%TIME:~-5,2%
@if "%tmp_secs:~0,1%" == " " set tmp_secs=0%tmp_secs:~1,1%
@set tmp_year=%DATE:~6,4%
@set tmp_month=%DATE:~3,2%
@if "%tmp_month:~0,1%" == " " set tmp_month=0%tmp_month:~1,1%
@set tmp_day=%DATE:~0,2%
@if "%tmp_day:~0,1%" == " " set tmp_day=0%tmp_day:~1,1%
@SET tmp_timestamp=%tmp_year%%tmp_month%%tmp_day%_%tmp_hour%%tmp_min%%tmp_secs%
@rem echo tmp_timestamp=%tmp_timestamp%
@SET tmp_filename=%tmp_path%%tmp_timestamp%_%COMPUTERNAME%_%tmp_suffix%%tmp_extension%
@rem --Get datetime in format YYYYMMDD_hhmmss---END--------------------------
@
@rem Create a random number in the range is [1..100], instead of the default [0..32767].
@SET /A tmp_random=%RANDOM%*100/32768
@
@rem If a file of this name already exists, then append a random number.
@if exist %tmp_filename% SET tmp_filename=%tmp_path%%tmp_timestamp%_%COMPUTERNAME%_%tmp_suffix%_%tmp_random%%tmp_extension%
@goto :EOF
