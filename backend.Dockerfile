# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the main project .csproj
COPY TicketingSys/TicketingSys.csproj ./TicketingSys/
RUN dotnet restore ./TicketingSys/TicketingSys.csproj

# Copy the rest of the application
COPY TicketingSys/ ./TicketingSys/

# Publish using the .csproj (NOT the solution)
WORKDIR /src/TicketingSys
RUN dotnet publish TicketingSys.csproj -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TicketingSys.dll"]
