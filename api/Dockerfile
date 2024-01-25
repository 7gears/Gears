FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-step

WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out



FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /App
COPY --from=build-step /App/out .

ENV ASPNETCORE_ENVIRONMENT=${ENVIRONMENT} \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

EXPOSE 8080



ENTRYPOINT ["dotnet", "Host.dll"]