@echo off

echo Creating archive...

powershell -Command ^
"Compress-Archive -Path 'BOTB64.Client\data\*' -DestinationPath 'BOTB64.Client\data.zip' -Force"

echo Encrypting...

BOTB64.Packer\bin\Release\net9.0\BOTB64.Packer.exe

copy /Y "BOTB64.Client\data.b64" "BOTB64.Client\bin\Release\net8.0\"
copy /Y "BOTB64.Client\data.b64" "BOTB64.Client\bin\Debug\net8.0\"
del "BOTB64.Client\data.b64"
del "BOTB64.Client\data.zip"
del "BOTB64.Client\normalized.zip"

echo Done.
pause