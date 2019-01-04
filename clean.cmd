REM wipes out all bin and obj folders, which "Clean Solution" annoyingly doesn't do
REM https://stackoverflow.com/a/755433/62600
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"