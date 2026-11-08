using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Triggers;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Quests;

namespace SmartFellasMod
{

    internal sealed class DialogueHandler
    {
        private readonly IMonitor Monitor;

        public DialogueHandler(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        /// <summary>
        /// Check to see if an NPC is currently being interacted with (with the action button)
        /// </summary>
        /// <param name="targetTile">The tile the cursor was over when performing action.</param>
        /// <param name="npcName">The name of the NPC to look for</param>
        /// <returns>The NPC object, if they were found</returns>
        public NPC? NPCChecker(Vector2 targetTile, string npcName)
        {
            Rectangle targetArea = new Rectangle((int)targetTile.X * 64, (int)targetTile.Y * 64, 64, 64);
            // A second target area for detecting if the cursor is right above the NPCs tile since you can usually interact with them like that as well.
            Rectangle targetAreaDown = new Rectangle((int)targetTile.X * 64, (int)targetTile.Y * 64 + 64, 64, 64);
            Netcode.NetCollection<NPC> characters = Game1.currentLocation.characters;
            NPC? npc = null;

            // Search every character in the current location, if the guide is found and shows up where the current tile is, continue.
            foreach (NPC character in characters)
            {
                this.Monitor.Log(character.GetBoundingBox().ToString(), LogLevel.Info);
                if (character.Name == "SmartFellas.AscensionCode.Content_GuideNPC" && (character.GetBoundingBox().Intersects(targetArea) || character.GetBoundingBox().Intersects(targetAreaDown)))
                    npc = character;
            }


            return npc;
        }


        // Check the guide's dialogue
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.Button.IsActionButton())
                return;

            // Don't interfere with existing menus or events.
            if (Game1.activeClickableMenu != null || Game1.eventUp)
                return;

            // Don't interfere with giving gifts.
            if (Game1.player.ActiveObject != null)
                return;

            // Pinpoint the currently selected tile and look for the NPC named guide, continue if he's found
            Vector2 targetTile = e.Cursor.GrabTile;
            NPC? npc = NPCChecker(e.Cursor.GrabTile, "SmartFellas.AscensionCode.Content_GuideNPC");

            if (npc == null)
                return;

            // Check if the quest is active
            string questId = "Stage1Return";
            Quest? questFound = null;
            foreach (Quest quest in Game1.player.questLog)
                if (quest.id.ToString() == questId)
                    questFound = quest;

            if (questFound == null)
                return;

            npc.CurrentDialogue.Clear();

            // All lines for the guideNPC
            string[] lines =
            {
                "Hey, @, how's the trip up been so far?",
                "While you were up there, I was able to find the blueprint I used when making my first oxygen mask!",
                "It's not much, but I figure you can probably get more use out of it than me now, so why don't you take it?"
            };

            int currentLine = 0;

            // Loop through each line as they get finished.
            void ShowNextLine()
            {
                Dialogue dialogue = new Dialogue(
                    npc,
                    null,
                    lines[currentLine]
                );
                currentLine++;

                dialogue.onFinish += () =>
                {
                    if (currentLine < lines.Length)
                        ShowNextLine();
                    else
                    {
                        // Complete the active quest and send the recipe
                        questFound.questComplete();

                        // Only give the mask if the player doesn't have it to remove playtesting errors
                        Game1.playSound("newRecipe");
                        if (!Game1.player.craftingRecipes.ContainsKey("SmartFellas.AscensionCode.Content_Basic_Oxygen_Mask"))
                            Game1.player.craftingRecipes.Add("SmartFellas.AscensionCode.Content_Basic_Oxygen_Mask", 0);

                        // Give the player a quest for crafting the mask
                        Game1.player.addQuest("OxygenMaskCraft");
                    }
                };
                npc.CurrentDialogue.Push(dialogue);
                Game1.drawDialogue(npc);
            }

            ShowNextLine();
        }
    }
}
