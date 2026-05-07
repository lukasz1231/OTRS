# Instrukcja użytkownika systemu OTRS

## 1. Do czego służy aplikacja

OTRS to aplikacja do obsługi zgłoszeń serwisowych i zdarzeń. Użytkownik może zgłosić problem, sprawdzić jego status i dopisać komentarz. Pracownicy helpdesku, technicy i administratorzy mogą obsługiwać zgłoszenia, zmieniać ich status, priorytet, kategorię, klienta oraz kolejkę. Administrator zarządza też słownikami systemowymi, użytkownikami i klientami.

System składa się z dwóch części:

- frontend Vue dostępny standardowo pod adresem `http://localhost:5173`,
- backend ASP.NET Core dostępny standardowo pod adresem `https://localhost:7054` oraz `http://localhost:5066`.

## 2. Uruchomienie aplikacji

### Backend

1. Wejdź do katalogu `otrs-backend/otrs-backend`.
2. Uruchom API:

```powershell
dotnet run
```

3. Po uruchomieniu API powinno być dostępne pod adresem `https://localhost:7054`. W trybie developerskim dostępny jest też Swagger pod adresem `https://localhost:7054/swagger`.

Backend korzysta z bazy SQLite `otrs.db`. Przy starcie aplikacji wykonywane są migracje bazy danych.

### Frontend

1. Wejdź do katalogu `otrs-frontend`.
2. Uruchom aplikację:

```powershell
npm run dev
```

3. Otwórz w przeglądarce adres pokazany przez Vite, zwykle `http://localhost:5173`.

## 3. Logowanie i rejestracja

1. Otwórz ekran logowania.
2. Wybierz zakładkę `Logowanie` albo `Rejestracja`.
3. Przy rejestracji podaj imię i nazwisko, adres e-mail, hasło oraz potwierdzenie hasła.
4. Hasło musi mieć co najmniej 8 znaków.
5. Po zalogowaniu system przenosi użytkownika do panelu głównego.

Jeśli użytkownik zapomniał hasła, może użyć opcji `Zapomniałeś hasła?`. System generuje link resetujący hasło. Link jest ważny przez 1 godzinę. W środowisku developerskim link jest także zwracany przez API i wypisywany w konsoli backendu.

## 4. Role i uprawnienia

System rozróżnia kilka typów użytkowników:

- `Klient` lub zwykły użytkownik: tworzy zgłoszenia, przegląda swoje zgłoszenia, dodaje komentarze.
- `Technik`: widzi zgłoszenia przypisane do niego albo do jego kolejek, może zmieniać część parametrów zgłoszenia i obsługiwać realizację.
- `Helpdesk`: widzi zgłoszenia, obsługuje zgłoszenia oczekujące, może zmieniać kolejki, klienta i dane klasyfikacyjne.
- `Admin`: ma pełny dostęp do obsługi zgłoszeń i panelu administracyjnego.

Widoczność ekranów zależy od roli. Jeżeli użytkownik nie ma uprawnień do danej strony, aplikacja przekieruje go do dashboardu albo formularza właściwego dla jego roli.

## 5. Panel główny

Po zalogowaniu użytkownik widzi dashboard z podsumowaniem pracy:

- liczba wszystkich widocznych zgłoszeń,
- liczba nowych zgłoszeń,
- liczba zgłoszeń w trakcie,
- liczba zgłoszeń z przekroczonym SLA,
- wykres zgłoszeń z ostatnich 7 dni,
- lista 5 ostatnich zgłoszeń.

Z dashboardu można szybko przejść do utworzenia zgłoszenia albo do listy zgłoszeń.

## 6. Tworzenie zgłoszenia

### Zwykły użytkownik

1. Kliknij `Utwórz zgłoszenie` albo przejdź do formularza `Nowe zgłoszenie`.
2. Wpisz tytuł zgłoszenia. Wymagane jest od 5 do 50 znaków.
3. Wpisz szczegółowy opis problemu. Wymagane jest co najmniej 20 znaków.
4. Wybierz typ, priorytet i kategorię.
5. Kliknij `Utwórz zgłoszenie`.
6. Po zapisaniu system przekieruje Cię do szczegółów nowego zgłoszenia.

