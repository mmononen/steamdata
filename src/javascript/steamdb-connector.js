const mysql = require("mysql2");
const fs = require("fs");
const path = require("path");

const configPath = path.join(__dirname, '../../db-config.json');
const config = JSON.parse(fs.readFileSync(configPath, "utf8"));

if (config.ssl && config.ssl["ssl-mode"] === "REQUIRED") {
    config.ssl = {
        rejectUnauthorized: true,
    };
}

const connection = mysql.createConnection(config);

function fetchAllGames(limit, callback) {
    const query = `
        SELECT ig.appid, ig.name, ig.release_date, ig.price, ig.metacritic_score, ig.recommendations,
               GROUP_CONCAT(DISTINCT c.category_name ORDER BY c.category_name) AS categories,
               GROUP_CONCAT(DISTINCT g.genre_name ORDER BY g.genre_name) AS genres,
               ig.positive, ig.negative, ig.estimated_owners, ig.average_playtime_forever, ig.peak_ccu,
               GROUP_CONCAT(DISTINCT t.tag_name ORDER BY t.tag_name) AS tags,
               ig.pct_pos_total, ig.num_reviews_total
        FROM indie_games ig
        LEFT JOIN game_categories gc ON ig.appid = gc.appid
        LEFT JOIN categories c ON gc.category_id = c.category_id
        LEFT JOIN game_genres gg ON ig.appid = gg.appid
        LEFT JOIN genres g ON gg.genre_id = g.genre_id
        LEFT JOIN game_tags gt ON ig.appid = gt.appid
        LEFT JOIN tags t ON gt.tag_id = t.tag_id
        GROUP BY ig.appid
        LIMIT ?;
    `;
    connection.query(query, [limit], callback);
}


function fetchGameByAppid(appid, callback) {
    const query = `
        SELECT ig.appid, ig.name, ig.release_date, ig.price,
               GROUP_CONCAT(DISTINCT c.category_name ORDER BY c.category_name) AS categories,
               GROUP_CONCAT(DISTINCT g.genre_name ORDER BY g.genre_name) AS genres,
               GROUP_CONCAT(DISTINCT t.tag_name ORDER BY t.tag_name) AS tags
        FROM indie_games ig
        LEFT JOIN game_categories gc ON ig.appid = gc.appid
        LEFT JOIN categories c ON gc.category_id = c.category_id
        LEFT JOIN game_genres gg ON ig.appid = gg.appid
        LEFT JOIN genres g ON gg.genre_id = g.genre_id
        LEFT JOIN game_tags gt ON ig.appid = gt.appid
        LEFT JOIN tags t ON gt.tag_id = t.tag_id
        WHERE ig.appid = ?
        GROUP BY ig.appid;
    `;
    connection.query(query, [appid], (err, results) => {
        if (err) {
            console.error("Error executing query:", err);
            callback(err, null);
            return;
        }
        // console.log("Query results:", results);
        callback(null, results[0]);
	});
}

function searchGames(filters, callback) {
    const { name, category, genre, tag } = filters;
    let query = `
        SELECT ig.appid, ig.name, ig.release_date, ig.price,
               GROUP_CONCAT(DISTINCT c.category_name ORDER BY c.category_name) AS categories,
               GROUP_CONCAT(DISTINCT g.genre_name ORDER BY g.genre_name) AS genres,
               GROUP_CONCAT(DISTINCT t.tag_name ORDER BY t.tag_name) AS tags
        FROM indie_games ig
        LEFT JOIN game_categories gc ON ig.appid = gc.appid
        LEFT JOIN categories c ON gc.category_id = c.category_id
        LEFT JOIN game_genres gg ON ig.appid = gg.appid
        LEFT JOIN genres g ON gg.genre_id = g.genre_id
        LEFT JOIN game_tags gt ON ig.appid = gt.appid
        LEFT JOIN tags t ON gt.tag_id = t.tag_id
        WHERE 1=1
    `;
    const queryParams = [];

    if (name) {
        query += ` AND ig.name LIKE ?`;
        queryParams.push(`%${name}%`);
    }

    if (category) {
        query += ` AND c.category_name LIKE ?`;
        queryParams.push(`%${category}%`);
    }

    if (genre) {
        query += ` AND g.genre_name LIKE ?`;
        queryParams.push(`%${genre}%`);
    }

    if (tag) {
        query += ` AND t.tag_name LIKE ?`;
        queryParams.push(`%${tag}%`);
    }

    query += ` GROUP BY ig.appid;`;

    connection.query(query, queryParams, callback);
}

// Fetch all categories
function fetchCategories(callback) {
    const query = 'SELECT category_name FROM categories ORDER BY category_name;';
    connection.query(query, (err, results) => {
        if (err) {
            callback(err, null);
        } else {
            const categories = results.map(result => result.category_name);
            callback(null, categories);
        }
    });
}

// Fetch all genres
function fetchGenres(callback) {
    const query = 'SELECT genre_name FROM genres ORDER BY genre_name;';
    connection.query(query, (err, results) => {
        if (err) {
            callback(err, null);
        } else {
            const genres = results.map(result => result.genre_name);
            callback(null, genres);
        }
    });
}

// Fetch all tags
function fetchTags(callback) {
    const query = 'SELECT tag_name FROM tags ORDER BY tag_name;';
    connection.query(query, (err, results) => {
        if (err) {
            callback(err, null);
        } else {
            const tags = results.map(result => result.tag_name);
            callback(null, tags);
        }
    });
}

module.exports = {
    fetchAllGames,
    fetchGameByAppid,
    searchGames,
    fetchCategories,
    fetchGenres,
    fetchTags
};
