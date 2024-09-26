const express = require('express');
const path = require('path');
const {
  fetchAllGames,
  fetchGameByAppid,
  searchGames,
  fetchCategories,
  fetchGenres,
  fetchTags,
} = require('../src/JavaScript/steamdb-connector');

const app = express();
const port = 3000;

app.use(express.static(path.join(__dirname, 'public')));

// Fetch all games or search by filters
app.get('/api/games', (req, res) => {
  const { limit = 10, name, category, genre, tag } = req.query;

  if (name || category || genre || tag) {
    searchGames({ name, category, genre, tag }, (err, games) => {
      if (err) return res.status(500).json({ error: err.message });
      res.json(games);
    });
  } else {
    fetchAllGames(parseInt(limit), (err, games) => {
      if (err) return res.status(500).json({ error: err.message });
      res.json(games);
    });
  }
});

// Fetch a game by AppID
app.get('/api/games/:appid', (req, res) => {
  fetchGameByAppid(req.params.appid, (err, game) => {
    if (err) return res.status(500).json({ error: err.message });
    res.json(game);
  });
});

// Fetch all categories
app.get('/api/categories', (req, res) => {
  fetchCategories((err, categories) => {
    if (err) return res.status(500).json({ error: err.message });
    res.json(categories);
  });
});

// Fetch all genres
app.get('/api/genres', (req, res) => {
  fetchGenres((err, genres) => {
    if (err) return res.status(500).json({ error: err.message });
    res.json(genres);
  });
});

// Fetch all tags
app.get('/api/tags', (req, res) => {
  fetchTags((err, tags) => {
    if (err) return res.status(500).json({ error: err.message });
    res.json(tags);
  });
});

// Start the server
app.listen(port, () => {
  console.log(`Server running at http://localhost:${port}`);
});
