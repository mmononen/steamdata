document.addEventListener('DOMContentLoaded', function () {
  populateDropdown('categories', 'category');
  populateDropdown('genres', 'genre');
  populateDropdown('tags', 'tag');
});

function populateDropdown(endpoint, elementId) {
  fetch(`/api/${endpoint}`)
    .then((response) => response.json())
    .then((data) => {
      const dropdown = document.getElementById(elementId);
      data.forEach((item) => {
        const option = document.createElement('option');
        option.value = item;
        option.textContent = item;
        dropdown.appendChild(option);
      });
    })
    .catch((error) => console.error('Error fetching dropdown data:', error));
}

// Fetch the first 10 games
function fetchFirst10Games() {
  fetch('/api/games?limit=10')
    .then((response) => response.json())
    .then((data) => displayGames(data))
    .catch((error) => console.error('Error:', error));
}

// Consolidated Search Function
function search() {
  const appId = document.getElementById('appid').value;
  const name = document.getElementById('name').value;
  const category = document.getElementById('category').value;
  const genre = document.getElementById('genre').value;
  const tag = document.getElementById('tag').value;

  const queryParams = new URLSearchParams();

  if (appId) queryParams.append('appid', appId);
  if (name) queryParams.append('name', name);
  if (category) queryParams.append('category', category);
  if (genre) queryParams.append('genre', genre);
  if (tag) queryParams.append('tag', tag);

  fetch(`/api/games?${queryParams.toString()}`)
    .then((response) => response.json())
    .then((data) => displayGames(data))
    .catch((error) => console.error('Error:', error));
}

function displayGames(games) {
  const resultDiv = document.getElementById('result');
  resultDiv.innerHTML = ''; // Clear previous results

  if (!games || games.length === 0) {
    resultDiv.innerHTML = '<p>No games found.</p>';
    return;
  }

  const parseAndFormat = (text) => {
    return text ? text.replace(/'/g, '').replace(/,/g, ' / ') : 'N/A';
  };

  const createLinks = (text, type) => {
    if (!text) return 'N/A';
    const baseUrlMap = {
      genre: 'https://store.steampowered.com/genre/',
      tag: 'https://store.steampowered.com/tags/en/',
    };
    const baseUrl = baseUrlMap[type];

    return text
      .split(',')
      .map((item) => {
        const formattedItem = item.replace(/'/g, '').trim();
        return `<a href="${baseUrl}${encodeURIComponent(formattedItem)}" target="_blank" style="color: #008CFF;">
                            ${formattedItem}
                        </a>`;
      })
      .join(' / ');
  };

  games.forEach((game) => {
    const gameDiv = document.createElement('div');
    gameDiv.className = 'game-result';

    // Game Title
    const titleDiv = document.createElement('div');
    titleDiv.className = 'game-title';
    titleDiv.innerHTML = `
            <a href="https://store.steampowered.com/app/${
              game.appid
            }" target="_blank" style="color: #008CFF; font-size: 1.5em; font-weight: bold;">
                ${game.name || 'Unknown Name'}
            </a>
        `;
    gameDiv.appendChild(titleDiv);

    // Categories, Genres, and Tags
    const metaDiv = document.createElement('div');
    metaDiv.className = 'game-meta';
    metaDiv.innerHTML = `
            <div><strong>Categories:</strong> ${parseAndFormat(game.categories)}</div>
            <div><strong>Genres:</strong> ${createLinks(game.genres, 'genre')}</div>
            <div><strong>Tags:</strong> ${createLinks(game.tags, 'tag')}</div>
        `;
    gameDiv.appendChild(metaDiv);

    // Details Table
    const detailsTable = document.createElement('table');
    detailsTable.className = 'game-details';
    detailsTable.innerHTML = `
            <tr><td><strong>Release:</strong></td><td>${
              game.release_date ? new Date(game.release_date).toLocaleDateString() : 'N/A'
            }</td></tr>
            <tr><td><strong>Price:</strong></td><td>$${game.price || 'N/A'}</td></tr>
            <tr><td><strong>MC Score:</strong></td><td>${game.metacritic_score || 'N/A'}</td></tr>
            <tr><td><strong>Recs:</strong></td><td>${game.recommendations || '0'}</td></tr>
            <tr><td><strong>+ Reviews:</strong></td><td>${game.positive || '0'}</td></tr>
            <tr><td><strong>- Reviews:</strong></td><td>${game.negative || '0'}</td></tr>
            <tr><td><strong>Owners:</strong></td><td>${game.estimated_owners || 'N/A'}</td></tr>
            <tr><td><strong>Avg Playtime:</strong></td><td>${game.average_playtime_forever || '0'} mins</td></tr>
            <tr><td><strong>Peak CCU:</strong></td><td>${game.peak_ccu || '0'}</td></tr>
            <tr><td><strong>% Positive:</strong></td><td>${game.pct_pos_total || '0'}%</td></tr>
            <tr><td><strong># Reviews:</strong></td><td>${game.num_reviews_total || '0'}</td></tr>
        `;
    gameDiv.appendChild(detailsTable);

    resultDiv.appendChild(gameDiv);
  });
}
