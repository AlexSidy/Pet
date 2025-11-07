#!/bin/sh
# Entrypoint script for Angular dev server with SSL support

# Create SSL directory if it doesn't exist
mkdir -p /app/ssl

# Generate SSL certificate if it doesn't exist
if [ ! -f /app/ssl/server.crt ] || [ ! -f /app/ssl/server.key ]; then
    echo "Generating self-signed SSL certificate..."
    openssl genrsa -out /app/ssl/server.key 2048
    openssl req -new -x509 -key /app/ssl/server.key -out /app/ssl/server.crt -days 365 \
        -subj "/C=RU/ST=Moscow/L=Moscow/O=Development/CN=localhost" \
        -addext "subjectAltName=DNS:localhost,DNS:*.localhost,IP:127.0.0.1,IP:::1"
    chmod 644 /app/ssl/server.crt
    chmod 600 /app/ssl/server.key
    echo "SSL certificate generated successfully!"
else
    echo "SSL certificate already exists, skipping generation."
fi

# Execute the command passed to the container
exec "$@"

