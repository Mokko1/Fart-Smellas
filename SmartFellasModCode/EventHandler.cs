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
    internal class EventHandler
    {
        private readonly IMonitor Monitor;
        public EventHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }
        public void OnPlayerWarped(object? sender, WarpedEventArgs warpedEvent)
        {
            if (warpedEvent.NewLocation.Name == "SmartFellas.AscensionCode.Content_BaseCamp")
            {
                if (!Game1.player.eventsSeen.Contains("basecamp_scene_one_1022"))
                {
                    Event baseCampEvent = new Event("basecamp_scene_one_1022");
                    Game1.currentLocation.startEvent(baseCampEvent);
                    // The player has already watched this event

                }
            }
        }
}

}
