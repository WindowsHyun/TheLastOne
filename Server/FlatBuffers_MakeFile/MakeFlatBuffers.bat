@echo off
title FlatBuffers Make By.WindowsHyun
set fbsFile=None
set menunr=GARBAGE
echo  ---------------------------------------------------------------------------------------------
echo FlatBuffers 1.7.1 ¡æ C++ or C#
echo  ---------------------------------------------------------------------------------------------
echo ¡Ø How to use
echo 	1. Put the fbs file in the / input_fbs folder.
echo 	2. Select the language you want to convert.
echo 	3. Complete
echo  =============================================================================================
echo  ^| Current-fbs: FlatBuffers.fbs ^|
echo  ---------------------------------------------------------------------------------------------
echo  1    C++ conversion
echo  2    C# conversion
echo  3    Java conversion
echo  4    Go conversion
echo  5    Python conversion
echo  6    Javascript conversion [Error]
echo  7    PHP conversion
echo  8    Json conversion
echo  9    All conversion
echo  10   C++ And C# conversion
echo  0    Quit                                                             FIX UPDATE : 2017-08-14
echo  =============================================================================================
SET /P menunr=Please make your decision:
IF %menunr% == 0 (
echo Bye~
pause
exit
)
IF %menunr% == 1 (
flatc --cpp -o ./output_file/Cpp ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 2 (
flatc --csharp -o ./output_file/CSharp ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 3 (
flatc --java -o ./output_file/Java ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 4 (
flatc --go -o ./output_file/Go ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 5 (
flatc --python -o ./output_file/Python ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 6 (
flatc --javascript -o ./output_file/Javascript ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 7 (
flatc --php -o ./output_file/PHP ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 8 (
flatc --json -o ./output_file/Json ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 9 (
flatc --cpp -o ./output_file/Cpp ./input_fbs/FlatBuffers.fbs
flatc --csharp -o ./output_file/CSharp ./input_fbs/FlatBuffers.fbs
flatc --java -o ./output_file/Java ./input_fbs/FlatBuffers.fbs
flatc --go -o ./output_file/Go ./input_fbs/FlatBuffers.fbs
flatc --python -o ./output_file/Python ./input_fbs/FlatBuffers.fbs
flatc --javascript -o ./output_file/Javascript ./input_fbs/FlatBuffers.fbs
flatc --php -o ./output_file/PHP ./input_fbs/FlatBuffers.fbs
flatc --json -o ./output_file/Json ./input_fbs/FlatBuffers.fbs
pause
exit
)
IF %menunr% == 10 (
flatc --cpp -o ./output_file/Cpp ./input_fbs/FlatBuffers.fbs
flatc --csharp -o ./output_file/CSharp ./input_fbs/FlatBuffers.fbs
pause
exit
)
echo Conversion succeeded..!
echo Thank You~
PAUSE