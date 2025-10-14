# TestAppMaui

Rozwiązanie prezentuje implementację architektury DDD dla aplikacji .NET MAUI z lokalną bazą SQLite oraz usługą gateway w ASP.NET Core. Projekt został przygotowany w oparciu o .NET 8 (aktualnie dostępna stabilna wersja środowiska wykonawczego) i zawiera następujące elementy:

- **Domain** – warstwa domenowa z agregatem `TaskItem` oraz podstawową infrastrukturą DDD.
- **Application** – logika przypadków użycia wykorzystująca Mediatr oraz kontrakty repozytoriów asynchronicznych.
- **Infrastructure** – implementacja repozytoriów i konfiguracja EF Core dla SQLite (klient) oraz SQL Server (gateway), a także integracja z CRM Dynamics 365.
- **MauiClient** – aplikacja .NET MAUI (Windows, Android, iOS, Mac Catalyst) korzystająca z kontrolki Telerik RadDataGrid oraz RadEntry/RadButton do pracy na lokalnej bazie SQLite i synchronizacji przez REST API.
- **Gateway** – lekka usługa WebAPI (Minimal API) działająca jako brama komunikacyjna z bazą SQL Server oraz CRM Dynamics 365 8.2 on-premise, z włączonym CORS.

## Użyte technologie

- [.NET MAUI](https://learn.microsoft.com/dotnet/maui/what-is-maui)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/) z dostawcami SQLite oraz SQL Server
- [MediatR](https://github.com/jbogard/MediatR)
- [Dawn.Guard](https://github.com/Enkomio/Dawn) do walidacji danych wejściowych
- [CommunityToolkit.MVVM](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) dla wzorca MVVM
- [Telerik UI for .NET MAUI](https://www.telerik.com/maui-ui) – wymagane zdefiniowanie źródła pakietów NuGet Telerik w celu pobrania bibliotek (konieczna aktywna licencja)
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) do generowania dokumentacji Swagger w gateway

## Pierwsze kroki

1. **Workload .NET MAUI** – aby zbudować aplikację należy zainstalować workload MAUI i platformowe SDK (`dotnet workload install maui`).
2. **Źródło pakietów Telerik** – dodaj prywatne źródło NuGet Telerik w pliku `NuGet.Config` lub globalnie (`dotnet nuget add source`).
3. **Przygotowanie baz danych** – gateway wymaga dostępnego serwera SQL Server; domyślny connection string `GatewayDatabase` można zmienić w `src/Gateway/appsettings.json`. Klient MAUI tworzy lokalny plik SQLite (`testappmaui.db`).
4. **Konfiguracja CRM** – sekcja `Crm` w `src/Gateway/appsettings.json` definiuje adres Web API (`BaseUrl`) oraz poświadczenia (NTLM). W przypadku braku konfiguracji końcówki CRM nie są wystawiane.
5. **Uruchomienie gateway** – `dotnet run --project src/Gateway/Gateway.csproj` (po zainstalowaniu SDK). Końcówki REST są domyślnie dostępne dla klientów przeglądarkowych (Angular) dzięki włączonemu CORS.
6. **Uruchomienie klienta MAUI** – `dotnet build src/MauiClient/MauiClient.csproj` oraz uruchomienie na wybranej platformie (`dotnet build -t:Run`). Klient komunikuje się z gateway przy użyciu `HttpClient`; adres bazowy można nadpisać zmienną środowiskową `GATEWAY_BASE_URL`.

## Architektura

Schemat warstwowy (DDD):

```
Presentation (MauiClient, Gateway)
        ↓
   Application (MediatR, DTO)
        ↓
     Domain (Encje, Agregaty)
        ↓
 Infrastructure (EF Core, SQLite)
```

Warstwa `Application` nie zna implementacji repozytoriów – komunikuje się z nimi przez interfejs `IRepository`. Wstrzykiwanie zależności zapewniają metody rozszerzające `AddApplication` i `AddInfrastructure`.

## Angular / Gateway

Gateway został przygotowany jako Minimal API i może pełnić warstwę komunikacji z aplikacją Angular. Udostępnia końcówki REST (`GET /tasks`, `POST /tasks`) operujące na tych samych przypadkach użycia co aplikacja MAUI.

## Uwagi

- Obecnie stabilne środowisko to .NET 8; po udostępnieniu .NET 10 wystarczy zaktualizować `TargetFramework` w plikach `.csproj`.
- Środowisko demonstracyjne nie posiada zainstalowanego SDK .NET ani workloadów MAUI, dlatego nie wykonano kompilacji ani testów automatycznych.
- Ikony pakietu dla platformy Windows są generowane w trakcie kompilacji z danych Base64, aby repozytorium nie zawierało plików binarnych.
