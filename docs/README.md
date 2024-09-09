## Setup

### Python Script

1. **Location**: `src/python/steamdb-connector.py`
2. **Dependency**: `mysql-connector-python`
   - To install:
     ```bash
     pip install mysql-connector-python
     ```

### JavaScript Script

1. **Location**: `src/javascript/steamdb-connector.js`
2. **Dependency**: `mysql2`
   - To install:
     ```bash
     npm install mysql2
     ```

### Configuration File (`db-config.json`)

Both scripts require a configuration file named `db-config.json`, which stores the MySQL connection details. This file should be placed in the same directory as the scripts.

Ensure that the db-config.json file is included in your .gitignore to avoid exposing sensitive data.

## Helper Functions

### Python Script Helper Functions

- **`fetch_all_games()`**: Fetches all games from the database.
- **`fetch_game_by_appid(appid)`**: Fetches a specific game by `appid`.
- **`insert_game(game_data)`**: Inserts a new game record into the database.
- **`update_game(appid, field, value)`**: Updates a specific field for a game by `appid`.
- **`delete_game(appid)`**: Deletes a game from the database by `appid`.

### JavaScript Script Helper Functions

- **`fetchAllGames()`**: Fetches all games from the database.
- **`fetchGameByAppid(appid)`**: Fetches a specific game by `appid`.
- **`searchGamesByFilter(filter)`**: Searches games by genre or tag using a filter.

---

## Database Schema

### `indie_games` Table

The `indie_games` table stores game data with the following columns:

1. **`appid`** (INT, PRIMARY KEY): The unique identifier for each game.
2. **`name`** (VARCHAR(255)): The name of the game.
3. **`release_date`** (DATE): The release date of the game.
4. **`genres`** (TEXT): The genres of the game.
5. **`tags`** (TEXT): Tags, including 'Indie'.
6. **`pct_pos_total`** (DECIMAL(5,2)): Percentage of total positive reviews.
7. **`pct_pos_recent`** (DECIMAL(5,2)): Percentage of recent positive reviews.
8. **`windows`** (TINYINT(1)): Boolean value indicating if the game is available on Windows.
9. **`mac`** (TINYINT(1)): Boolean value indicating if the game is available on Mac.
10. **`linux`** (TINYINT(1)): Boolean value indicating if the game is available on Linux.
11. **`metacritic_score`** (INT): The Metacritic score of the game.
12. **`categories`** (TEXT): Categories of the game (e.g., Single-player, Multiplayer).
13. **`estimated_owners`** (VARCHAR(50)): The estimated number of game owners (e.g., "10,000 - 50,000").
14. **`ccu`** (INT): Current concurrent users.
15. **`price`** (DECIMAL(10,2)): The price of the game in dollars.
