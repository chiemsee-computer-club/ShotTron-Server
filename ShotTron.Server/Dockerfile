FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ShotTron.Server/ShotTron.Server.csproj", "ShotTron.Server/"]
RUN dotnet restore "ShotTron.Server/ShotTron.Server.csproj"
COPY . .
WORKDIR "/src/ShotTron.Server"
RUN dotnet build "ShotTron.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShotTron.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShotTron.Server.dll"]
