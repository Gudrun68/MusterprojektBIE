# MusterprojektBie - Entwickler-Dokumentation

## Übersicht

Dieses Dokument dient als Beispiel für die Projektdokumentation und zeigt, wie Sie verschiedene Komponenten des MusterprojektBie verwenden können.

## Projektstruktur

```
MusterprojektBie/
├── Classes/          # Zusätzliche Utility-Klassen
├── Converters/       # WPF Value Converter
├── Docs/            # Projektdokumentation
├── Helpers/         # Hilfsfunktionen
├── Model/           # Datenmodelle
├── Services/        # Geschäftslogik und Datenzugriff
├── View/            # WPF Views (XAML)
└── ViewModel/       # ViewModels für MVVM
```

## Services

### DatabaseConnectionManager

Der `DatabaseConnectionManager` stellt Methoden für den Oracle-Datenbankzugriff zur Verfügung.

#### Verwendung - Query ausführen:

```csharp
string connectionString = "Data Source=localhost:1521/XE;User Id=user;Password=pass;";
string query = "SELECT * FROM DEBITOREN WHERE ID = :id";

DatabaseConnectionManager.ExecuteQuery(connectionString, query, reader =>
{
    while (reader.Read())
    {
        var id = reader.GetInt32("ID");
        var name = reader.GetString("NAME");
        Console.WriteLine($"Debitor: {id} - {name}");
    }
});
```

#### Verwendung - Daten einfügen/aktualisieren:

```csharp
string insertQuery = "INSERT INTO DEBITOREN (NAME, EMAIL) VALUES (:name, :email)";

DatabaseConnectionManager.ExecuteNonQuery(connectionString, insertQuery, command =>
{
    command.Parameters.Add(":name", OracleDbType.Varchar2).Value = "Max Mustermann";
    command.Parameters.Add(":email", OracleDbType.Varchar2).Value = "max@example.com";
});
```

## Model

### Debitor-Klasse

Die `Debitor`-Klasse repräsentiert einen Kunden/Debitor im System.

```csharp
var debitor = new Debitor
{
    Id = 1,
    Name = "Beispiel GmbH",
    Email = "kontakt@beispiel.de"
};
```

## Best Practices

### 1. Fehlerbehandlung
- Verwenden Sie immer try-catch-Blöcke bei Datenbankoperationen
- Prüfen Sie den `ConnectionFailed`-Status des DatabaseConnectionManagers

### 2. Connection Strings
- Lagern Sie Connection Strings in Konfigurationsdateien aus
- Verwenden Sie niemals Passwörter im Quellcode

### 3. SQL-Parameter
- Verwenden Sie immer parametrisierte Abfragen
- Vermeiden Sie String-Konkatenation für SQL-Befehle

## Häufige Fehler

### Oracle-Verbindungsprobleme
- **ORA-12170**: Connection timeout - Netzwerkverbindung prüfen
- **ORA-12535**: TNS operation timed out - Oracle-Service verfügbar?

### Lösungsansätze
1. Oracle-Client korrekt installiert?
2. TNS-Namen korrekt konfiguriert?
3. Firewall-Einstellungen prüfen

---
*Letzte Aktualisierung: 28. Juli 2025*
