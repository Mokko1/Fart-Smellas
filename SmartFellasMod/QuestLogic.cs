using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Quests;

namespace SmartFellasMod
{
    internal sealed class QuestLogic : Mod
    {


        /*********
        ** Public methods
        *********/
        /// <summary>
        /// Needed for the file, not used here
        /// </summary>
        public override void Entry(IModHelper helper) {}
            

        /*********
        ** Private methods
        *********/


        /// <summary>
        /// Start 
        /// </summary>
        private void StartInitialQuest()
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