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

            // Pinpoint the currently selected tile
            Vector2 targetTile = e.Cursor.GrabTile;
            Rectangle targetArea = new Rectangle((int)targetTile.X * 64, (int)targetTile.Y * 64, 64, 64);

            // Search every character in the current location, if the guide is found and shows up where the current tile is, continue.
            Netcode.NetCollection<NPC> characters = Game1.currentLocation.characters;
            NPC? npc = null;
            foreach (NPC character in characters)
                if (character.Name == "SmartFellas.AscensionCode.Content_GuideNPC" && character.GetBoundingBox().Intersects(targetArea))
                        npc = character;

           if (npc == null)
                return;

            // Check if the quest is active
            string questId = "Stage1Return";
            Quest? questFound = null;
            foreach (Quest quest in Game1.player.questLog)
                if (quest.id.ToString() == questId)
                {
                    quest.questComplete();
                    questFound = quest;
                }
                
            if (questFound == null)
                return;

            npc.CurrentDialogue.Clear();

            // Dialogue the NPC should say when the quest for the oxygen mask is active
            npc.CurrentDialogue.Push(new(npc, null, "Hey, @, how's the trip up been so far?"));
            npc.CurrentDialogue.Push(new(npc, null, "While you were up there, I was able to find the blueprint I used when making my first oxygen mask!"));
            npc.CurrentDialogue.Push(new(npc, null, "It's not much, but I figure you can probably get more use out of it than me now, so why don't you take it?"));
            Game1.drawDialogue(npc);

            // Complete the active quest and send the recipe
            questFound.questComplete();

            // Only give the mask if the player doesn't have it to remove playtesting errors
                Game1.playSound("newRecipe");
            if (!Game1.player.craftingRecipes.ContainsKey("SmartFellas.AscensionCode.Content_Basic_Oxygen_Mask")) {
                Game1.player.craftingRecipes.Add("SmartFellas.AscensionCode.Content_Basic_Oxygen_Mask", 0);
            }
        }
    }
}
