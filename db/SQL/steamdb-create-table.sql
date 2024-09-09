CREATE TABLE indie_games (
    appid INT PRIMARY KEY,                -- Unique identifier for each game
    name VARCHAR(255) NOT NULL,           -- Name of the game
    release_date DATE,                    -- Release date of the game
    genres TEXT,                          -- Genres of the game (e.g., RPG, Action)
    tags TEXT,                            -- Tags, including 'Indie' (store as comma-separated or JSON array)
    pct_pos_total DECIMAL(5, 2),          -- Percentage of total positive reviews
    pct_pos_recent DECIMAL(5, 2),         -- Percentage of recent positive reviews
    windows TINYINT(1),                   -- Game available on Windows (0 for FALSE, 1 for TRUE)
    mac TINYINT(1),                       -- Game available on Mac (0 for FALSE, 1 for TRUE)
    linux TINYINT(1),                     -- Game available on Linux (0 for FALSE, 1 for TRUE)
    metacritic_score INT,                 -- Metacritic score (if applicable)
    categories TEXT,                      -- Game categories (e.g., Single-player, Multiplayer)
    estimated_owners VARCHAR(50),         -- Estimated number of owners (range as text)
    ccu INT,                              -- Current concurrent users (CCU)
    price DECIMAL(10, 2)                  -- Game price in USD or relevant currency
);
