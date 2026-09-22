using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Quests;

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

            // Register the console command
            helper.ConsoleCommands.Add(
                name: "set_oxygen",
                documentation: "Sets the current oxygen deprivation level.\n\nUsage: set_oxygen <value>",
                callback: this.HandleSetOxygen
            );

            // used to teleport to new area
            helper.ConsoleCommands.Add("player_warp_test", "Warps the player to your custom area.\n\nUsage: player_warp_test", this.WarpToCustomArea);

        }


        private void WarpToCustomArea(string command, string[] args)
        {
            // Check if a save is loaded so the game doesn't crash
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("You must load a save before using this command.", LogLevel.Warn);
                return;
            }

            // Replace "YourCustomMapName" with the string ID of your custom area
            // and provide destination tile coordinates (e.g., X: 5, Y: 5)
            Game1.warpFarmer("SmartFellas.Ascension_test", 5, 5, false);

            this.Monitor.Log("Warped to custom area!", LogLevel.Info);
        }

        /*********
        ** Private Fields
        *********/

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
        /// essentially, an update function called for every tick of the game
        /// </summary>
        /// <param name="sender"> object that emitted the UpdateTicked event</param>
        /// <param name="e"> tick params </param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Ignore updates if the player hasn't loaded in yet
            if (!Context.IsWorldReady || Game1.player == null)
                return;

            AdjustStaminaForOxygen();
        }

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
    }
}