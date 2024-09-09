// Install mysql2 package (if not already installed)
// npm install mysql2

const mysql = require("mysql2");
const fs = require("fs");
const path = require("path");

// Read the db-config.json file to get connection details
const configPath = path.join(__dirname, "../../db-config.json");
const config = JSON.parse(fs.readFileSync(configPath, "utf8"));

// Check if SSL settings exist in the config
if (config.ssl && config.ssl["ssl-mode"] === "REQUIRED") {
	config.ssl = {
		rejectUnauthorized: true, // You can set this to false for testing
	};
}

// Create the MySQL connection (open once)
const connection = mysql.createConnection(config);

// Helper function to fetch all games from the database
function fetchAllGames() {
	const query = `
        SELECT appid, name, release_date, genres, tags, pct_pos_total, pct_pos_recent, 
               windows, mac, linux, metacritic_score, categories, estimated_owners, ccu, price
        FROM indie_games;
    `;
	connection.query(query, (err, results) => {
		if (err) {
			console.error("Error fetching games:", err);
			return;
		}
		console.log("All games:", results);
	});
}

// Helper function to fetch a game by appid
function fetchGameByAppid(appid) {
	const query = "SELECT * FROM indie_games WHERE appid = ?;";
	connection.query(query, [appid], (err, result) => {
		if (err) {
			console.error("Error fetching game:", err);
			return;
		}
		console.log("Game found:", result[0]);
	});
}

// Helper function to search games by genre or tag
function searchGamesByFilter(filter) {
	const query = `
        SELECT appid, name, release_date, genres, tags, pct_pos_total, pct_pos_recent, 
               windows, mac, linux, metacritic_score, categories, estimated_owners, ccu, price
        FROM indie_games
        WHERE genres LIKE ? OR tags LIKE ?;
    `;
	connection.query(query, [`%${filter}%`, `%${filter}%`], (err, results) => {
		if (err) {
			console.error("Error searching games by filter:", err);
			return;
		}
		console.log(`Games matching the filter "${filter}":`, results);
	});
}

// Close the connection when all database operations are done
function closeConnection() {
	connection.end((err) => {
		if (err) {
			console.error("Error closing the connection:", err);
			return;
		}
		console.log("Database connection closed.");
	});
}

module.exports = {
	fetchAllGames,
	fetchGameByAppid,
	searchGamesByFilter,
};
// Example usage
// Uncomment the lines below to test the functions
// fetchAllGames();
// fetchGameByAppid(10);
// searchGamesByFilter('RPG');

// Once you are done, call this to close the connection
// closeConnection();
