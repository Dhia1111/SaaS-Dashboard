# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["APIs/APIs.sln", "APIs/"]
COPY ["APIs/APIs.csproj", "APIs/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Connection/Connection.csproj", "Connection/"]

# Restore NuGet packages
RUN dotnet restore "APIs/APIs.sln"

# Copy all source code
COPY . .

# Build and publish the API project
WORKDIR "/src/APIs"
RUN  dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://*:7073
EXPOSE 7073
ENTRYPOINT ["dotnet", "APIs.dll"]