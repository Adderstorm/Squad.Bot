#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Squad.Bot/Squad.Bot.csproj", "Squad.Bot/"]
RUN dotnet restore "Squad.Bot/Squad.Bot.csproj"
COPY . .
WORKDIR "/src/Squad.Bot"
RUN dotnet build "Squad.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Squad.Bot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ADD Squad.Bot/appsettings.json /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Squad.Bot.dll"]