# Entity Framework Core – Schnellstart für ApplicationDataContext

## Voraussetzungen
- .NET 6 oder neuer
- NuGet-Pakete:
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.Sqlite (oder ein anderes Provider-Paket)
  - Microsoft.Extensions.Configuration (optional, für appsettings.json)

## Installation der Pakete (im Projektverzeichnis)

```powershell
# Entity Framework Core und SQLite Provider
 dotnet add package Microsoft.EntityFrameworkCore
 dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# Optional: Für Konfiguration aus appsettings.json
 dotnet add package Microsoft.Extensions.Configuration
 dotnet add package Microsoft.Extensions.Configuration.Json
```

## Migrationen (nur bei Code-First)

```powershell
# Tools installieren (nur einmal nötig)
dotnet tool install --global dotnet-ef

# Migration erstellen
dotnet ef migrations add InitialCreate

# Migration anwenden (Datenbank erzeugen/aktualisieren)
dotnet ef database update
```

## Beispiel: Verwendung des ApplicationDataContext

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MusterprojektBie.Services;

// Konfiguration laden
var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

string connectionString = config.GetConnectionString("OracleDatabase") ?? "Data Source=musterprojekt.db";

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();
optionsBuilder.UseSqlite(connectionString);

using var context = new ApplicationDataContext(optionsBuilder.Options);

// Beispiel: Alle Debitoren laden
var debitoren = context.Debitoren.ToList();
```

## Hinweise
- Die Datei `appsettings.json` muss im Projektverzeichnis liegen.
- Für Oracle statt SQLite: `dotnet add package Oracle.EntityFrameworkCore` und `optionsBuilder.UseOracle(connectionString);`
- Migrationen funktionieren nur mit Code-First-Ansatz.

---
Für weitere Fragen siehe die offizielle Doku: https://learn.microsoft.com/de-de/ef/core/
