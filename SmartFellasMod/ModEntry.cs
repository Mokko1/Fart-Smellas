using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Quests;

namespace SmartFellasMod
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Info);

            if (e.Button == SButton.Tab)
                StartExampleQuest();
        }

        private void StartExampleQuest()
        {
            var exampleQuest = new Quest();
            exampleQuest.id.Value = "999";
            exampleQuest.questType.Value = 3;

            exampleQuest.questTitle = "Cool Test Quest B)";
            exampleQuest.questDescription = "This is a very cool template quest";
            exampleQuest.moneyReward.Value = 500;


            Game1.player.questLog.Add(exampleQuest);
        }

    }
}