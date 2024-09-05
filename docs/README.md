### Usage

1. **Python Script**:

   - Found under `src/python/steamdb-connector.py`.
   - Use `pip install mysql-connector-python` to install the necessary dependency.

2. **JavaScript Script**:
   - Found under `src/javascript/steamdb-connector.js`.
   - Use `npm install mysql2` to install the necessary dependency.

## Password from Roy

## Helper Functions

- `fetchAllGames()`: Fetches all games from the database.
- `fetchGameByAppid(appid)`: Fetches a specific game by `appid`.
- `insertGame(gameData)`: Inserts a new game record into the database.
- `updateGame(appid, field, value)`: Updates a game field by `appid`.
- `deleteGame(appid)`: Deletes a game by `appid`.

## Column explanations for the steam_games table:

1. appid (INT, PRIMARY KEY)
   - The unique identifier for each game, assigned by Steam.
2. name (VARCHAR(255))
   - The name of the game.
3. developer (VARCHAR(255))
   - The developer of the game.
4. publisher (VARCHAR(255))
   - The publisher of the game.
5. score_rank (VARCHAR(50))
   - The score rank of the game (can be empty or NULL).
6. positive (INT)
   - The number of positive reviews the game has received.
7. negative (INT)
   - The number of negative reviews the game has received.
8. userscore (INT)
   - The user score of the game (can be 0 or NULL if not applicable).
9. owners (VARCHAR(50))
   - The estimated number of owners of the game, often given as a range (e.g., "1,000,000 .. 5,000,000").
10. average_forever (INT)
    - The average playtime of the game in minutes for all players.
11. average_2weeks (INT)
    - The average playtime of the game in the last two weeks, in minutes.
12. median_forever (INT)
    - The median playtime of the game in minutes for all players.
13. median_2weeks (INT)
    - The median playtime of the game in the last two weeks, in minutes.
14. price (DECIMAL(10,2))
    - The current price of the game, in dollars (converted from cents).
15. initialprice (DECIMAL(10,2))
    - The initial price of the game, in dollars (converted from cents).
16. discount (INT)
    - The current discount on the game as a percentage.
17. ccu (INT)
    - The current concurrent users (CCU) playing the game.
