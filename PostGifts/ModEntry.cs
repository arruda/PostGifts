using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewModdingAPI.Utilities;
using StardewValley;
using xTile.Dimensions;

namespace PostGifts
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            ControlEvents.MouseChanged += this.MouseChanged;
        }
        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (!e.Button.ToString().Equals("O"))
            {
                return;
            }
            this.Monitor.Log($"{Game1.player.Name} pressed valid {e.Button}.");

            var farmers = Game1.getAllFarmers();
            var npcs = Utility.getAllCharacters();

            foreach (NPC npc in npcs)
            {
                if (!npc.isVillager())
                    continue;

                this.Monitor.Log($"Character name: {npc.getName()}.");

                if (!npc.getName().Equals("Linus"))
                {
                    continue;
                }
                StardewValley.Object fav_object = npc.getFavoriteItem();
                this.Monitor.Log($"instantiated obj.");
                if (fav_object is null)
                {
                    this.Monitor.Log($"No Favorite Item!.");

                }
                else
                {
                    this.Monitor.Log($"Favorite Item: {fav_object.name}.");
                    npc.receiveGift(fav_object, Game1.player, true, 1, false);
                    this.Monitor.Log($"Gift '{fav_object.name}' sent from '{Game1.player.Name}' to '{npc.getName()}'.");
                    break;
                }

            }
        }

        private void MouseChanged(object sender, EventArgsMouseStateChanged e)
        {

            if (!Context.IsWorldReady)
                return;

            if (e.NewState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.Monitor.Log($"Pressed right mouse button.");

                // Check if the click is on the letterbox tile or the one above it
                Location tileLocation = new Location((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);

                if (tileLocation.X == 68 && (tileLocation.Y >= 15 && tileLocation.Y <= 16))
                {
                    this.Monitor.Log($"clicked mail box.");

                    if (CanUsePostbox())
                    {
                        this.Monitor.Log($"can use mail box.");

						this.DisplayFriendSelector();
                    }
                }
            }
        }

        private bool CanUsePostbox()
        {
            return Game1.mailbox != null && (Game1.mailbox.Count == 0);
            //Any();
        }

		private Dictionary<string, string> getFriendsDict(){

            Dictionary<string, string> friendsIdNames =
                    new Dictionary<string, string>();
			
			var npcs = Utility.getAllCharacters();

            foreach (NPC npc in npcs)
            {
                if (!npc.isVillager())
                    continue;

				if (!Game1.player.friendshipData.ContainsKey(npc.getName())){
					continue;
				}
                    
				friendsIdNames.Add(npc.getName(), npc.getName());

            }
			return friendsIdNames;
		}

        private void DisplayFriendSelector()
        {
            if (Game1.activeClickableMenu != null) return;
			List<Response> responseList = new List<Response>();
			var friendsDict = this.getFriendsDict();
			foreach (KeyValuePair<string, string> keyValues in friendsDict)
			{
				responseList.Add(new Response(keyValues.Key, keyValues.Value));
            }
            
			if (responseList.Count == 0)
            {
                return;
            }

            responseList.Add(new Response("leave", "leave"));

            Game1.currentLocation.createQuestionDialogue(
				"Select Friend:", responseList.ToArray(), FriendSelectorAnswered, (NPC)null
			);
            Game1.player.Halt();
        }

		private void FriendSelectorAnswered(StardewValley.Farmer farmer, string answer)
        {
            if (answer.Equals("leave")) return;

			this.Monitor.Log($"Selected friend: {answer}.");
			this.sendGift(answer);


        }

        private void sendGift(String npcName)
        {
            var npcs = Utility.getAllCharacters();

            foreach (NPC npc in npcs)
            {
				if (npc.getName().Equals(npcName)){
                    this.Monitor.Log($"Character name: {npc.getName()}.");

                    StardewValley.Object fav_object = npc.getFavoriteItem();
                    this.Monitor.Log($"instantiated obj.");
                    if (fav_object is null)
                    {
                        this.Monitor.Log($"No Favorite Item!.");
                    }
                    else
                    {
                        this.Monitor.Log($"Favorite Item: {fav_object.name}.");
                        npc.receiveGift(fav_object, Game1.player, true, 1, false);
                        this.Monitor.Log($"Gift '{fav_object.name}' sent from '{Game1.player.Name}' to '{npc.getName()}'.");
                        break;
                    }

				}
                                         
            }
        }
    }
}