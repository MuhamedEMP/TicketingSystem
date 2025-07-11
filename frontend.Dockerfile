# Build stage
FROM node:20-alpine as build
WORKDIR /app
COPY ./vuefrontend/package*.json ./
RUN npm install
COPY ./vuefrontend ./
RUN npm run build

# Serve stage
FROM nginx:stable-alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY ./vuefrontend/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
