# Create image based off of the official Node 10 image
FROM node:22-alpine as builder

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

#TODO протестировать данные измнеения в задаче #27
# script for set enviroments
#COPY ./src /app/src
#RUN node generate-proxy.js
#RUN chmod +x set-enviroments.sh
#RUN ./set-enviroments.sh

USER $APP_UID
# Запускаем приложение
CMD ["ng", "serve", "--host", "0.0.0.0", "--poll", "2000", "--proxy-config", "proxy.conf.json"]

