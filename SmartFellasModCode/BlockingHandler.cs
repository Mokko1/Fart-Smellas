using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace SmartFellasMod
{
    internal sealed class BlockingHandler
    {
        private readonly IMonitor Monitor;
        private int monsterCount;

        public Texture2D skullHUDTexture;
        public Texture2D skullbackgroundHUDTexture;

        public BlockingHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// Called every tick, checking to see if all enemies are clear to remove obstructions that keep player from progressing.
        /// </summary>
        public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) 
                return;

            if (ModConstants.EnemyMapLoaded != EnemyLoadingState.LOADING)
                return;

                if (Game1.currentLocation.Name == ModConstants.ContentPackId + "_Mountain_1")
            {
                monsterCount = Game1.currentLocation.characters.OfType<Monster>().Count();
                if (monsterCount == 0)
                {
                    Vector2[] pos = { new Vector2(38, 4), new Vector2(39, 4), new Vector2(38, 5), new Vector2(39, 5) };
                    if (Game1.currentLocation.hasTileAt((int)pos[0].X, (int)pos[0].Y, "Buildings")) {
                        foreach (Vector2 tile in pos)
                            Game1.currentLocation.removeTile((int)tile.X, (int)tile.Y, "Buildings");
                    }
                    ModConstants.EnemyMapLoaded = EnemyLoadingState.LOADED;
                }
            }
            else if (Game1.currentLocation.Name == ModConstants.ContentPackId + "_Mountain_2")
            {
                monsterCount = Game1.currentLocation.characters.OfType<Monster>().Count();
                if (monsterCount == 0)
                {
                    Vector2[] pos = { new Vector2(8, 2), new Vector2(9, 2), new Vector2(8, 3), new Vector2(9, 3) };
                    if (Game1.currentLocation.hasTileAt((int)pos[0].X, (int)pos[0].Y, "Buildings")) {
                        foreach (Vector2 tile in pos)
                            Game1.currentLocation.removeTile((int)tile.X, (int)tile.Y, "Buildings");
                    }
                    ModConstants.EnemyMapLoaded = EnemyLoadingState.LOADED;
                }
            }
            else
                ModConstants.EnemyMapLoaded = EnemyLoadingState.LOADED;
        }

        public void RenderEnemyLeftHud(object? sender, RenderedHudEventArgs e)
        {
            // Don't run in menu, don't run when a cutscene is playing
            if (!Context.IsWorldReady || Game1.eventUp)
                return;

            // Only show in some locations and if there are monsters
            string currentLocation = Game1.currentLocation.Name;
            if (!currentLocation.Contains("SmartFellas"))
                return;
            currentLocation = currentLocation.Substring(currentLocation.LastIndexOf("SmartFellas.AscensionCode.Content_") + 34);
            string[] validLocations = { "Mountain_1", "Mountain_2" };

            if (!validLocations.Contains(currentLocation) || monsterCount == 0)
                return;


            int screenHeight = Game1.graphics.GraphicsDevice.Viewport.Height;
            int screenWidth = Game1.graphics.GraphicsDevice.Viewport.Width;

            Vector2 offset = new(screenWidth / 2, screenHeight/11.0f);

            // Blank pixel for displaying the actual moving bar
            Texture2D pixelTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });


            SpriteBatch spriteBatch = e.SpriteBatch;

            // Draw the HUD holder
            spriteBatch.Draw(
                texture: skullbackgroundHUDTexture,
                position: offset - new Vector2(40, -1),
                sourceRectangle: null,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: new Vector2(1.0f, 1.0f),
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            // Draw the skull
            spriteBatch.Draw(
                texture: skullHUDTexture,
                position: offset - new Vector2(28, -10),
                sourceRectangle: null,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: new Vector2(0.5f, 0.5f),
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            // Draw enemies left
            spriteBatch.DrawString(
                    spriteFont: Game1.dialogueFont,
                    text: monsterCount.ToString(),
                    position: offset + new Vector2(4, 4),
                    color: Color.Black,
                    rotation: 0,
                    origin: Vector2.Zero,
                    scale: new Vector2(1.0f, 1.0f),
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );
        }


        // Potential functions for animating the rocks to shake before dissapearing
        private void RockAnimationStart()
        {

        }

        public void RockAnimation(object? sender, RenderedWorldEventArgs e) 
        {

        }
    }
}
