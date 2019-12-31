@echo off
rem Target Folder (Remember to include/keep trailing backslash)
rem set LOG_DIR=J:\tmp\logs\
set LOG_DIR=%USERPROFILE%\tmp\logs\
rem set LOG_DIR=%USERPROFILE%\Documents\tmp\logs\
mkdir %LOG_DIR%
call :MAKE_FILENAME %LOG_DIR% PhotoFiler .txt
SET LOGFILENAME=%tmp_filename%

@echo on
CALL :FIX c:\Users\Jonnie\Downloads
CALL :FIX c:\Users\Jonnie\Pictures
CALL :FIX c:\home\_Photos\
CALL :FIX d:\home\_Photos\
rem CALL :SHOW c:\Users\Jonnie\Downloads
rem CALL :SHOW c:\Users\Jonnie\Pictures
rem CALL :SHOW c:\home\_Photos\
rem CALL :SHOW d:\home\_Photos\
goto :EOF

:FIX
.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowIncorrectFiletimeVsExifTime --SetFiletimesFromExifTimes  >> %LOGFILENAME%
.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowMissingFilenamePrefix       --AddMissingFilenamePrefix   >> %LOGFILENAME%
goto :EOF

:SHOW
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showbranded                                     
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showunbranded                                   
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showknown                                       
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showunknown                                     
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showfullsize                                    
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --shownotfullsize                                 
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showcorrectfiletimevsexiftime                   
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showcorrectfilenameprefixvsexiftime             
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showcorrectfilenameprefixvsfiletime             
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showincorrectfiletimevsexiftime                 
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showincorrectfilenameprefixvsexiftime           
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showincorrectfilenameprefixvsfiletime           
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showmalformedfilenameprefix                     
@rem .\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --showmissingfilenameprefix                       

.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowIncorrectFiletimeVsExifTime          >> %LOGFILENAME%
.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowIncorrectFilenamePrefixVsExiftime    >> %LOGFILENAME%
.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowIncorrectFilenamePrefixVsFiletime    >> %LOGFILENAME%
.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowMalformedFilenamePrefix              >> %LOGFILENAME%
.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowMissingFilenamePrefix                >> %LOGFILENAME%
.\PhotoFiler\bin\Release\PhotoFiler.exe %1 -r --ShowKnown --ShowNotFullsize                          >> %LOGFILENAME%
goto :EOF

:MAKE_FILENAME
set tmp_path=%1
set tmp_suffix=%2
set tmp_extension=%3

rem --Get datetime in format YYYYMMDD_hhmmss---BEGIN------------------------
rem SET tmp_timestamp=%DATE:~6,4%%DATE:~3,2%%DATE:~0,2%_%TIME:~-11,2%%TIME:~-8,2%%TIME:~-5,2%
set tmp_hour=%TIME:~-11,2%
if "%tmp_hour:~0,1%" == " " set tmp_hour=0%tmp_hour:~1,1%
set tmp_min=%TIME:~-8,2%
if "%tmp_min:~0,1%" == " " set tmp_min=0%tmp_min:~1,1%
set tmp_secs=%TIME:~-5,2%
if "%tmp_secs:~0,1%" == " " set tmp_secs=0%tmp_secs:~1,1%
set tmp_year=%DATE:~6,4%
set tmp_month=%DATE:~3,2%
if "%tmp_month:~0,1%" == " " set tmp_month=0%tmp_month:~1,1%
set tmp_day=%DATE:~0,2%
if "%tmp_day:~0,1%" == " " set tmp_day=0%tmp_day:~1,1%
SET tmp_timestamp=%tmp_year%%tmp_month%%tmp_day%_%tmp_hour%%tmp_min%%tmp_secs%
rem echo tmp_timestamp=%tmp_timestamp%
SET tmp_filename=%tmp_path%%tmp_timestamp%_%COMPUTERNAME%_%tmp_suffix%%tmp_extension%
rem --Get datetime in format YYYYMMDD_hhmmss---END--------------------------

rem Create a random number in the range is [1..100], instead of the default [0..32767].
SET /A tmp_random=%RANDOM%*100/32768

rem If a file of this name already exists, then append a random number.
if exist %tmp_filename% SET tmp_filename=%tmp_path%%tmp_timestamp%_%COMPUTERNAME%_%tmp_suffix%_%tmp_random%%tmp_extension%
goto :EOF
