using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using StardewValley.Objects;
using System.ComponentModel.Design;

namespace SmartFellasMod
{
    /// <summary>
    /// Handles checks related to oxygen and draining additional energy
    /// </summary>
    public sealed class OxygenManager
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
        // its minimum is 0 and max is 2
        private static float oxygenDeprivation = 0;
        private static float adjustedOxygenDeprivation = 0;
        private static float oxygenProtectionLevel = 0;
        private string currentHatName = "";

        private static float OxygenDeprivation
        {
            get { return oxygenDeprivation; }
            set
            {

                //ensure oxygenDeprivation is assigned correctly
                oxygenDeprivation = Math.Clamp(value, 0f, 2f);
                SetAdjustedDeprivationLevel();

            }
        }

        // used to check the oxygen protection values of oxygen masks
        // the deprivation value will be subtracted by the protection value to get actual deprivation amount
        private readonly Dictionary<string, float> OxygenMaskProtection = new()
        {
            [$"{ModConstants.ContentPackId}_Basic_Oxygen_Mask"] = 0.3f
        };

        /// <summary>
        /// This find the player's hat and assigns its oxygenProtection level according to the OxygenMaskProtection dictionary
        /// currently this is called every frame, there is likely an event or check we can do to circumvent the need for this
        /// </summary>
        private void SetOxygenProtection()
        {
            // get whatever hat the player is wearing
            Hat playerHat = Game1.player.hat.Value;

            // the player isn't wearing a hat so they can't have any oxygen protection from a mask
            if (playerHat == null)
            {
                if (oxygenProtectionLevel != 0)
                {
                    SetAdjustedDeprivationLevel();
                    currentHatName = "";
                }

                oxygenProtectionLevel = 0f;
                return;
            }
            else if (currentHatName == playerHat.Name)
            {
                // bail, this is the same hat as the last check
                return;
            }
            

            currentHatName = playerHat.Name;

            // if they are wearing something that gives protection
            if (OxygenMaskProtection.ContainsKey(currentHatName))
            {
                // return the amount of protection given
                oxygenProtectionLevel = OxygenMaskProtection[currentHatName];
                this.Monitor.Log($"Protection amount: {oxygenProtectionLevel}", LogLevel.Info);
                SetAdjustedDeprivationLevel();
            }
            else
            {
                // not wearing a mask that gives protection
                oxygenProtectionLevel = 0f;
                return;

            }

        }

        private static void SetAdjustedDeprivationLevel()
        {
            adjustedOxygenDeprivation = Math.Clamp(oxygenDeprivation - oxygenProtectionLevel, 0, 999);

        }

        public Texture2D customTexture;


        /*********
        ** Methods
        *********/

        /// <summary>
        /// Render the hud element for the oxygen mechanic using the actual value as text
        /// </summary>
        public void RenderOxygenHud(object sender, RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            SpriteBatch spriteBatch = e.SpriteBatch;
            int screenWidth = Game1.graphics.GraphicsDevice.Viewport.Width;
            int screenHeight = Game1.graphics.GraphicsDevice.Viewport.Height;

            spriteBatch.Draw(
                texture: customTexture,
                position: new Vector2(50, screenHeight * (float)(5.0/6.0)),
                sourceRectangle: null,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 2,
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            // this fixes any rounding point errors
            string displayValue = adjustedOxygenDeprivation.ToString("0.0");


            Vector2 stringSize = Game1.dialogueFont.MeasureString(displayValue.ToString());

            spriteBatch.DrawString(
                spriteFont: Game1.dialogueFont,
                text: displayValue.ToString(),
                position: new Vector2(84 - stringSize.X / 2, screenHeight * (float)(5.0/6.0) + 12),
                color: Color.White
            );
        }

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

            SetOxygenProtection();

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

                // add additional energy loss if adjustedOxygenDeprivation is above 0
                float adjustedLoss = energyLost * adjustedOxygenDeprivation;
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
        public static void SetPlayerOxygen(float value)
        {
            OxygenDeprivation = value;
        }
    }
}