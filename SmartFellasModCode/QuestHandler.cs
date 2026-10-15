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
    internal sealed class QuestHandler
    {
        private readonly IMonitor Monitor;

        // CURRENTLY THIS FILE DOES NOTHING


        public QuestHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// called once the player moves into a new area, spawns blocking rock in if condition isn't met.
        /// </summary>
        public void Warped(object sender, WarpedEventArgs e)
        {
            GameLocation location = Game1.getLocationFromName("Backwoods");
            return;
            if (e.NewLocation != location) return;        
            
            // Position of the rocks that initally makes the quest inaccessible
            Vector2[] pos = { new Vector2(24, 6), new Vector2(25, 6), new Vector2(26, 6) };
            
            location.Objects[pos[0]] = new StardewValley.Object(pos[0], "78");
            location.Objects[pos[1]] = new StardewValley.Object(pos[1], "78");
            location.Objects[pos[2]] = new StardewValley.Object(pos[2], "78");
        }


    }
}
