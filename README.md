# 🎓 University Admissions Scoring Engine

## 🇵🇱 Opis projektu (Polski)

**University Admissions Scoring Engine** to system wspomagający proces rekrutacji na studia wyższe. Umożliwia definiowanie algorytmów przeliczania wyników matur, przypisywanie ich do kierunków studiów oraz automatyczne generowanie rankingów kandydatów.

Projekt został zbudowany w technologii:
- ASP.NET Core (MVC)
- Entity Framework Core
- SQL Server (LocalDB)
- HTML / CSS / JavaScript (frontend)

---

## 🚀 Główne funkcjonalności

### 📚 Zarządzanie maturami i przedmiotami
- Obsługa różnych typów matur (np. nowa, stara)
- Konfigurowalne skale punktowe (np. 0–100, 1–6 co 0.5)
- Przedmioty z podziałem na:
  - tryb (ustny / pisemny)
  - poziom (podstawowy / rozszerzony)

---

### 🧠 System algorytmów punktacji
- Tworzenie dowolnych algorytmów rekrutacyjnych
- Obsługa:
  - SUMA (sumowanie punktów)
  - LUB (wybór najlepszej kombinacji)
- Drzewiasta struktura algorytmu (grupy i podgrupy)
- Możliwość:
  - oznaczenia przedmiotu jako wymagany
  - ustawienia minimalnej liczby punktów (dla przedmiotu lub grupy)
- Obsługa wielu matur w jednym algorytmie (różne przeliczenia)

---

### 🎯 Inteligentne liczenie punktów
- Automatyczny wybór najlepszej kombinacji przedmiotów
- Obsługa wielu wariantów jednego przedmiotu (np. podstawa vs rozszerzenie)
- Uwzględnianie:
  - mnożników
  - progów minimalnych
  - wymagalności przedmiotów

---

### 🧪 Tryb testowania algorytmu
- Możliwość symulacji kandydata
- Wprowadzanie wyników ręcznie
- Podgląd:
  - uzyskanych punktów
  - ścieżki obliczeń (debug algorytmu)

---

### 🎓 Kierunki studiów
- Tworzenie kierunków z parametrami:
  - tryb (stacjonarne / niestacjonarne)
  - rodzaj (I stopnia / II stopnia / jednolite)
  - limity przyjęć
- Przypisywanie algorytmów do kierunków
- Obsługa przypisywania i odpinania algorytmów

---

### 📊 Ranking kandydatów
- Automatyczne:
  - przeliczanie punktów
  - generowanie rankingów
- Statusy:
  - Przyjęty
  - Lista rezerwowa
  - Niezakwalifikowany
- Sortowanie według punktów
- Dynamiczna aktualizacja wyników

---

### 🧾 Interfejs użytkownika
- Edytor algorytmów w formie drzewa
- Dynamiczne operacje bez przeładowania strony (AJAX)
- Wyszukiwanie przedmiotów w czasie rzeczywistym
- Czytelny podgląd algorytmu

---

## 🏗️ Architektura

Projekt oparty jest na architekturze MVC:

- **Models** – struktura danych (Entity Framework)
- **Views** – interfejs użytkownika (Razor)
- **Controllers** – logika aplikacji
- **Services** – logika biznesowa (np. liczenie punktów)

---

## ⚙️ Uruchomienie projektu

1. Otwórz projekt w Visual Studio
2. Wykonaj migracje:
   ```
   Update-Database
   ```
3. Uruchom aplikację (`F5`)
4. System automatycznie zainicjalizuje dane startowe

---

## 📌 Planowane rozszerzenia

- Eksport rankingów do PDF
- Import danych kandydatów (CSV / Excel)
- Panel administracyjny
- API do integracji z systemami uczelni
- Autoryzacja użytkowników

---

---

# 🇬🇧 Project Description (English)

**University Admissions Scoring Engine** is a system designed to support the university admissions process. It allows defining scoring algorithms, assigning them to study programs, and automatically generating candidate rankings.

Built with:
- ASP.NET Core (MVC)
- Entity Framework Core
- SQL Server (LocalDB)
- HTML / CSS / JavaScript

---

## 🚀 Main Features

### 📚 Exam and subject management
- Support for multiple exam types (e.g., new, old exams)
- Configurable scoring scales (e.g., 0–100, 1–6 with 0.5 step)
- Subjects with:
  - mode (oral / written)
  - level (basic / advanced)

---

### 🧠 Scoring algorithm system
- Fully configurable admission algorithms
- Supports:
  - SUM (add scores)
  - OR (choose best combination)
- Tree-based structure (groups and subgroups)
- Features:
  - required subjects
  - minimum score thresholds
- Multiple exam variants per algorithm

---

### 🎯 Intelligent scoring engine
- Automatically selects the best subject combination
- Supports multiple variants of the same subject
- Handles:
  - multipliers
  - minimum thresholds
  - required conditions

---

### 🧪 Algorithm testing mode
- Simulate candidate input
- Manual score input
- Displays:
  - calculated score
  - decision path (debug output)

---

### 🎓 Study programs
- Define programs with:
  - mode (full-time / part-time)
  - type (Bachelor / Master / long-cycle)
  - admission limits
- Assign / unassign algorithms to programs

---

### 📊 Candidate ranking
- Automatic:
  - score calculation
  - ranking generation
- Status assignment:
  - Accepted
  - Waiting list
  - Rejected
- Sorted by score
- Dynamic updates

---

### 🧾 User interface
- Tree-based algorithm editor
- AJAX-based dynamic updates (no page reload)
- Real-time subject search
- Clear algorithm visualization

---

## 🏗️ Architecture

The project follows MVC architecture:

- **Models** – data layer (Entity Framework)
- **Views** – UI (Razor)
- **Controllers** – application logic
- **Services** – business logic (scoring engine)

---

## ⚙️ Running the project

1. Open in Visual Studio
2. Run:
   ```
   Update-Database
   ```
3. Start the app (`F5`)
4. Initial data will be seeded automatically

---

## 📌 Future improvements

- PDF export of rankings
- Candidate import (CSV / Excel)
- Admin panel
- API integration
- User authentication
