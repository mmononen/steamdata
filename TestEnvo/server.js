const express = require("express");
const path = require("path"); // To work with file paths
const app = express();
const port = 3000;
const { fetchAllGames, fetchGameByAppid } = require("../src/javascript/steamdb-connector");

// Serve static files from the TestEnvo directory
app.use(express.static(path.join(__dirname, "TestEnvo")));

// Fetch all games from MySQL
app.get("/games", (req, res) => {
	fetchAllGames((err, games) => {
		if (err) {
			return res.status(500).json({ error: err.message });
		}
		res.json(games);
	});
});

// Fetch a game by AppID from MySQL
app.get("/games/:appid", (req, res) => {
	const appid = req.params.appid;
	fetchGameByAppid(appid, (err, game) => {
		if (err) {
			return res.status(500).json({ error: err.message });
		}
		res.json(game);
	});
});

// Serve the index.html file on the root URL
app.get("/", (req, res) => {
	res.sendFile(path.join(__dirname, "TestEnvo/index.html"));
});

// Start the Express server
app.listen(port, () => {
	console.log(`Server running at http://localhost:${port}`);
});
