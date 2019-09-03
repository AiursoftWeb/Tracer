FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build-env
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /build

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /build .
ENTRYPOINT ["dotnet","./Tracer.dll"]
