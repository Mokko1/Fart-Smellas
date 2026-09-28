using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Quests;
using System;

namespace SmartFellasMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // subscribe into the game loop updates
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Player.Warped += OnPlayerWarped;


            // Create the console command
            helper.ConsoleCommands.Add(
                name: "set_oxygen",
                documentation: "Sets the current oxygen deprivation level.\n\nUsage: set_oxygen <value>",
                callback: this.HandleSetOxygen
            );

            // used to teleport to new area BaseCamp
            helper.ConsoleCommands.Add("player_warp_test", "Warps the player to your custom area.\n\nUsage: player_warp_test", this.WarpToCustomArea);

        }


        /*********
        ** Private Fields
        *********/

        // code used to retrieve stuff from the content pack
        private const string ContentPackId = "SmartFellas.AscensionCode.Content";

        // tracks how much stamina the player had last tick
        private float lastStamina;

        // higher the less oxygen the player has access to, used to add more energy lost 
        // its minimum is 0 and max is 1
        private float oxygenDeprivation = 0;

        private float OxygenDeprivation { get { return oxygenDeprivation; }
            set {

                //ensure oxygenDeprivation is assigned correctly
                if (value >= 0 && value <= 2) {
                    oxygenDeprivation = value;
                } 
            } }

        /*********
        ** Private methods
        *********/

        /// <summary>
        /// called once the save data for a game has been loaded
        /// </summary>
        /// <param name="sender">object that emitted the SaveLoaded event</param>
        /// <param name="e"> event arguments </param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // initialize stamina tracking
            lastStamina = Game1.player.Stamina;
        }

        /// <summary>
        /// Warps the player to the basecamp location, can be modified to send them somewhere based on command arguments
        /// </summary>
        /// <param name="command">player_warp_test is the command that is typed into the console to call this</param>
        /// <param name="args">arguments that follow the command</param>
        private void WarpToCustomArea(string command, string[] args)
        {
            // checl that a save is loadd so the game doesn't crash
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("You must load a save before using this command.", LogLevel.Warn);
                return;
            }

            // currently hardcoded to warp to basecamp
            string locationName = $"{ContentPackId}_BaseCamp";

            // checks if this warp would fail
            if (Game1.getLocationFromName(locationName) == null)
            {
                // informs the user of where the code attempted tosend them to help with debugging
                this.Monitor.Log($"Could not find location '{locationName}'. Is your Content Patcher pack loaded correctly?", LogLevel.Error);
                return;
            }

            Game1.warpFarmer(locationName, 5, 5, false);
            this.Monitor.Log($"Warped to {locationName}!", LogLevel.Info);
        }

        /// <summary>
        /// essentially, an update function called for every tick of the game
        /// </summary>
        /// <param name="sender"> object that emitted the UpdateTicked event</param>
        /// <param name="e"> tick params </param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore updates if the player hasn't loaded in yet
            if (!Context.IsWorldReady || Game1.player == null)
                return;

            AdjustStaminaForOxygen();
        }

        /// <summary>
        /// checks if the player used any energy, if they did adds additional energy drain for their current oxygen level
        /// </summary>
        private void AdjustStaminaForOxygen()
        {
            float currentStamina = Game1.player.Stamina;

            // Check if stamina decreased during this tick
            if (currentStamina < lastStamina)
            {
                float energyLost = lastStamina - currentStamina;

                // add additional energy loss if oxygenDeprivation is above 0
                float adjustedLoss = energyLost * oxygenDeprivation;
                Game1.player.Stamina -= adjustedLoss;

                // Update tracker 
                lastStamina = Game1.player.Stamina;

                // show how much energy they have, used to track how much is lost
                this.Monitor.Log($"Energy: {lastStamina}", LogLevel.Info);

            }
            else
            {
                // Update tracker normally if stamina stayed the same or increased
                lastStamina = currentStamina;
            }
        }

        /// <summary>
        /// Sets the player's oxygen to a specific number for testing
        /// </summary>
        /// <param name="command">set_oxygen is the command used to call this</param>
        /// <param name="args"> the number the player is trying to set the oxygen to</param>
        private void HandleSetOxygen(string command, string[] args)
        {
            // check the user provided an argument
            if (args.Length == 0)
            {
                this.Monitor.Log("You must specify a float value. Example: set_oxygen 0.5", LogLevel.Error);
                return;
            }

            // Try parsing the input to a float
            if (float.TryParse(args[0], out float newValue))
            {
                // success
                OxygenDeprivation = newValue;
                this.Monitor.Log($"oxygenDeprivation successfully set to: {oxygenDeprivation}", LogLevel.Info);
            }
            else
            {
                this.Monitor.Log($"'{args[0]}' is not a valid float number.", LogLevel.Error);
            }
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

            if (warpedEvent.NewLocation.Name == $"{ContentPackId}_Climb")
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