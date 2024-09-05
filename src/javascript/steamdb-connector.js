// Install mysql2 package (if not already installed)
// npm install mysql2

const mysql = require("mysql2");

// Connection details
const connection = mysql.createConnection({
	host: "86.60.209.30",
	user: "remoteuser",
	password: "karvainenkala",
	database: "indie_games_db",
	port: 3306,
});

// Helper function to fetch all games
function fetchAllGames() {
	const query = "SELECT * FROM steam_games;";
	connection.query(query, (err, results) => {
		if (err) {
			console.error("Error fetching all games:", err);
			return;
		}
		console.log("All games:", results);
	});
}

// Helper function to fetch a game by appid
function fetchGameByAppid(appid) {
	const query = "SELECT * FROM steam_games WHERE appid = ?;";
	connection.query(query, [appid], (err, result) => {
		if (err) {
			console.error("Error fetching game:", err);
			return;
		}
		console.log("Game found:", result[0]);
	});
}

// Helper function to insert a new game
// Check README.md for more information
function insertGame(gameData) {
	const query = `
    INSERT INTO steam_games (
      appid, name, developer, publisher, score_rank, positive, negative, userscore, owners, 
      average_forever, average_2weeks, median_forever, median_2weeks, price, initialprice, discount, ccu
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
  `;
	connection.query(query, gameData, (err, result) => {
		if (err) {
			console.error("Error inserting game:", err);
			return;
		}
		console.log("Game inserted successfully.");
	});
}

// Helper function to update game data
function updateGame(appid, field, value) {
	const query = `UPDATE steam_games SET ${field} = ? WHERE appid = ?;`;
	connection.query(query, [value, appid], (err, result) => {
		if (err) {
			console.error("Error updating game:", err);
			return;
		}
		console.log(`Game with appid ${appid} updated successfully.`);
	});
}

// Helper function to delete a game by appid
function deleteGame(appid) {
	const query = "DELETE FROM steam_games WHERE appid = ?;";
	connection.query(query, [appid], (err, result) => {
		if (err) {
			console.error("Error deleting game:", err);
			return;
		}
		console.log(`Game with appid ${appid} deleted successfully.`);
	});
}

// Example usage
// Uncomment the lines below to test the functions

// fetchAllGames();
// fetchGameByAppid(10);
// insertGame([10, 'Counter-Strike', 'Valve', 'Valve', null, 234220, 6172, 0, '10,000,000 .. 20,000,000', 15339, 2792, 209, 117, 9.99, 9.99, 0, 10444]);
// updateGame(10, 'name', 'Updated Game Name');
// deleteGame(10);

// Close the connection when done
// connection.end();