Dla zwykłego użytkownika kolejka ustawiana jest domyślnie przez system.

### Helpdesk, technik i administrator

Pracownik obsługi tworzy zgłoszenie w rozszerzonym formularzu:

1. Wpisz tytuł i opis zgłoszenia.
2. Wybierz klienta.
3. Wybierz typ, priorytet, kategorię i kolejkę.
4. Kliknij `Utwórz zgłoszenie`.

Kategorie są filtrowane według wybranego klienta. Najpierw wybierz klienta, a dopiero potem kategorię.

## 7. Lista zgłoszeń

Ekran `Zgłoszenia` pokazuje zgłoszenia widoczne dla zalogowanego użytkownika.

Na liście można:

- wyszukać zgłoszenie po ID lub tytule,
- filtrować zgłoszenia po statusie: wszystkie, w toku, zakończone,
- filtrować po priorytecie,
- sprawdzić status SLA,
- przejść do szczegółów zgłoszenia.

Helpdesk i administrator mają dodatkowy ekran `Oczekujące`, który pokazuje nowe zgłoszenia oczekujące na rozpoczęcie obsługi.

## 8. Szczegóły zgłoszenia

W szczegółach zgłoszenia dostępne są:

- publiczny numer zgłoszenia, np. `PL2026033000001`,
- tytuł i opis,
- status,
- priorytet,
- kategoria,
- typ,
- klient,
- kolejka,
- data utworzenia,
- dane zgłaszającego,
- informacje o SLA,
- komentarze i załączniki.

Każdy uprawniony użytkownik może dodać komentarz. Komentarz może zawierać samą treść, same załączniki albo treść razem z załącznikami.

## 9. Obsługa statusów i SLA

SLA jest liczone na podstawie czasu utworzenia zgłoszenia oraz liczby godzin SLA ustawionej w priorytecie.

System pokazuje między innymi:

- termin realizacji,
- pozostały czas,
- informację, czy SLA jest w normie,
- ostrzeżenie, gdy do SLA zostało mniej niż 8 godzin,
- stan krytyczny, gdy zostały 2 godziny lub mniej,
- przekroczenie SLA.

Jeżeli status oznacza wstrzymanie, SLA zostaje zatrzymane. Jeżeli status oznacza rozwiązanie lub zamknięcie, system zapisuje czas rozwiązania i sprawdza, czy zgłoszenie zmieściło się w SLA.

Technik ma ograniczone możliwości zmiany statusu. Administrator i helpdesk mają szersze uprawnienia do prowadzenia zgłoszenia.

## 10. Powiadomienia

System posiada ekran `Powiadomienia`, dostępny po kliknięciu ikony dzwonka w pasku nawigacji.

Powiadomienia informują o zdarzeniach związanych ze zgłoszeniami, między innymi:

- utworzeniu nowego zgłoszenia,
- dodaniu komentarza,
- zmianie statusu zgłoszenia.

Przy ikonie dzwonka widoczna jest liczba nieodczytanych powiadomień. Jeżeli liczba jest większa niż 99, system pokazuje `99+`.

Na ekranie powiadomień można:

- zobaczyć listę najnowszych powiadomień,
- odróżnić powiadomienia przeczytane od nieprzeczytanych,
- kliknąć powiadomienie, aby przejść do powiązanego zgłoszenia,
- oznaczyć pojedyncze powiadomienie jako przeczytane przez kliknięcie,
- użyć przycisku `Odczytaj wszystkie`, aby oznaczyć wszystkie powiadomienia jako przeczytane.

Powiadomienia są wysyłane do administratorów, helpdesku oraz techników powiązanych ze zgłoszeniem lub jego kolejką. System nie wysyła powiadomienia do użytkownika, który sam wykonał daną akcję.

## 11. Profil użytkownika

W profilu użytkownik widzi:

