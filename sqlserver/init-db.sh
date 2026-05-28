#!/bin/bash
set -e

/opt/mssql/bin/sqlservr &
pid=$!

until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1; do
  echo "Waiting for SQL Server to start..."
  sleep 2
done

echo "SQL Server started. Running initialization script..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /scripts/Backup.sql
echo "Initialization complete."

wait $pid
