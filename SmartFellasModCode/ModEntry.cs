using StardewModdingAPI;

namespace SmartFellasMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private OxygenManager oxygenManager;
        private WarpCommands warpCommands;
        private PlayerWarpHandler playerWarpHandler;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            oxygenManager = new OxygenManager(this.Monitor);
            warpCommands = new WarpCommands(this.Monitor);
            playerWarpHandler = new PlayerWarpHandler(this.Monitor);

            // subscribe into the game loop updates
            helper.Events.GameLoop.UpdateTicked += oxygenManager.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += oxygenManager.OnSaveLoaded;
            helper.Events.Player.Warped += playerWarpHandler.OnPlayerWarped;


            // Create the console command
            helper.ConsoleCommands.Add(
                name: "set_oxygen",
                documentation: "Sets the current oxygen deprivation level.\n\nUsage: set_oxygen <value>",
                callback: oxygenManager.HandleSetOxygen
            );

            // used to teleport to new area BaseCamp
            helper.ConsoleCommands.Add("player_warp_camp", "Warps the player to your custom area.\n\nUsage: player_warp_camp", warpCommands.WarpToCustomArea);

        }
    }
}