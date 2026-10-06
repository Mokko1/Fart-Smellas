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
    internal sealed class RockHandler
    {
        private readonly IMonitor Monitor;

        public RockHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }
        private bool hasEntered = false;

        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {

            //
            if (!Context.IsWorldReady) return;
            var farm = Game1.getFarm();



            foreach (var furniture in farm.furniture)
            {
      
                if (furniture.name == "SmartFellas.AscensionCode.Content_magic_rock")
                {
                    this.Monitor.Log("correct", LogLevel.Info);
                    FertilizeTiles(farm,furniture.TileLocation);
                }


            }
        }

        public void OnPlayerWarped(object? sender, WarpedEventArgs warpedEvent)
        {
            //debug warp SmartFellas.AscensionCode.Content_baseCamp
            Monitor.Log(warpedEvent.NewLocation.Name, LogLevel.Info);
            if(warpedEvent.NewLocation.Name == "SmartFellas.AscensionCode.Content_BaseCamp" && !hasEntered)
            {
                Monitor.Log("entered", LogLevel.Info);
                Furniture magicRock = ItemRegistry.Create<Furniture>("(F)SmartFellas.AscensionCode.Content_magic_rock");
                magicRock.SetPlacement(20,20);
                Game1.getLocationFromName("SmartFellas.AscensionCode.Content_BaseCamp").furniture.Add(magicRock);
                hasEntered = true;
            }
        }

        private void FertilizeTiles(GameLocation location, Vector2 tile)
        {
      

            for(int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Vector2 vector2 = new Vector2(tile.X+i, tile.Y+j);
                    this.Monitor.Log("tile", LogLevel.Info);
                    if (location.terrainFeatures.TryGetValue(vector2, out var feature))
                    {
                        this.Monitor.Log(feature.ToString(), LogLevel.Info);
                        if (feature is HoeDirt dirt)
                        {
                            this.Monitor.Log("yes", LogLevel.Info);
                            dirt.fertilizer.Value = "DeluxeFertilizer";
                        }
                    }
                }
            }
        }

    }
}
