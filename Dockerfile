FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["src/FlixStats/FlixStats.csproj", "src/FlixStats/"]
RUN dotnet restore "src/FlixStats/FlixStats.csproj"
COPY . .
WORKDIR "/src/src/FlixStats"
RUN dotnet build "FlixStats.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FlixStats.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "FlixStats.dll"]
