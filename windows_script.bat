@echo off

echo Starting Docker container for Microsoft SQL Server...
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyStrongPassword123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest

FOR /F "tokens=*" %%i IN ('docker ps -q --filter ancestor=mcr.microsoft.com/mssql/server:latest') DO SET CONTAINER_ID=%%i
echo Waiting for SQL Server to start up...
timeout /t 20 /nobreak >nul

echo Navigating to the Frontend application directory...
cd Aplikacija\Frontend

echo Installing npm dependencies for the Frontend application...
npm install

echo Creating the .env file with the database server URL...
echo VITE_DB_SERVER=http://localhost:5184 > .env

echo Navigating to the Backend application directory...
cd ..\Backend

echo Cleaning and restoring .NET dependencies...
dotnet clean && dotnet restore

echo Applying Entity Framework database migrations...
dotnet ef database update

cd ..\..

echo Executing SQL script to populate the database...
docker cp data.sql %CONTAINER_ID%:/data.sql
docker exec -it %CONTAINER_ID% /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P MyStrongPassword123! -I -i /data.sql -C -t 30 -N -b -e

echo Database population complete.
pause
