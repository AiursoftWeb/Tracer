FROM microsoft/aspnetcore-build AS build-env
WORKDIR /app

COPY . .
RUN dotnet publish

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-env /app/bin/Debug/netcoreapp2.0/publish/ .
ENTRYPOINT ["dotnet","./Tracer.dll"]
