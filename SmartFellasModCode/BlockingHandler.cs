using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFellasMod
{
    internal sealed class BlockingHandler
    {
        private readonly IMonitor Monitor;

        public BlockingHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// Called once the player moves into a new area, spawns blocking rock in if condition isn't met.
        /// </summary>
        public void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            return;
        }


    }
}
