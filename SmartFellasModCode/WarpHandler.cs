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
            //First, create a new custom monster.
            //We will use a stone golem for starters.
            Monster testMonster = new RockGolem(new Vector2(15, 6));

            //Verification that the monster exists.
            this.Monitor.Log($"{testMonster.Name}", LogLevel.Debug);

            //Then, we need to add this monster to the map itself.
            Game1.currentLocation.addCharacter(testMonster);
        }
    }
}