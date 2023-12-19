# Praktyki aplikacja

To repozytorium zawiera aplikację z usługami Angular, Django, ASP.NET, Gotenberg oraz bazę postgres. Wszystkie usługi są skonteneryzowane

## Spis treści

- [Opis](#opis)
- [Wymagania wstępne](#wymagania-wstępne)
- [Rozpoczęcie pracy](#rozpoczęcie-pracy)
- [Szczegóły kontenerów](#szczegóły-kontenerów)
- [Użycie](#użycie)

## Opis

Projekt to wielokontenerowa aplikacja korzystająca z Docker Compose, łącząca różne technologie:

- Aplikacja Angular dla frontendu
- Serwer Django dla logiki backendowej
- Serwer ASP.NET do komunikacja z narzędziem Gotenberg i obsługą konwersji dokumentów
- Narzędzie Gotenberg do konwersji dokumentów
- Baza danych Postgres

## Wymagania wstępne

Upewnij się, że masz zainstalowane:

- Docker

## Rozpoczęcie pracy

1. Sklonuj repozytorium:

   ```bash
   git clone https://github.com/Papcer/praktyki.git
   cd praktyki

2. Zbuduj kontenery:

   ```bash
   make build

3. Uruchom kontenery:

    ```bash
    make start

4. Dostępne usługi:

- Aplikacja Angular: http://localhost:4200
- Serwer Django: http://localhost:8000
- Serwer ASP.NET http://localhost:8080
- Narzędzie Gotenberg http://localhost:3000
- Baza danych http://localhost:5432

## Użycie

Odwiedź adresy URL podane w sekcji rozpoczęcie pracy, aby uzyskać dostęp do usług. Opis dostępnych api pod adresami

- Serwer Django: http://localhost:8000/swagger
- Serwer ASP.NET http://localhost:8080/swagger
