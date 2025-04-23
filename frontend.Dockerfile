
FROM node:20-alpine
WORKDIR /app
COPY ./vuefrontend/package*.json ./
ENV NODE_ENV=development
ENV VITE_API_BASE_URL=http://backend:5172
RUN npm install
COPY ./vuefrontend ./
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host"]