- imię i nazwisko,
- adres e-mail,
- rolę,
- listę swoich zgłoszeń.

Z profilu można przejść do zmiany hasła oraz do utworzenia nowego zgłoszenia.

## 12. Panel administracyjny

Panel administracyjny jest dostępny dla administratora. Część danych może być odczytywana także przez role obsługowe, ale tworzenie, edycja i usuwanie konfiguracji jest przeznaczone głównie dla administratora.

### Użytkownicy

Administrator może:

- przeglądać użytkowników,
- wyszukiwać użytkowników,
- edytować imię, nazwisko, e-mail i hasło,
- przypisywać role,
- usuwać użytkowników.

Nie można usunąć użytkownika, który jest twórcą zgłoszenia albo jest przypisany do zgłoszenia.

### Klienci

Administrator może tworzyć i edytować klientów. Dane klienta obejmują nazwę, opis, adres, telefon oraz przypisanych użytkowników.

Przypisanie użytkownika do klienta wpływa na dane kontaktowe i klasyfikację zgłoszeń.

### Kolejki

Kolejki służą do kierowania zgłoszeń do odpowiednich zespołów. Administrator może:

- tworzyć kolejki,
- usuwać kolejki,
- przypisywać użytkowników do kolejek,
- usuwać użytkowników z kolejek.

Nie można usunąć kolejki, która zawiera zgłoszenia.

### Kategorie

Kategorie opisują obszar lub usługę, której dotyczy zgłoszenie. Kategoria może być powiązana z klientem. Administrator może dodawać, edytować i usuwać kategorie.

Nie można usunąć kategorii używanej w zgłoszeniach.

### Priorytety

Priorytet określa ważność zgłoszenia i czas SLA. Administrator ustawia:

- nazwę,
- opis,
- poziom,
- liczbę godzin SLA.

SLA musi być większe od zera. Nie można usunąć priorytetu używanego w zgłoszeniach.

### Statusy

Status określa etap obsługi zgłoszenia, np. nowe, w toku, wstrzymane, wykonane lub rozwiązane. Administrator może tworzyć, edytować i usuwać statusy.

Nie można usunąć statusu używanego w zgłoszeniach.

### Typy

Typ opisuje rodzaj zgłoszenia. Administrator może dodawać, edytować i usuwać typy.

Nie można usunąć typu używanego w zgłoszeniach.

## 13. Dobre praktyki pracy ze zgłoszeniami

- Tytuł powinien krótko opisywać problem.
- Opis powinien zawierać kroki odtworzenia problemu, komunikaty błędów i wpływ na pracę.
- Priorytet dobieraj według pilności i wpływu na użytkowników.
- Komentarze dodawaj w szczegółach zgłoszenia, aby historia obsługi była kompletna.
- Załączniki dodawaj jako część komentarza, np. zrzuty ekranu, logi lub dokumenty.
- Zmieniaj status zgłoszenia na bieżąco, żeby dashboard i SLA pokazywały rzeczywisty stan pracy.

## 14. Najczęstsze problemy

### Nie mogę się zalogować

Sprawdź adres e-mail i hasło. Hasło przy rejestracji musi mieć co najmniej 8 znaków. Jeśli hasło jest zapomniane, użyj resetu hasła.

### Nie widzę panelu administracyjnego

Panel administracyjny wymaga roli `Admin`. Poproś administratora o nadanie odpowiedniej roli.

### Nie widzę zgłoszenia

Widoczność zgłoszeń zależy od roli. Zwykły użytkownik widzi swoje zgłoszenia. Technik widzi zgłoszenia przypisane do siebie albo swoich kolejek. Helpdesk i administrator widzą wszystkie zgłoszenia.

### Nie mogę usunąć elementu słownika

System blokuje usuwanie elementów używanych w zgłoszeniach, np. statusu, priorytetu, kategorii, kolejki lub typu.

### Kategorie nie pokazują się w formularzu helpdesku

W formularzu helpdesku najpierw wybierz klienta. Lista kategorii jest filtrowana według wybranego klienta.
