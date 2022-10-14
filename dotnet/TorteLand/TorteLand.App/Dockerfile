FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TorteLand.App/TorteLand.App.csproj", "TorteLand.App/"]
RUN dotnet restore "TorteLand.App/TorteLand.App.csproj"
COPY . .
WORKDIR "/src/TorteLand.App"
RUN dotnet build "TorteLand.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TorteLand.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ADD TorteLand.App/docker_config/appsettings.json /app
ENTRYPOINT ["dotnet", "TorteLand.App.dll"]