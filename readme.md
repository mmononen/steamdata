## Git Repository for ICT CAMP II 2024 Steamdata Data Project

Kun indie-pelinkehittäjällä on käytössään data tämän vuosikymmenen indie-peleistä, jonka rakenne sisältää muun muassa pelin nimen, julkaisupäivän, genret, tagit, myynti- ja käyttäjätiedot, hän saattaa haluta vastauksia useisiin eri kysymyksiin. Näiden tietojen avulla kehittäjä voi ymmärtää paremmin markkinoiden tarpeita, kilpailua ja potentiaalisia menestystekijöitä. Tässä on joitakin keskeisiä kysymyksiä, joihin kehittäjä voisi haluta vastauksia:
## Datasta yhteisesti katsottavat asiat:

```
['AppID']: {
	'name', 
	'release_date', 
	'initialprice', - original US price in cent
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
    	'peak_ccu', (peak CCU yesterday)
	'tags',
    	'pct_pos_total',
	'num_reviews_total'
}
```
 
### 0. Tilastot
    - pelien määrä per datasetti
    - top5 genret
    - top5 tagit
    - ilmaisten pelien prosenttiosuus
    - moninpelit vs. yksinpelit / moninpelihybridit vs. yksinpelihybridit
    - käyttöjärjestelmien / alustojen prosenttiosuus

### 1. Pelin suosion ja arvostelujen yhteys (käyttäjäarvostelut + MetaCritic):

    - Mikä on yhteys pelin positiivisten arvostelujen prosenttiosuuden (sekä kokonaisten että uusien arvostelujen) ja pelin genren, tagien tai hintaluokan välillä?
    - Miten metacritic-pisteet korreloivat myyntien tai käyttäjäarvioiden kanssa?
    - Vaikuttaako pelin hinta positiivisten arvostelujen osuuteen?

### 2. Trendaavat genret vuosittain
	-  ChatGPT suosittelee ccu:ta parametriksi

### 3. Myyntimäärät ja omistajat (hintatason vaikutus):

    - Miten pelien arvioidut omistajamäärät korreloivat pelin genren, hintatason ja muiden tekijöiden (esim. tagit tai kategoriat) kanssa?
    - Ovatko tietyt genre- tai hintaluokat suositumpia kuin toiset omistajamäärillä mitattuna?
    - Miten pelin hinta vaikuttaa myyntimääriin tai aktiivisten pelaajien määrään (ccu)?
    - Onko halvemmat pelit yleisesti ottaen paremmin arvioituja kuin kalliimmat pelit?

### 4. Early Accessin vaikutus pelin suosioon:

    - Onko yhteys pelin julkaisupäivän ja sen huippuaktiivisten pelaajien määrän (ccu) välillä?
    - Ovatko tietyt julkaisupäivät suotuisampia kuin toiset (esim. lomakaudet, viikonloput)?

### 5. Genrejen ja tagien merkitys:

    - Mitkä genret tai tagit ovat suosituimpia positiivisten arvostelujen tai myyntimäärien perusteella?
    - Onko tietty genre erityisen suosittu tietyillä käyttöjärjestelmillä (Windows, Mac, Linux)?
    - Mitkä tagit korreloivat korkeampien myyntien tai aktiivisten pelaajamäärien kanssa?
