set basepath=%~dp0
set oldname=%1
set newname=%oldname:.apk=%_zipalign.apk
%basepath%zipalign.exe -v 4 %oldname% %newname%