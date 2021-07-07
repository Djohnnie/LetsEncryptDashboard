FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LetsEncrypt.Dashboard/LetsEncrypt.Dashboard.csproj", "LetsEncrypt.Dashboard/"]
RUN dotnet restore "LetsEncrypt.Dashboard/LetsEncrypt.Dashboard.csproj"
COPY . .
WORKDIR "/src/LetsEncrypt.Dashboard"
RUN dotnet build "LetsEncrypt.Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetsEncrypt.Dashboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LetsEncrypt.Dashboard.dll"]