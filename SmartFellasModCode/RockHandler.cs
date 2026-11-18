using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Linq;

namespace SmartFellasMod
{
    internal sealed class RockHandler
    {
        // used to ensure spawning and moving works correctly
        private const string RockItemId = "SmartFellas.AscensionCode.Content_magic_rock";
        private const string RockQualifiedId = "(F)" + RockItemId;
        private const string RockLocationName = "SmartFellas.AscensionCode.Content_Mountain_Peak";
        private const string SpawnedFlag = "SmartFellas.AscensionCode/RockSpawned";
        
        private readonly IMonitor Monitor;

        // used to animate the rock
        private const int FrameSize = 16;
        private const int FrameCount = 7;
        private const int TicksPerFrame = 12;
        private const int InventoryFrame = 4; 

        private static readonly Rectangle InventoryRect =
            new(InventoryFrame * FrameSize, 0, FrameSize, FrameSize);

        private static readonly Rectangle IdleRect = new(0, 0, FrameSize, FrameSize);


        public RockHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// make sure that the soil around any rock in the world is fertilized at the beginning of the day
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            Utility.ForEachLocation(location =>
            {
                foreach (Furniture furniture in location.furniture)
                {
                    if (furniture.QualifiedItemId == RockQualifiedId)
                        FertilizeTiles(location, furniture.TileLocation);
                }
                return true; // keep iterating
            });
        }

        public void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer || e.NewLocation.Name != RockLocationName) return;

            // flag survives the rock being picked up and moved
            if (e.NewLocation.modData.ContainsKey(SpawnedFlag)) return;

            Furniture magicRock = ItemRegistry.Create<Furniture>(RockQualifiedId);
            magicRock.SetPlacement(25, 11);
            e.NewLocation.furniture.Add(magicRock);
            e.NewLocation.modData[SpawnedFlag] = "true";
            this.Monitor.Log("Spawned magic rock.", LogLevel.Debug);
        }

        public void OnInventoryChanged(object sender, StardewModdingAPI.Events.InventoryChangedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            // Check newly added items
            foreach (var item in e.Added)
            {
                // Example: check item ID or name
                this.Monitor.Log($"Picked up item: {item.DisplayName} (ID: {item.QualifiedItemId}) x{item.Stack}", StardewModdingAPI.LogLevel.Info);
                if (item.QualifiedItemId == "(F)SmartFellas.AscensionCode.Content_magic_rock")
                {
                    this.Monitor.Log($"aga", StardewModdingAPI.LogLevel.Info);
                    Item itemToHold = Game1.player.CurrentItem;
                    TriggerPickupAnimation(Game1.player, itemToHold);
                }
            }
           

            // Check quantity increases on existing stackable items
            foreach (var change in e.QuantityChanged)
            {
                int diff = change.NewSize - change.OldSize;
                if (diff > 0)
                {
                    this.Monitor.Log($"Increased stack for: {change.Item.DisplayName} by {diff}", StardewModdingAPI.LogLevel.Info);
                }
            }
        }


        public void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            // keep the icon on the inventory frame while the rock is carried
            if (e.IsMultipleOf(4))
            {
                foreach (Item item in Game1.player.Items)
                {
                    if (item is Furniture f && f.QualifiedItemId == RockQualifiedId && f.sourceRect.Value != InventoryRect)
                        f.sourceRect.Value = InventoryRect;
                }
            }

            // animate rocks placed in the current location
            var location = Game1.currentLocation;
            if (location is null || location.furniture.Count == 0) return;

            // ping-pong: 0,1,2,3,4,5,6,5,4,3,2,1, then repeat
            int cycle = 2 * (FrameCount - 1);
            int step = (Game1.ticks / TicksPerFrame) % cycle;
            int frame = step < FrameCount ? step : cycle - step;
            var rect = new Rectangle(frame * FrameSize, 0, FrameSize, FrameSize);

            foreach (Furniture furniture in location.furniture)
            {
                if (furniture.QualifiedItemId == RockQualifiedId && furniture.sourceRect.Value != rect)
                    furniture.sourceRect.Value = rect;
            }
        }

        private void FertilizeTiles(GameLocation location, Vector2 tile)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var pos = new Vector2(tile.X + i, tile.Y + j);
                    if (location.terrainFeatures.TryGetValue(pos, out var feature) && feature is HoeDirt dirt)
                        dirt.fertilizer.Value = "DeluxeFertilizer";
                }
            }
        }
        public void TriggerPickupAnimation(Farmer player, Item item)
        {
            player.completelyStopAnimatingOrDoingAction();
            player.faceDirection(Game1.down); // Optional: force player to face forward
            player.FarmerSprite.animateOnce(130,150f,4);
            // This forces the temporary frame and locks player control for the duration

            player.currentLocation.playSound("jingle1");
        }
    }
}