FROM node:24.0.2 AS build
ARG API_BASE_URL
WORKDIR /app/
RUN echo VITE_API_BASE_URL=${API_BASE_URL} > .env
COPY ./package*.json ./
RUN npm ci
COPY ./ ./
RUN npm run build

FROM nginx:1.27.5 AS prod
WORKDIR /app/
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/ /usr/share/nginx/html/
