# Fun To Match

### Playing the Game

To play the game, use the mouse to click on a tile to select it, and then select an adjacent tile to swap positions. If the new position creates a match, the game destroys the matched tiles and adds points to your total score. The game also keeps track of the number of moves you make, which sets it apart from normal Match 3 games where players have a limited number of moves.

There's a button called "Simulate" which will execute 30 random moves. The number of simulated moves can be adjusted in the Inspector within the "Grid" game object.

### The Code

To organize the scripts and assets, I placed them all in the folder /Assets/MatchToFun/Scripts. All the scripts used were created during the development of this game. Pre-made assets were used for the UI.

Among the created scripts, the most important ones include:

- GridManager (responsible for creating the grid, matching tiles, controlling tile movement logic and simulating random tile movement)
- TileItem (sets the color of the tile and manages tile movement)
- GameManager (controls UI interactions)

### Thought Process

I began by implementing the grid spawn logic. Once the grid was created and all tiles with colors were displayed on the screen, I implemented the swapping logic, followed by the matching logic. Then, I implemented the logic for tiles 'falling' and concluded with the simulation of random movements.

I tested each feature as I developed them and conducted a comprehensive test at the end after creating the build.

### My Performance

I am quite satisfied with the final result, as I managed to accomplish everything proposed by the challenge. However, there is always room for improvement. If I were to continue working on this project, I would add:

- Stage selection
- New logic for complex matches and power-ups
- Improvement on the UI, animations, and interactions.
