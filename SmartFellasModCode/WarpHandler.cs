using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace SmartFellasMod
{
    /// <summary>
    /// Handles checks related to the player going to an area
    /// </summary>
    internal sealed class PlayerWarpHandler
    {
        private readonly IMonitor Monitor;

        private Monster[] monsters = new Monster[10];

        public PlayerWarpHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// Runs anytime the player is warped to a new location (travels there)
        /// Triggered to check for sending mail to start the mod questline.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="warpedEvent">The event that stores the arguments for the player being warped</param>
        public void OnPlayerWarped(object? sender, WarpedEventArgs warpedEvent)
        {
            if (warpedEvent.NewLocation is MineShaft mine)
            {
                // When the player reaches the tenth level in the mines, send the mail to start the quest
                if (mine.mineLevel == 10)
                    if (!Game1.player.hasOrWillReceiveMail("ascension_mine_letter"))
                        Game1.addMailForTomorrow("ascension_mine_letter");
            }

            //Thor - Added this line in order to know exactly where the player is in our mod so I can add things right.
            this.Monitor.Log($"{warpedEvent.NewLocation.Name}", LogLevel.Debug);

            if (warpedEvent.NewLocation.Name == $"{ModConstants.ContentPackId}_BaseCamp")
            {
                SpawnCustomMonsters();
            }
        }


        public void SpawnCustomMonsters()
        {
            //Monitor.Log("Attempting to spawn custom monsters.", LogLevel.Debug);

            //Spawn 3 Rock Golems for now.
            //Note for positioning, Player starts off at 768, 1296.
            SpawnRockGolem(1000, 1296);
            SpawnRockGolem(2000, 1650);
            SpawnRockGolem(2700, 1500);
        }

        /// <summary>
        /// Spawn in a Rock Golem at a set location.
        /// </summary>
        /// <param name="xPos">The X position where the golem spawns.</param>
        /// <param name="yPos">The starting Y position of the golem.</param>
        private void SpawnRockGolem(int xPos, int yPos)
        {
            //Spawn a rock golem at a determined location.
            Monster mon = new RockGolem(new Vector2(xPos, yPos));

            //Add the golem to the game scene.
            Game1.currentLocation.addCharacter(mon);
        }
    }
}