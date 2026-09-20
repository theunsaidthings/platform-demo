#!/usr/bin/env bash
set -e

# --- 1. Validation Check ---
# Check if DB_CONNECTION_STRING is unset (-z tests if the length of the string is zero)
if [ -z "$DB_CONNECTION_STRING" ]; then
  # Print structured JSON error to standard error (stream 2)
  echo '{"level":"Error","message":"Pre-flight check failed: DB_CONNECTION_STRING is missing."}' >&2
  exit 1
fi

# --- 2. Success Path ---
# If DB_CONNECTION_STRING is present, output structured JSON to standard output (stream 1)
echo '{"level":"Information","message":"Pre-flight checks passed. Starting .NET API..."}'

# --- 3. Process Replacement ---
# Execute the passed-in command (e.g., 'dotnet run') and replace PID 1
exec "$@"