SZYFRATOR
Szyfruje wczytane pliki wybranym algorytmem. W oknie można wybrać poprzednich odbiorców  takiego zaszyfrowanego pliku lub dodać nowych. Wtedy w folderze „Debug” tworzy się folder „Keys”, a w nim podfoldery „Private keys” oraz „Public keys”, które zawierają klucze, pozwalające odszyfrować klucz sesyjny, wykorzystany do zaszyfrowania wiadomości. Aby użyć klucza prywatnego będzie trzeba wpisać hasło, które podaje się przy tworzeniu klucza.
Plik wynikowy to plik xml, dzięki któremu osoba posiadająca wersję odszyfrowującą programu będzie mogła odszyfrować stworzoną przez nas zaszyfrowaną wiadomość. O ile posiada klucz prywatny jednego z odbiorców, do których kierowana jest wiadomość.

Tipy:
- Być może wymagana będzie zmiana referencji do biblioteki BouncyCastle, która została użyta w projekcie. Plik znajduje się w folderze „Debug”.
- Przed kliknięciem „Encrypt” należy pamiętać o zaznaczeniu na liście odbiorców osób, dla których ma być przekazana wiadomość.