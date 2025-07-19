FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5111

ENV ASPNETCORE_URLS=http://+:5111

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["NetWebApiKC/NetWebApiKC.csproj", "NetWebApiKC/"]
RUN dotnet restore "NetWebApiKC/NetWebApiKC.csproj"
COPY . .
WORKDIR "/src/NetWebApiKC"
RUN dotnet build "NetWebApiKC.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "NetWebApiKC.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetWebApiKC.dll"]
