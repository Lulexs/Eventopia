@echo off

echo Stopping database docker container...
docker stop eventopia-sql

echo Removing docker container...
docker rm eventopia-sql

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyStrongPassword123!" --name eventopia-sql -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest

echo Waiting for SQL Server to start up...
timeout /t 10 /nobreak

echo Applying Entity Framework database migrations...
cd Aplikacija\Backend\Application
dotnet ef database update
cd ..\..\..

echo Executing SQL script to populate the database...
docker cp pw_data.sql eventopia-sql:/data.sql
docker exec -it eventopia-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P MyStrongPassword123! -I -i /data.sql -C -t 30 -N -b -e

echo Database population complete.
pause

