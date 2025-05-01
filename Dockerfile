# Fase di build ottimizzata con caching degli strati
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG CONFIGURATION=Release

WORKDIR /src

# Copia solo i file necessari per il restore
COPY ["DSx.Caching.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["sources/**/*.csproj", "sources/"]
COPY ["tests/**/*.csproj", "tests/"]

# Restore delle dipendenze con caching esplicito
RUN dotnet restore "DSx.Caching.sln" --runtime linux-x64 \
    && rm -rf **/bin **/obj

# Copia tutto il codice sorgente
COPY . .

# Compilazione e pubblicazione ottimizzata
RUN dotnet publish "DSx.Caching.sln" \
    -c $CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --runtime linux-x64 \
    --self-contained false \
    -p:DebugType=None \
    -p:DebugSymbols=false

# Fase di runtime leggera con immagine ASP.NET Core ottimizzata
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Configurazione esplicita delle variabili d'ambiente
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_NOLOGO=true
ENV DOTNET_EnableDiagnostics=0

# Copia solo gli artefatti necessari
COPY --from=build /app/publish .

# Configurazione dell'utente non privilegiato
USER $APP_UID

ENTRYPOINT ["dotnet", "DSx.Caching.dll"]