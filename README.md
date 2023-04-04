# Tower Defense game

## **Plannig Process**

I started defining the main elements that I could think in the game. After that, I started to write things that could be part of theses main elements.

![Managers](/Readme assets/planning process.jpg)

This game is heavily event and data driven. I used a lot of events to uncouple the code. So I used a lot of the Observer Pattern, together with the Component Pattern.
With these two patterns I was able to have an Enemy that has a movement component and the movement component doesn't know anything about the code of, for instance, health and attack.
If I need communication between components, I used events to handle it.

## **Scalability**
The project uses Scriptable objects, Prefabs variants and it's very Data Driven so Game Designers and Developers and scale the game without even write code, but just creating new instances and changing values.

### Prefabs and Prefabs variants
The Health bar is a good example of how the game uses prefabs do make scalability easy. Plus, enemies and projectiles shows how prefab variants are useful to the same objective and bring some diversity.

### Scriptable objects
I used scriptable objects to make the Shop easy to expand. You only have to create a piece of data (`DefenseData`) to create a new defense at the shop.

### Adjusting variables

## **Game Start Process**
1. The `GameManager` starts all player's economy 
2. The `GridsManager` generates a random start and end positions. Then a random path from start to end.
3. The `PlayerMainTower` places the main tower at the end position and initializes itself.
4. The `EnemyManager` sets the enemy spawn origin to the start position and initialize all the enemy's pools.
5. The `WaveManager` starts to spawn the waves over time.

## **Managers**

![Managers](/Readme assets/main components.png)

### **GameManager**

The GameManager holds some key events Shortcut and link for handle player points, coins and kills. It is a singleton to keep its access easy.

### **LeaderboardManager**

Just Handles the leaderboard logic. It subscribes at OnChangeValue of points score (from [GameManager](#gamemanager))
and updates the leaderboard. The score is stored in a Json file and saved on `Application.persistentDataPath`.

### **WaveManager**

The Wave Manager handles the spawn of each wave. The waves spawns has a constant time to spawn (`_timeToSpawn`), but the
number of enemies grows exponentially with the elapsed time of the game. You can control how much it grows from the
inspector using:

- `_intensity`
- `_initialEnemies`

It also has a reference to `WaveHUD` to display to the player how much time is left to that wave and how many enemies
are remaining (including the last wave)

### **EnemyManager**

Spawns all enemies in a [Pool](#objectpool) using `ObjectPool<Enemy>`for each type of enemy. All the enemy pools are stored in a pool dictionary.
The Enemy inheritance helps a lot here, because the pool dictionary can only have one type: Enemy.
The EnemyManager also handles some Events setup for enemies every time the spawn, like set the movement target position and disable the game object when the enemy dies.

### **GridManager**
Selects
- `width`
- `height`
- `tileWidth`
- `tileHeight`
- `borderThickness`
- `originTransform`

## **Enemies**

![Managers](/Readme assets/enemy.png)

The enemies are based on the component patterns where which component has a function and they don't know about the others. The `NavMeshMovement` and the `Health`components shows how I used this patterns. The both work without intercode.
I also used `Health` for the `PlayerMainTower` which is a child class of `Tower` whose prefab has health.

## **Pathfinding**
For the enemy movement I used the `NavmeshAgent`, a Unity built in tool to handle pathfinding. It's super simple and easy to use, perfect for this short project.
I baked the area where the enemies could walk (this helps to avoid some real time calculations) and made the "NotWalkable" tiles dynamic for this bake.

## **Towers and Projectiles**

### Towers
The towers uses inheritance to handle the different types of towers. The `PlayerMainTower` and the regular tower. The `Tower` class has variables for the game designer to change and balance the game the way he needs
- `rangeUpgradeMultiplier`
- `damageUpgradeMultiplier`
- `fireRateUpgradeMultiplier`
- `costUpgradeMultiplier`
- `attackRange`
- `damage`
- `fireRate`
- `rotationSpeed`
- `shootPrefab`

You can create new towers creating new variants of the regular tower prefab. Just add the model and change the attributes.

### Projectiles
The projectile handles the effects of the game. We have 3 types: normal, freeze and area. It only need a target position and it will follow in a certain speed. 
When the projectile collides with something, handle the assigned effect and fire some events, like damage.

You can create new projectiles creating new variants of the main prefab.

## **Economy**

### **Store**
The stores is prepared for new towers. It has a list of `DefenseData`, a `ScriptableObject` which contains only data about the tower to buy, like
`defenseName`, `defenseCost`, `defenseCostMultiplier`, `defenseDescription` and `prefab`.

### Player Score
I created a class `PlayerScore` to handle scores like Points, Coin, Kills and Wave Number. The all use the same base and events. I'm able to create more scores if needed just adding the `PlayerScore` component.

To add new towers, just create a new `DefenseData` from the menu `Defenses->New Defense Data` (ScriptableObject), fill all the fields and add to the `AllDefenseData` list.
## **QA Testing and Utils**

### Shortcuts
You can set the game speed by pressing 1-6 and speed up or down.

### Custom Editors
I did some CustomEditors to make it easy to test and find bugs in some features.

#### Change Player Score Editor
You can give player coins to test new towers sooner

![Managers](/Readme assets/player coins.png)

#### Health Editor
You can change the health of any game object that has health, player and enemies. The health bar listens the OnChangeValue event.
![Managers](/Readme assets/health editor.png)

### ObjectPool
A template for a pool of objects. Avoids Instantiating and Destroying objects to better performance. It has a maximum size and spawn all the objects on initialization.
Used for 
- `Enemy`
- `Projectile`
- All numbers that pop-ups over the enemies and the towers

## Next Steps
+ Balance the game 
+ Change all the 3D models
+ Add new towers
+ Add new Enemies
+ Add sound effects and BGM
+ Add visual effects
+ Add options like pause, etc