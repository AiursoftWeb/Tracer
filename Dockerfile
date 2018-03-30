FROM microsoft/aspnetcore
WORKDIR /app
COPY ./bin/Debug/netcoreapp2.0/publish .
ENTRYPOINT ["dotnet","./Tracer.dll"]
