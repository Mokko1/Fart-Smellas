using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SmartFellasMod
{
    /// <summary>
    /// Handles checks related to oxygen and draining additional energy
    /// </summary>
    internal sealed class OxygenManager
    {
        private readonly IMonitor Monitor;

        public OxygenManager(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /*********
        ** Private Fields
        *********/

        // tracks how much stamina the player had last tick
        private float lastStamina;

        // higher the less oxygen the player has access to, used to add more energy lost 
        // its minimum is 0 and max is 1
        private float oxygenDeprivation = 0;

        private float OxygenDeprivation
        {
            get { return oxygenDeprivation; }
            set
            {

                //ensure oxygenDeprivation is assigned correctly
                if (value >= 0 && value <= 2)
                {
                    oxygenDeprivation = value;
                }
            }
        }

        /*********
        ** Methods
        *********/

        /// <summary>
        /// called once the save data for a game has been loaded
        /// </summary>
        /// <param name="sender">object that emitted the SaveLoaded event</param>
        /// <param name="e"> event arguments </param>
        public void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // initialize stamina tracking
            lastStamina = Game1.player.Stamina;
        }

        /// <summary>
        /// essentially, an update function called for every tick of the game
        /// </summary>
        /// <param name="sender"> object that emitted the UpdateTicked event</param>
        /// <param name="e"> tick params </param>
        public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
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
        public void HandleSetOxygen(string command, string[] args)
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