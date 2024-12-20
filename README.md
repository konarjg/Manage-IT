# Manage-IT
Wersja webowa:
1. Backend: asp.net
2. Frontend: JS + CSS + CSHTML

Wersja desktopowa:
1. Backend: WPF
2. Frontend: XML

Ustawienie środowiska pracy:
1. Instalujemy visual studio community 2022 z modułami asp.net i programowanie aplikacji w języku C#
2. Tworzymy folder w dowolnym miejscu i otwieramy go
3. Prawy przycisk myszy -> otwórz w terminalu
4. Wpisujemy komendę git init -> inicjujemy repozytorium lokalne git
5. Wpisujemy komendę git remote add origin https://github.com/konarjg/Manage-IT -> dodaje link do repozytorium w chmurze github pod nazwą origin
6. Wpisujemy komendę git pull origin -> pobiera cały kod z chmury od ostatniej zatwierdzonej zmiany
7. Wpisujemy komendę git switch imie -> przełącza nas z lokalnego branchu na branch o naszym imieniu, tutaj prowadzimy naszą całą pracę
8. Otwieramy plik Manage IT.sln

Wykonywanie zadań:
1. Wejdź na https://trello.com/b/2YgmexjY/manage-it i sprawdź aktualne przydzielone zadanie
2. Przeczytaj uważnie instrukcję do zadania
3. Otwórz folder roboczy
4. Wpisz komendę git pull origin imie, aby pobrać najnowsze zmiany na twoim branchu
5. PAMIĘTAJ, BY ZAWSZE PISAĆ KOD TYLKO NA SWOIM BRANCHU I NIE MODYFIKOWAĆ INNYCH PLIKÓW NIŻ TE PRZEWIDZIANE W ZADANIU!!!
6. Zacznij pisać kod
8. Wpisz komendy git add -A następnie git commit -a -m "OPIS ZMIAN" i potem git push origin imie, aby podesłać każdą zmianę do chmury, najlepiej kilka razy w trakcie pracy po większych postępach
9. Po ukończeniu zadania wejdź na https://github.com/konarjg/Manage-IT
10. Utwórz pull request i poczekaj na zatwierdzenie przez kierownika
11. NIGDY NIE TWÓRZ PULL REQUESTÓW NA BRANCH INNY NIŻ MAIN!!!
12. Po zatwierdzeniu zmerguj pull request z branchem main

Przydatne linki:
1. https://learn.microsoft.com/en-us/dotnet/csharp/ -> dokumentacja C# plus wiele przykładów, zawiera także dokumentację WPF i ASP.NET
2. https://www.w3schools.com/git/ -> poradnik korzystania z git
3. https://refactoring.guru/design-patterns/csharp -> wzorce projektowe w C#
4. https://www.lambdatest.com/learning-hub/unit-testing -> testy jednostkowe, poradnik i dobre praktyki
