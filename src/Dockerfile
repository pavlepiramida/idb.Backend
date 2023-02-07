#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["idb.Backend.csproj", "."]
RUN dotnet restore "./idb.Backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "idb.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "idb.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet  idb.Backend.dll