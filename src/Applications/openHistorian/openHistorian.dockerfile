#Published App MUST be self-contained for this dockerfile to create a working image

# Use the official .NET 8.0 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory inside the container
WORKDIR /app

# Copy the published application from the local 'openHistorian' folder to the container
COPY ./openHistorian/** /app/

COPY ./defaults.ini /usr/share/openHistorian/
COPY ./settings.ini /usr/share/openHistorian/
COPY ./wwwroot /app/wwwroot

# Set permissions to 777 for all copied folders and files
RUN chmod -R 777 /app && chmod -R 777 /usr/share/openHistorian

# Ensure the application is executable
RUN chmod +x openHistorian

# Expose the application port
EXPOSE 8180

# Define the entry point to run the application
ENTRYPOINT ["/app/openHistorian"]