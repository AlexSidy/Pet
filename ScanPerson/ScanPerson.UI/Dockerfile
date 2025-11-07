# Create image based off of the official Node 22 image
FROM node:22-alpine as builder

# Install OpenSSL for SSL certificate generation
RUN apk add --no-cache openssl

# Change directory so that our commands run inside this new directory
WORKDIR /app

# Открываем порт 4200
EXPOSE 4200

# Copy dependency definitions
COPY package*.json ./

RUN npm install

RUN npm install -g @angular/cli

# Get all the code needed to run the app
COPY . .

# Copy and set up entrypoint script
COPY docker-entrypoint.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# Set entrypoint
ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]

#TODO протестировать данные измнеения в задаче #27
# script for set enviroments
#COPY ./src /app/src
#RUN node generate-proxy.js
#RUN chmod +x set-enviroments.sh
#RUN ./set-enviroments.sh

# Запускаем приложение
# CMD ["ng", "serve", "--host", "0.0.0.0", "--poll", "2000", "--proxy-config", "proxy.conf.json"]
CMD ["ng", "serve", "--host", "0.0.0.0", "--poll", "2000", "--configuration", "https", "--proxy-config", "proxy.conf.json"]

