using StardewValley;

namespace SmartFellasMod
{
    /// <summary>
    /// Tracks the loading state of the map in regards to enemies for loading in path blockers.
    /// </summary>
    enum EnemyLoadingState {
        UNLOADED,
        LOADING,
        LOADED
    }
    /// <summary>
    /// Holds globals multiple scripts may need to use
    /// </summary>
    internal static class ModConstants
    {

        // code used to retrieve stuff from the content pack
        public const string ContentPackId = "SmartFellas.AscensionCode.Content";
        public static String[] locationNames = { "Buffer", "BaseCamp", "Mountain_1", "Mountain_2", "Mountain_Peak" };

        private static EnemyLoadingState enemyMapLoaded;
        /// <summary>
        /// A bool that's true when a map that has enemies on it has loaded (doesn't matter if the map will spawn enemies for this to be true.)
        /// </summary>
        public static EnemyLoadingState EnemyMapLoaded { get { return enemyMapLoaded; } set { enemyMapLoaded = value; } }
    }
}