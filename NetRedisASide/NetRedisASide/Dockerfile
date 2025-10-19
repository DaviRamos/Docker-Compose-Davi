FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5122

ENV ASPNETCORE_URLS=http://+:5122

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["NetRedisASide/NetRedisASide.csproj", "NetRedisASide/"]
RUN dotnet restore "NetRedisASide/NetRedisASide.csproj"
COPY . .
WORKDIR "/src/NetRedisASide"
RUN dotnet build "NetRedisASide.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "NetRedisASide.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetRedisASide.dll"]
