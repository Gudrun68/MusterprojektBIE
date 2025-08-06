# DatabaseConnectionManager - Anleitung für Entwickler

## Übersicht

Der `DatabaseConnectionManager` ist eine zentrale Klasse für Oracle-Datenbankzugriffe in diesem Projekt. Diese Anleitung zeigt, wie Sie ihn richtig verwenden.

## Konfiguration der Credentials

### 1. JSON-Konfigurationsdatei erstellen

Erstellen Sie eine `appsettings.json` Datei in Ihrem Projektverzeichnis:

```json
{
  "ConnectionStrings": {
    "OracleDatabase": "Data Source=localhost:1521/XE;User Id=MyUser;Password=MyPassword;",
    "TestDatabase": "Data Source=testserver:1521/TESTDB;User Id=TestUser;Password=TestPass;"
  },
  "DatabaseSettings": {
    "CommandTimeout": 30,
    "ConnectionTimeout": 15,
    "MaxRetryAttempts": 3
  }
}
```

### 2. Umgebungsvariablen verwenden (SICHERER!)

Für produktive Umgebungen sollten Sie Umgebungsvariablen verwenden:

**Windows PowerShell:**
```powershell
$env:DB_USER = "MeinBenutzer"
$env:DB_PASSWORD = "MeinSicheresPasswort"
$env:DB_SERVER = "produktionsserver:1521/PRODDB"
```

**Windows CMD:**
```cmd
set DB_USER=MeinBenutzer
set DB_PASSWORD=MeinSicheresPasswort
set DB_SERVER=produktionsserver:1521/PRODDB
```

### 3. ConfigurationManager verwenden

```csharp
// Konfiguration laden
var config = new ConfigurationManager("appsettings.json");

// Connection String für verschiedene Umgebungen
string devConnectionString = config.GetOracleConnectionString("Development");
string prodConnectionString = config.GetOracleConnectionString("Production");

// Sichere Variante mit Umgebungsvariablen
string secureConnectionString = config.GetSecureConnectionString();
```

## Verwendung des DatabaseConnectionManager

### Einfache SELECT-Abfrage

```csharp
string connectionString = config.GetOracleConnectionString();
string query = "SELECT ID, NAME, EMAIL FROM DEBITOREN WHERE ID = :id";

DatabaseConnectionManager.ExecuteQuery(connectionString, query, reader =>
{
    while (reader.Read())
    {
        int id = reader.GetInt32("ID");
        string name = reader.IsDBNull("NAME") ? "" : reader.GetString("NAME");
        string email = reader.IsDBNull("EMAIL") ? "" : reader.GetString("EMAIL");
        
        Console.WriteLine($"Debitor: {id} - {name} ({email})");
    }
});
```

### INSERT/UPDATE/DELETE-Operationen

```csharp
string insertQuery = "INSERT INTO DEBITOREN (NAME, EMAIL) VALUES (:name, :email)";

DatabaseConnectionManager.ExecuteNonQuery(connectionString, insertQuery, command =>
{
    command.Parameters.Add(":name", OracleDbType.Varchar2).Value = "Neue Firma GmbH";
    command.Parameters.Add(":email", OracleDbType.Varchar2).Value = "kontakt@neuefirma.de";
});
```

### Verbindungsstatus prüfen

```csharp
if (DatabaseConnectionManager.ConnectionFailed)
{
    Console.WriteLine("Datenbankverbindung ist fehlgeschlagen!");
    // Alternative Logik oder Fehlermeldung
    return;
}

// Normale Datenbankoperation
DatabaseConnectionManager.ExecuteQuery(connectionString, query, reader => {
    // Datenverarbeitung
});
```

## Best Practices

### 1. Sicherheit

❌ **NIEMALS so:**
```csharp
string connectionString = "Data Source=server;User Id=admin;Password=123456;";
```

✅ **Immer so:**
```csharp
var config = new ConfigurationManager();
string connectionString = config.GetSecureConnectionString();
```

### 2. Parametrisierte Abfragen

❌ **NIEMALS so (SQL-Injection-Gefahr):**
```csharp
string query = $"SELECT * FROM DEBITOREN WHERE NAME = '{userName}'";
```

✅ **Immer so:**
```csharp
string query = "SELECT * FROM DEBITOREN WHERE NAME = :userName";
DatabaseConnectionManager.ExecuteQuery(connectionString, query, reader => {
    // Verarbeitung
});
```

### 3. Fehlerbehandlung

```csharp
try
{
    DatabaseConnectionManager.ExecuteQuery(connectionString, query, reader =>
    {
        // Datenverarbeitung
    });
}
catch (OracleException ex)
{
    Console.WriteLine($"Oracle-Fehler: {ex.Message}");
    // Spezifische Oracle-Fehlerbehandlung
}
catch (Exception ex)
{
    Console.WriteLine($"Allgemeiner Fehler: {ex.Message}");
    // Allgemeine Fehlerbehandlung
}
```

## Häufige Oracle-Fehlercodes

| Code | Bedeutung | Lösung |
|------|-----------|---------|
| ORA-12170 | Connection timeout | Netzwerkverbindung prüfen |
| ORA-12535 | TNS operation timed out | Oracle-Service läuft? |
| ORA-01017 | Invalid username/password | Credentials prüfen |
| ORA-12154 | TNS could not resolve name | TNS-Namen konfigurieren |

## Konfigurationsdateien verwalten

### Entwicklungsumgebung
- `appsettings.json` - Grundkonfiguration
- `appsettings.Development.json` - Development-Overrides

### Produktionsumgebung
- Keine Passwörter in Konfigurationsdateien!
- Umgebungsvariablen verwenden
- Azure Key Vault oder ähnliche Services

### .gitignore Einträge
```
appsettings.json
appsettings.*.json
!appsettings.example.json
```

## Beispiel für komplette Implementierung

```csharp
public class DebitorRepository
{
    private readonly string _connectionString;
    
    public DebitorRepository()
    {
        var config = new ConfigurationManager();
        _connectionString = config.GetSecureConnectionString();
    }
    
    public List<Debitor> GetAll()
    {
        var debitoren = new List<Debitor>();
        
        if (DatabaseConnectionManager.ConnectionFailed)
        {
            throw new InvalidOperationException("Datenbankverbindung fehlgeschlagen");
        }
        
        string query = "SELECT ID, NAME, EMAIL FROM DEBITOREN ORDER BY NAME";
        
        DatabaseConnectionManager.ExecuteQuery(_connectionString, query, reader =>
        {
            while (reader.Read())
            {
                debitoren.Add(new Debitor
                {
                    Id = reader.GetInt32("ID"),
                    Name = reader.IsDBNull("NAME") ? "" : reader.GetString("NAME"),
                    Email = reader.IsDBNull("EMAIL") ? "" : reader.GetString("EMAIL")
                });
            }
        });
        
        return debitoren;
    }
}


---

