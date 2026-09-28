using StardewModdingAPI;
using StardewValley;

namespace SmartFellasMod
{
    /// <summary>
    /// Holds the command used to warp to BaseCamp, 
    /// This functionality isn't required but it makes testing things faster as we don't have to walk there
    /// </summary>
    internal sealed class WarpCommands
    {
        private readonly IMonitor Monitor;

        public WarpCommands(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// Warps the player to the basecamp location, can be modified to send them somewhere based on command arguments
        /// </summary>
        /// <param name="command">player_warp_test is the command that is typed into the console to call this</param>
        /// <param name="args">arguments that follow the command</param>
        public void WarpToCustomArea(string command, string[] args)
        {
            // checl that a save is loadd so the game doesn't crash
            if (!Context.IsWorldReady)
            {
                this.Monitor.Log("You must load a save before using this command.", LogLevel.Warn);
                return;
            }

            // currently hardcoded to warp to basecamp
            string locationName = $"{ModConstants.ContentPackId}_BaseCamp";

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
    }
}