FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["dotnet/TorteLand/TorteLand.Bot/TorteLand.Bot.csproj", "TorteLand.Bot/"]
RUN dotnet restore "TorteLand.Bot/TorteLand.Bot.csproj"
COPY dotnet/TorteLand/ .
WORKDIR "/src/TorteLand.Bot"
RUN dotnet build "TorteLand.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TorteLand.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ADD TorteLand.Secret/TorteLand.Bot/appsettings.json /app
ENTRYPOINT ["dotnet", "TorteLand.Bot.dll"]