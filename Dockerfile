# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the .csproj and restore
COPY ticketingsys/TicketingSys.csproj ./ticketingsys/
RUN dotnet restore ./ticketingsys/TicketingSys.csproj

# Copy the rest of the code
COPY ticketingsys ./ticketingsys/
WORKDIR /src/ticketingsys

# Build the project
RUN dotnet build TicketingSys.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish TicketingSys.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketingSys.dll"]
