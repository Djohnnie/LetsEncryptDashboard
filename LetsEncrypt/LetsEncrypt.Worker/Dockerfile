FROM mcr.microsoft.com/dotnet/runtime:10.0

# Set the working directory
WORKDIR /app

# Copy the app
COPY bin/Release/publish /app

ENV TZ=Europe/Brussels
ENV CONNECTION_STRING=.
ENV CERTIFICATE_PATH=.
ENV CERTIFICATE_PASSWORD=.
ENV CHALLENGE_PATH=.
ENV WORKER_INTERVAL=.

# Start the app
ENTRYPOINT ["dotnet", "LetsEncrypt.Worker.dll"]