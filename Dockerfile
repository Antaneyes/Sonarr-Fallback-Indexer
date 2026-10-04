# Imagen de Sonarr Fallback Indexer sobre la imagen de LinuxServer.
#
#   docker build -t local/sonarr-fallback:<version> .
#
# LSIO_TAG debe ser la versión de upstream en la que se basa este fork, para que el
# sistema base sea el mismo con el que se publicó esa versión. El programa se sustituye
# entero, así que la versión de Sonarr que trae la imagen base no se usa.
ARG LSIO_TAG=4.0.20.3014-ls326

# --- Backend --------------------------------------------------------------------------
# SDK Alpine para compilar para musl de forma nativa; global.json fija exactamente el SDK 6.0.405
# (con otro SDK 6.0 se empaquetan versiones de DLL distintas y Sonarr no arranca).
FROM mcr.microsoft.com/dotnet/sdk:6.0.405-alpine3.17 AS backend
RUN apk add --no-cache bash jq sqlite-libs
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /src
COPY . .
RUN dotnet msbuild -restore src/Sonarr.sln -p:SelfContained=True -p:Configuration=Release \
        -p:Platform=Posix -p:RuntimeIdentifiers=linux-musl-x64 -t:PublishAllRids -v:m
# Todos los proyectos se publican en la misma carpeta y alguno puede sobrescribir una
# DLL de NuGet con otra versión. Se vuelve a copiar cada asset de paquete que declara
# Sonarr.deps.json desde la caché de NuGet, para que coincida con lo que espera la app.
# Además, la app corre con el usuario abc: todo debe ser legible y los ejecutables
# (ffprobe, createdump…) ejecutables por cualquiera, no solo por root.
RUN set -e; pub=_output/net6.0/linux-musl-x64/publish; \
    jq -r '.libraries as $l | .targets[] | to_entries[] | select(.value.runtime != null) | .key as $k \
           | select($l[$k].type == "package") | .value.runtime | keys[] | "\($l[$k].path)/\(.)"' \
       "$pub/Sonarr.deps.json" | sort -u | while read -r asset; do \
        src="/root/.nuget/packages/$asset"; dst="$pub/$(basename "$asset")"; \
        if [ -f "$src" ] && [ -f "$dst" ] && ! cmp -s "$src" "$dst"; then echo "corrigiendo $(basename "$asset")"; cp "$src" "$dst"; fi; \
    done; \
    rm -rf "$pub/UI"; \
    chmod -R a+rX "$pub"; \
    find "$pub" -type f -perm -u+x -exec chmod a+x {} +

# --- Frontend -------------------------------------------------------------------------
FROM node:20 AS frontend
WORKDIR /src
COPY package.json yarn.lock .yarnrc ./
RUN yarn install --frozen-lockfile --network-timeout 120000
COPY . .
RUN yarn run build --env production

# --- Imagen final ---------------------------------------------------------------------
FROM linuxserver/sonarr:${LSIO_TAG}
RUN rm -rf /app/sonarr/bin
COPY --from=backend /src/_output/net6.0/linux-musl-x64/publish/ /app/sonarr/bin/
COPY --from=frontend /src/_output/UI/ /app/sonarr/bin/UI/
LABEL org.opencontainers.image.source="https://github.com/Antaneyes/Sonarr-Fallback-Indexer"
