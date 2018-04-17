FROM node as frontend-env
WORKDIR /src
COPY . .
RUN npm i -g bower
RUN bower install --allow-root

FROM microsoft/aspnetcore-build AS build-env
WORKDIR /src

COPY --from=frontend-env /src .
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /build

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-env /build .
ENTRYPOINT ["dotnet","./Tracer.dll"]
