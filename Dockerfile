# .NET SDK use karein build ke liye
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Project files copy karein aur restore karein
COPY *.csproj ./
RUN dotnet restore

# Pura code copy karein aur publish karein
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime image use karein
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "SentimentAi.dll"]
