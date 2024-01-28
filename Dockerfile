FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release --property:PublishDir=out



FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /App
COPY --from=base /App/api/src/Host/out .

ENV ASPNETCORE_ENVIRONMENT=${ENVIRONMENT} \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8



EXPOSE 8080


ENTRYPOINT ["dotnet", "Host.dll"]