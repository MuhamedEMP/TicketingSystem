
FROM node:20-alpine
WORKDIR /app
COPY ./vuefrontend/package*.json ./
RUN npm install
COPY ./vuefrontend ./
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host"]
