# Statička analiza projekta LV1


**Rezultat izvršavanja:** build je uspješan; SonarScanner je uspješno završio post-processing.

## Sažetak

U SonarQube listi prikazano je **16 nalaza**:

| Kategorija | Broj | Ozbiljnost |
|---|---:|---|
| Sigurnost | 7 | 1 High, 6 Medium |
| Pouzdanost | 2 | 2 Medium |
| Održavanje | 7 | Info |



## Najvažniji nalazi

### Sigurnost

- **Visok — MD5 za sažimanje lozinki (`Storage/Lozinke.cs`, L11).** MD5 nije namijenjen sigurnom čuvanju lozinki. Lozinke treba čuvati algoritmom za izvlačenje ključa koji je namjenski za lozinke, uz jedinstvenu so.
- **Srednji — SQL upiti sastavljeni unosom (`Storage/Baza.cs`, L134 i L159).** Sonar upozorava na upit za pretragu i upit za brisanje koji koriste string sastavljen od ulaznih podataka. Parametrizacija sprečava da unos mijenja SQL naredbu.
- **Srednji — `Random` umjesto kriptografski sigurnog generatora (četiri mjesta).** Najvažniji slučaj je token sesije u `Storage/Sesija.cs`; predvidiv token nije prikladan za autentikaciju. Ostala mjesta generišu ID-eve, pa treba zasebno procijeniti da li moraju biti nepredvidivi ili samo jedinstveni.

### Pouzdanost

- **Srednji — petlja može obraditi samo prvu stavku (`Services/Loan/LoanService.cs`, L141).** Metoda prima listu, a demo prosljeđuje dvije stavke, ali se metoda vraća unutar prve iteracije. Potrebno je potvrditi očekivano ponašanje i uskladiti implementaciju/povratnu vrijednost.
- **Srednji — poređenje `double` vrijednosti (`Services/Loan/LoanService.cs`, L114).** Ovdje se poredi vrijednost `podaci[4]`, koja se koristi kao oznaka 0/1, a ne iznos novca. Jasniji tip `bool` bi uklonio upozorenje i bolje izrazio namjeru.

### Održavanje

Info stavke se odnose na korištenje `IndexOf(char)`, označavanje `NovaVeza` kao `static`, preferiranje statičkog MD5 API-ja i četiri javna polja u `Sesija`. Preporuka za statički MD5 API je samo stilska; ona **ne** čini MD5 sigurnim algoritmom za lozinke.


## Diskusija

Nalaze treba prioritetno razmatrati ovim redom: čuvanje lozinki, parametrizacija SQL upita, sigurnost tokena sesije i ranjiva SQLite zavisnost. Upozorenja za `Random` na običnim ID-evima nisu nužno istog rizika kao upozorenje za token sesije. Upozorenje za `double` na L114 treba tumačiti u kontekstu: kod trenutno poredi oznaku 0/1, pa je promjena u `bool` prikladnija od tolerancije za finansijske iznose.

Statička analiza pronalazi obrasce koji mogu ukazivati na problem; nalaze treba potvrditi pregledom konteksta i zahtjeva sistema. U ovom prolazu build je uspio, ali nisu pokretani testovi.

**Quality Gate status:** Passed 
