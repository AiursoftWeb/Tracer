FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src

COPY . .
RUN dotnet publish -c Release -o /build

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /build .
ENTRYPOINT ["dotnet","./Tracer.dll"]
