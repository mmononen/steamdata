---
marp: true
title: 'SteamData Project'
author: 'Roy Koljonen'
date: 'October 2024'
theme: gaia
style: |
  section {
    background: linear-gradient(120deg, #3B4252 50%, #2E3440 50%);
    color: #D8DEE9;
  }
  .sub-title {
    font-size: 30px;
    color: #D8DEE9;
  }
  .tiny-text {
    font-size: 12px;
    color: #D8DEE9;
  }
paginate: true
---

# SteamData®

<br><br>

<span class="sub-title">**Roy Koljonen**</span>
<span class="sub-title">**Lari Lindberg**</span>
<span class="sub-title">**Mikko Mononen**</span>
<span class="sub-title">**Tommi Tammelin**</span>

<span class="sub-title">**October 2024**</span>

## <span class="bottom-align tiny-text">_"SteamData" is not really a registered trademark_</span>

---

<style scoped>
  h4 {
    /* margin-top: 50px; */
    /* font-size: 0.8em; */
  }
  li {
    font-size: 0.8em;
  }
</style>


#### Original Goals:

- Gather, analyze and visualize indie game data for recognizing trends.

#### Final Product:

- Cloud server with database structured to handle game information efficiently.
- Web interface allows users to filter and search for games, with results dynamically displayed based on user input.
- Backend app for importing, validating and filtering data with bulk data insertion to the database.
- Data analysis and visualisation.
- LLM sentiment analysis.

---

<style scoped>
    h4 {
    margin-top: -30px;
    font-size: 0.8em;
    }
    img.hieno {
        max-width: 100%;
        margin: 0 auto;
    }
</style>

#### Project Diagram

<img src="IMG/Hieno.png" alt="Hieno Diagram" class="hieno">

https://mmononen.github.io/steamdata/

---
<style scoped>
    h4 {
        margin-top: -50px;
        font-size: 0.8em;
    }
    img.trello {
        max-width: 100%;
        margin: 0 auto;
    }
</style>

#### Initial Planning

<img src="IMG/Trello1209.png" alt="Trello 1" class="trello">

---

<style scoped>
    h4 {
    /* margin-top: -30px; */
    /* font-size: 0.8em; */
    }
</style>

#### OOS Ideas:
- Backend app Steam API integration.
- Whole dataset and indie games dataset comparison.
- Branch ID

#### Future Ideas:
- More tools for the front end user.
- Data analysis visualization at front end.
- Automated database update.