FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LetsEncrypt.Worker.csproj", "LetsEncrypt.Worker/"]
WORKDIR "/src/LetsEncrypt.Worker"
RUN dotnet restore "LetsEncrypt.Worker.csproj"
COPY . .
RUN dotnet build "LetsEncrypt.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetsEncrypt.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LetsEncrypt.Worker.dll"]