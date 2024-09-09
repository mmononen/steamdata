// Function to display results dynamically in HTML format
function displayResult(data) {
	const resultElement = document.getElementById("result");

	// Clear previous results
	resultElement.innerHTML = "";

	// Check if data is an array (for multiple games) or an object (for a single game)
	if (Array.isArray(data)) {
		data.slice(0, 10).forEach((game) => {
			const gameElement = document.createElement("div");
			gameElement.classList.add("game");

			gameElement.innerHTML = `
                <div class="game-title">${game.name}</div>
                <p><strong>AppID:</strong> ${game.appid}</p>
                <p><strong>Release Date:</strong> ${game.release_date}</p>
                <p><strong>Genres:</strong> ${game.genres}</p>
                <p><strong>Tags:</strong> ${game.tags}</p>
                <p><strong>Positive Rating (Total):</strong> ${game.pct_pos_total}%</p>
                <p><strong>Positive Rating (Recent):</strong> ${game.pct_pos_recent}%</p>
                <p><strong>Price:</strong> $${game.price}</p>
            `;

			resultElement.appendChild(gameElement);
		});
	} else {
		// Display a single game (for search by AppID)
		const gameElement = document.createElement("div");
		gameElement.classList.add("game");

		gameElement.innerHTML = `
            <div class="game-title">${data.name}</div>
            <p><strong>AppID:</strong> ${data.appid}</p>
            <p><strong>Release Date:</strong> ${data.release_date}</p>
            <p><strong>Genres:</strong> ${data.genres}</p>
            <p><strong>Tags:</strong> ${data.tags}</p>
            <p><strong>Positive Rating (Total):</strong> ${data.pct_pos_total}%</p>
            <p><strong>Positive Rating (Recent):</strong> ${data.pct_pos_recent}%</p>
            <p><strong>Price:</strong> $${data.price}</p>
        `;

		resultElement.appendChild(gameElement);
	}
}

// Fetch all games, but display only the first 10
async function fetchFirst10Games() {
	try {
		const response = await fetch("http://localhost:3000/games");
		const data = await response.json();

		// Display the first 10 games dynamically
		displayResult(data);
	} catch (error) {
		console.error("Error fetching all games:", error);
	}
}

// Search by AppID entered in the input field
async function searchByAppId() {
	const appid = document.getElementById("appid").value;

	if (!appid) {
		alert("Please enter an AppID!");
		return;
	}

	try {
		const response = await fetch(`http://localhost:3000/games/${appid}`);
		const data = await response.json();

		// Display the fetched game dynamically
		displayResult(data);
	} catch (error) {
		console.error("Error fetching game by appid:", error);
	}
}
