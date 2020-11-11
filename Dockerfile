FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /build

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim
WORKDIR /app
COPY --from=build-env /build .
ENTRYPOINT ["dotnet","./Tracer.dll"]
