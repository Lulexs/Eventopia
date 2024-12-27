echo "Starting Docker container for Microsoft SQL Server..."
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyStrongPassword123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest

CONTAINER_ID=$(docker ps -q --filter ancestor=mcr.microsoft.com/mssql/server:latest)

echo "Waiting for SQL Server to start up..."
sleep 20  # Give SQL Server time to initialize

echo "Navigating to the Frontend application directory..."
cd Aplikacija/Frontend

echo "Installing npm dependencies for the Frontend application..."
npm install

echo "Creating the .env file with the database server URL..."
echo "VITE_DB_SERVER=http://localhost:5184" > .env

echo "Navigating to the Backend application directory..."
cd ../Backend

echo "Cleaning and restoring .NET dependencies..."
dotnet clean && dotnet restore

echo "Applying Entity Framework database migrations..."
dotnet ef database update

echo "Executing SQL script to populate the database..."
docker cp data.sql $CONTAINER_ID:/data.sql
docker exec -it $CONTAINER_ID /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P MyStrongPassword123! -i /data.sql

echo "Database population complete."
