using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Quests;

namespace SmartFellasMod
{
    internal sealed class QuestLogic
    {


        /*********
        ** Public methods
        *********/
            

        /*********
        ** Private methods
        *********/



        /// <summary>
        /// Start the first quest for the area, launches when mail is read.
        /// </summary>
        public static void StartInitialQuest()
        {

            Game1.addHUDMessage(new HUDMessage("Quest Recieved!", 2));
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