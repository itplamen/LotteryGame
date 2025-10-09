#!/usr/bin/env bash
set -e

host="$1"
shift
cmd="$@"

echo "Checking SQL Server at $host..."

until /opt/mssql-tools/bin/sqlcmd -S "$host" -U sa -P "YourStrong!Passw0rd" -Q "SELECT 1" >/dev/null 2>&1; do
  echo "Waiting for SQL Server at $host..."
  sleep 5
done

echo
exec $cmd