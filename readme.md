## Git Repository for ICT CAMP II 2024 Steamdata Data Project

Kun indie-pelinkehittäjällä on käytössään data tämän vuosikymmenen indie-peleistä, jonka rakenne sisältää muun muassa pelin nimen, julkaisupäivän, genret, tagit, myynti- ja käyttäjätiedot, hän saattaa haluta vastauksia useisiin eri kysymyksiin. Näiden tietojen avulla kehittäjä voi ymmärtää paremmin markkinoiden tarpeita, kilpailua ja potentiaalisia menestystekijöitä. Tässä on joitakin keskeisiä kysymyksiä, joihin kehittäjä voisi haluta vastauksia:
## Datasta yhteisesti katsottavat asiat:

```
'AppID':
	'name', 
	'release_date', 
	'required_age', 
	'price', 
    	'windows',
	'mac',
	'linux',
	'metacritic_score',
	'recommendations',
	'categories', 
	'genres',
    	'positive',
	'negative',
	'estimated_owners',
    	'average_playtime_forever',
    	'peak_ccu',
	'tags',
    	'pct_pos_total',
	'num_reviews_total']
```
 
### 0. Tilastot
    - pelien määrä per datasetti
    - top5 genret
    - top5 tagit
    - ilmaisten pelien prosenttiosuus
    - moninpelit vs. yksinpelit / moninpelihybridit vs. yksinpelihybridit
    - käyttöjärjestelmien / alustojen prosenttiosuus

### 1. Pelin suosion ja arvostelujen yhteys:

    - Mikä on yhteys pelin positiivisten arvostelujen prosenttiosuuden (sekä kokonaisten että uusien arvostelujen) ja pelin genren, tagien tai hintaluokan välillä?
    - Miten metacritic-pisteet korreloivat myyntien tai käyttäjäarvioiden kanssa?
    - Vaikuttaako pelin hinta positiivisten arvostelujen osuuteen?

### 2. Myyntimäärät ja omistajat:

    - Miten pelien arvioidut omistajamäärät korreloivat pelin genren, hintatason ja muiden tekijöiden (esim. tagit tai kategoriat) kanssa?
    - Ovatko tietyt genre- tai hintaluokat suositumpia kuin toiset omistajamäärillä mitattuna?

### 3. Käyttöjärjestelmätuen merkitys:

    - Vaikuttaako pelin tukeminen useilla käyttöjärjestelmillä (Windows, Mac, Linux) pelin suosioon (positiiviset arviot, myynti tai pelaajamäärät)?
    - Ovatko peleillä, jotka tukevat useampia käyttöjärjestelmiä, korkeammat omistajamäärät tai käyttäjäarviot kuin peleillä, jotka tukevat vain yhtä alustaa?

### 4. Hintatason vaikutus:

    - Miten pelin hinta vaikuttaa myyntimääriin tai aktiivisten pelaajien määrään (ccu)?
    - Onko halvemmat pelit yleisesti ottaen paremmin arvioituja kuin kalliimmat pelit?

### 5. Pelaajakäyttäytyminen:

    - Onko yhteys pelin julkaisupäivän ja sen huippuaktiivisten pelaajien määrän (ccu) välillä?
    - Ovatko tietyt julkaisupäivät suotuisampia kuin toiset (esim. lomakaudet, viikonloput)?

### 6. Genrejen ja tagien merkitys:

    - Mitkä genret tai tagit ovat suosituimpia positiivisten arvostelujen tai myyntimäärien perusteella?
    - Onko tietty genre erityisen suosittu tietyillä käyttöjärjestelmillä (Windows, Mac, Linux)?
    - Mitkä tagit korreloivat korkeampien myyntien tai aktiivisten pelaajamäärien kanssa?

### 7. Metacritic-arvostelujen rooli:

    - Onko korkea metacritic-pisteys tärkeämpi joissakin genreissä kuin toisissa?
    - Miten pelin metacritic-pisteet vaikuttavat pelin myyntiin tai arvioitujen omistajien määrään?

### 8. Aktiiviset pelaajat (ccu):

    - Mitkä tekijät (genre, hinta, arviot, alustatuki) korreloivat korkeimpien aktiivisten pelaajien määrien kanssa?
    - Onko aktiivisten pelaajien määrällä ja pelin arvioidulla omistajamäärällä suora yhteys?

### 9. Julkaisupäivän ajoitus:

    - Vaikuttaako pelin julkaisupäivä (esim. vuoden aika, kuukauden päivä) sen menestykseen arvostelujen tai myyntimäärien osalta?
    - Onko peleillä, jotka julkaistaan tietyllä aikavälillä, paremmat metacritic-arviot tai suuremmat omistajamäärät?

### 10. Vertailu kilpailijoihin:

    - Kuinka samankaltaiset pelit (esim. samalla genrellä tai tagilla) menestyvät suhteessa toisiinsa?
    - Ovatko kilpailijapelien hinnat, genret tai metacritic-pisteet yhteydessä toisten pelien menestykseen?

Näihin kysymyksiin vastaaminen voisi auttaa indie-kehittäjää optimoimaan oman pelinsä suunnittelua, julkaisua ja markkinointia.
