FROM microsoft/aspnetcore-build as build-env
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish --no-restore -c Release -o /build

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-env /build .
ENTRYPOINT ["dotnet","./Tracer.dll"]
