SELECT * FROM indie_games;
-- SELECT * FROM genres;
-- SELECT * FROM tags;
-- SELECT * FROM categories;
-- SELECT * FROM game_genres;
-- SELECT * FROM game_tags;
-- SELECT * FROM categories;


-- CREATE TABLE indie_games (
--     AppID INT PRIMARY KEY,
--     name VARCHAR(255) NOT NULL,
--     release_date DATE,
--     price DECIMAL(10, 2),             -- (e.g., 29.99)
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



-- CREATE TABLE genres (
--     genre_id INT PRIMARY KEY AUTO_INCREMENT,
--     genre_name VARCHAR(255) NOT NULL
-- );



-- CREATE TABLE tags (
--     tag_id INT PRIMARY KEY AUTO_INCREMENT,
--     tag_name VARCHAR(255) NOT NULL
-- );



-- CREATE TABLE categories (
--     category_id INT PRIMARY KEY AUTO_INCREMENT,
--     category_name VARCHAR(255) NOT NULL
-- );


-- CREATE TABLE game_genres (
--     AppID INT,
--     genre_id INT,
--     PRIMARY KEY (AppID, genre_id),
--     FOREIGN KEY (AppID) REFERENCES indie_games(AppID) ON DELETE CASCADE,
--     FOREIGN KEY (genre_id) REFERENCES genres(genre_id) ON DELETE CASCADE
-- )

-- DROP TABLE game_genres;



-- CREATE TABLE game_tags (
--     AppID INT,
--     tag_id INT,
--     PRIMARY KEY (AppID, tag_id),
--     FOREIGN KEY (AppID) REFERENCES indie_games(AppID) ON DELETE CASCADE,
--     FOREIGN KEY (tag_id) REFERENCES tags(tag_id) ON DELETE CASCADE
-- )


-- CREATE TABLE game_categories (
--     AppID INT,
--     category_id INT,
--     PRIMARY KEY (AppID, category_id),
--     FOREIGN KEY (AppID) REFERENCES indie_games(AppID) ON DELETE CASCADE,
--     FOREIGN KEY (category_id) REFERENCES categories(category_id) ON DELETE CASCADE
-- )

-- ALTER TABLE indie_games
-- DROP COLUMN windows,
-- DROP COLUMN mac,
-- DROP COLUMN linux;

-- SET FOREIGN_KEY_CHECKS = 0;
-- TRUNCATE TABLE game_tags;
-- TRUNCATE TABLE game_genres;
-- TRUNCATE TABLE game_categories;
-- TRUNCATE TABLE indie_games;
-- TRUNCATE TABLE tags;
-- TRUNCATE TABLE genres;
-- TRUNCATE TABLE categories;
-- SET FOREIGN_KEY_CHECKS = 1;

-- INSERT INTO indie_games 
--                 (
--                     AppID, name, release_date, price, metacritic_score, 
--                     recommendations, positive, negative, estimated_owners, 
--                     average_playtime_forever, peak_ccu, pct_pos_total, num_reviews_total
--                 ) 
--                 VALUES 
--                 (
--                     105600, 'Terraria', '2011-05-16', 9.99, 83, 
--                     1023411, 1254269, 30467, '20000000 - 50000000', 
--                     7451, 23331, 97, 1026239
--                 ) 
--                 ON DUPLICATE KEY UPDATE
--                     name = 'Terraria',
--                     release_date = '2011-05-16',
--                     price = 9.99,
--                     metacritic_score = 83,
--                     recommendations = 1023411,
--                     positive = 1254269,
--                     negative = 30467,
--                     estimated_owners = '20000000 - 50000000',
--                     average_playtime_forever = 7451,
--                     peak_ccu = 23331,
--                     pct_pos_total = 97,
--                     num_reviews_total = 1026239;