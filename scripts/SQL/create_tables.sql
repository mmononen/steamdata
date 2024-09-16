-- CREATE TABLE indie_games (
--     AppID INT PRIMARY KEY,
--     name VARCHAR(255) NOT NULL,
--     release_date DATE,
--     price DECIMAL(10, 2),             -- (e.g., 29.99)
--     windows TINYINT(1),               -- (TINYINT: 1 = true, 0 = false)
--     mac TINYINT(1),
--     linux TINYINT(1),
--     metacritic_score INT,             -- (e.g., 85)
--     recommendations INT,
--     positive INT,
--     negative INT,
--     estimated_owners VARCHAR(255),    -- (e.g., "500,000 - 1,000,000")
--     average_playtime_forever INT,
--     peak_ccu INT,                     -- Peak concurrent users
--     pct_pos_total DECIMAL(5, 2),      -- Percentage of positive reviews
--     num_reviews_total INT
-- );

-- SELECT * FROM indie_games;

-- CREATE TABLE genres (
--     genre_id INT PRIMARY KEY AUTO_INCREMENT,
--     genre_name VARCHAR(255) NOT NULL
-- );

-- SELECT * FROM genres;

-- CREATE TABLE tags (
--     tag_id INT PRIMARY KEY AUTO_INCREMENT,
--     tag_name VARCHAR(255) NOT NULL
-- );

-- SELECT * FROM tags;

-- CREATE TABLE categories (
--     category_id INT PRIMARY KEY AUTO_INCREMENT,
--     category_name VARCHAR(255) NOT NULL
-- );

-- SELECT * FROM categories;

-- CREATE TABLE game_genres (
--     AppID INT,
--     genre_id INT,
--     PRIMARY KEY (AppID, genre_id),
--     FOREIGN KEY (AppID) REFERENCES indie_games(AppID) ON DELETE CASCADE,
--     FOREIGN KEY (genre_id) REFERENCES genres(genre_id) ON DELETE CASCADE
-- )

-- DROP TABLE game_genres;

-- SELECT * FROM game_genres;

-- CREATE TABLE game_tags (
--     AppID INT,
--     tag_id INT,
--     PRIMARY KEY (AppID, tag_id),
--     FOREIGN KEY (AppID) REFERENCES indie_games(AppID) ON DELETE CASCADE,
--     FOREIGN KEY (tag_id) REFERENCES tags(tag_id) ON DELETE CASCADE
-- )

-- SELECT * FROM game_tags;

-- CREATE TABLE game_categories (
--     AppID INT,
--     category_id INT,
--     PRIMARY KEY (AppID, category_id),
--     FOREIGN KEY (AppID) REFERENCES indie_games(AppID) ON DELETE CASCADE,
--     FOREIGN KEY (category_id) REFERENCES categories(category_id) ON DELETE CASCADE
-- )

-- SELECT * FROM game_categories;