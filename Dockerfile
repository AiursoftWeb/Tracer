ARG CSPROJ_PATH="./src/"
ARG PROJ_NAME="Aiursoft.Tracer"

# ============================
# Prepare NPM Environment
FROM hub.aiursoft.cn/node:21-alpine as npm-env
ARG CSPROJ_PATH
WORKDIR /src
COPY . .

# NPM Build at PGK_JSON_PATH
RUN npm install --prefix "${CSPROJ_PATH}wwwroot"

# ============================
# Prepare Building Environment
FROM hub.aiursoft.cn/mcr.microsoft.com/dotnet/sdk:8.0 as build-env
ARG CSPROJ_PATH
ARG PROJ_NAME
WORKDIR /src
COPY --from=npm-env /src .

# Build
RUN dotnet publish ${CSPROJ_PATH}${PROJ_NAME}.csproj  --configuration Release --no-self-contained --runtime linux-x64 --output /app
RUN cp -r ${CSPROJ_PATH}/wwwroot/* /app/wwwroot

# ============================
# Prepare Runtime Environment
FROM hub.aiursoft.cn/mcr.microsoft.com/dotnet/aspnet:8.0
ARG PROJ_NAME
WORKDIR /app
COPY --from=build-env /app .

RUN sed -i 's/DataSource=app.db/DataSource=\/data\/app.db/g' appsettings.json
RUN sed -i 's/\/tmp\/data/\/data/g' appsettings.json
RUN mkdir -p /data

VOLUME /data
EXPOSE 5000

ENV SRC_SETTINGS=/app/appsettings.json
ENV VOL_SETTINGS=/data/appsettings.json
ENV DLL_NAME=${PROJ_NAME}.dll

#ENTRYPOINT dotnet $DLL_NAME --urls http://*:5000
ENTRYPOINT ["/bin/bash", "-c", "\
    if [ ! -f \"$VOL_SETTINGS\" ]; then \
        cp $SRC_SETTINGS $VOL_SETTINGS; \
    fi && \
    if [ -f \"$SRC_SETTINGS\" ]; then \
        rm $SRC_SETTINGS; \
    fi && \
    ln -s $VOL_SETTINGS $SRC_SETTINGS && \
    dotnet $DLL_NAME --urls http://*:5000 \
"]
