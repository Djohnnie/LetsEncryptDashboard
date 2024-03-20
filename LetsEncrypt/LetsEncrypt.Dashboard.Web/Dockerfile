FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the app
COPY bin/Release/publish /app

EXPOSE 8080

ENV TZ=Europe/Brussels
ENV CONNECTION_STRING=.

# Start the app
ENTRYPOINT ["dotnet", "LetsEncrypt.Dashboard.Web.dll"]