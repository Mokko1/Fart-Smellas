using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System.Collections.Generic;
using xTile.Dimensions;

namespace SmartFellasMod
{
    /// <summary>
    /// Handles checks related to the player going to an area
    /// </summary>
    internal sealed class PlayerWarpHandler
    {
        public Dictionary<String, bool> locationsSpawned = new Dictionary<string, bool>();

        private readonly IMonitor Monitor;

        public PlayerWarpHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// Reset enemy spawns at the start of every day
        /// </summary>
        public void OnDayStarted(object? sender, DayStartedEventArgs dayStartedEvent)
        {
            if (locationsSpawned.Count == 0)
                foreach (String location in ModConstants.locationNames)
                    locationsSpawned.Add(location, false);
            else
                foreach (String location in ModConstants.locationNames)
                    locationsSpawned[location] = false;
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

            // when going to a new map, check if that map has an oxygen deprivation level property
            if (warpedEvent.NewLocation.TryGetMapPropertyAs("OxygenDeprivation", out double deprivationValue, required: false))
            {
                OxygenManager.SetPlayerOxygen((float)deprivationValue);
            }
            else
            {
                // if there is no property on this map, assume normal oxygen levels
                OxygenManager.SetPlayerOxygen(0f);

            }

            //Handle the spawning of monsters in new mountain maps.
            if (warpedEvent.NewLocation.Name == $"{ModConstants.ContentPackId}_Mountain_1" && !locationsSpawned["Mountain_1"])
            {
                locationsSpawned["Mountain_1"] = true;
                SpawnFirstMonsters();
            }
            else if (warpedEvent.NewLocation.Name == $"{ModConstants.ContentPackId}_Mountain_2" && !locationsSpawned["Mountain_2"])
            {
                locationsSpawned["Mountain_2"] = true;
                SpawnSecondMonsters();
            }
            else if (warpedEvent.NewLocation.Name == $"{ModConstants.ContentPackId}_Mountain_Peak" && !locationsSpawned["Mountain_Peak"])
            {
                locationsSpawned["Mountain_Peak"] = true;
                SpawnThirdMonsters();
            }
        }

        /// <summary>
        /// Spawn custom monsters in the first mountain level(past Base Camp) at manually chosen positions.
        /// </summary>
        public void SpawnFirstMonsters()
        {
            Monitor.Log("Player Position: " + Game1.player.Position, LogLevel.Debug);
            //Player starts off Mountain Level at X:704 Y:1872.

            SpawnRockGolem(1500, 1296);
            SpawnRockGolem(1100, 1400);
            SpawnRockGolem(1500, 800);
            SpawnRockGolem(1800, 600);
            SpawnRockGolem(2600, 580);
        }

        /// <summary>
        /// Spawn custom monsters in the second mountain level.
        /// </summary>
        public void SpawnSecondMonsters()
        {
            Monitor.Log("Player Position: " + Game1.player.Position, LogLevel.Debug);
            //Player starts off Mountain 2 map at X:2496 Y:1936

            SpawnRockGolem(2400, 1600);
            SpawnRockGolem(2000, 1700);
            SpawnRockGolem(2100, 650);
            SpawnRockGolem(1600, 1200);
            SpawnRockGolem(1300, 650);
            SpawnRockGolem(600, 800);
            SpawnRockGolem(600, 250);
        }

        /// <summary>
        /// Same drill as above but for the third mountain map.
        /// </summary>
        public void SpawnThirdMonsters()
        {
            Monitor.Log("Player Position: " + Game1.player.Position, LogLevel.Debug);
            //Player starts off Mountain 2 map at X:1344 Y:1936

            SpawnRockGolem(1350, 1650);
            SpawnRockGolem(850, 1800);
            SpawnRockGolem(2000, 1750);
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