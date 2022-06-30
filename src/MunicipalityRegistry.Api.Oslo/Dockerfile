FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.3

LABEL maintainer "Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="municipality-registry"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

EXPOSE 2012/tcp
ENV ASPNETCORE_URLS http://*:2012

ENTRYPOINT ["./init.sh"]
