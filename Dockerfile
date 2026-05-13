# Etap budowania
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopiowanie pliku .csproj i przywracanie zależności
COPY ["otrs-backend/otrs-backend/otrs-backend.csproj", "otrs-backend/otrs-backend/"]
RUN dotnet restore "otrs-backend/otrs-backend/otrs-backend.csproj"

# Kopiowanie reszty kodu backendu
COPY . .
WORKDIR "/src/otrs-backend/otrs-backend"

# Budowanie i publikowanie aplikacji do folderu /app/publish
RUN dotnet publish "otrs-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etap uruchomieniowy (tylko to, co niezbędne do działania)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Uruchomienie aplikacji
ENTRYPOINT ["dotnet", "otrs-backend.dll"]
