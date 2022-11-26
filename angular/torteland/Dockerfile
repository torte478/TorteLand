FROM node:16-alpine as build
WORKDIR /usr/src/app
COPY angular/torteland/package.json angular/torteland/package-lock.json ./
RUN npm install
COPY angular/torteland/ .
COPY TorteLand.Secret/TorteLand.Front/config.json ./src/assets/config.json
RUN npm run build

FROM nginx:1.17.1-alpine
COPY angular/torteland/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /usr/src/app/dist/torteland /usr/share/nginx/html