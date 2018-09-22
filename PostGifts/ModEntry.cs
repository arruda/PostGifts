using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using xTile.Dimensions;

using ranis.SendItems.Menus;

namespace PostGifts
{
    public class ModEntry : Mod
    {
		public int currentPage = 0;
        public override void Entry(IModHelper helper)
        {
            ControlEvents.MouseChanged += this.MouseChanged;
        }

        private void MouseChanged(object sender, EventArgsMouseStateChanged e)
        {

            if (!Context.IsWorldReady)
                return;

            if (e.NewState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                // Check if the click is on the letterbox tile or the one above it
                Location tileLocation = new Location((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);

                if (tileLocation.X == 68 && (tileLocation.Y >= 15 && tileLocation.Y <= 16))
                {
                    if (CanUsePostbox())
                    {
						this.DisplayFriendSelector(this.currentPage);
                    }
                }
            }
        }

        private bool CanUsePostbox()
        {
            return Game1.mailbox != null && (Game1.mailbox.Count == 0);
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

				var friendship = Game1.player.friendshipData[npc.getName()];
				if (friendship.GiftsToday >= 1){
					continue;
				}
				if (friendship.GiftsThisWeek >= 2)
                {
                    continue;
                }
				friendsIdNames.Add(npc.getName(), npc.getName());

            }
			return friendsIdNames;
		}

		private int getTotalFriendsPages(List<Response> allResponses, int maxPerPage){
			var numberOfPages = allResponses.Count / (maxPerPage + 1);
			return numberOfPages;
		}

		private List<Response> getFriendsListPage(List<Response> allResponses, int maxPerPage, int page){

			List<Response> currentPageList = new List<Response>();
			var numberOfPages = this.getTotalFriendsPages(allResponses, maxPerPage);
            
            if (page > numberOfPages)
			{
                currentPageList.Add(new Response($"Page 0", $"Page 0"));
				return currentPageList;
            }
			int initialIndex = maxPerPage * page;
            
			for (int i = initialIndex; i < initialIndex + maxPerPage; i++)
			{   
				if (i < allResponses.Count){
					currentPageList.Add(allResponses[i]);	
				}
				else{
					break;
				}

			}
            if (page < numberOfPages)
            {
				currentPageList.Add(new Response($"Page {page + 1}", $"Page {page + 2}"));
			}
			else if(page != 0){
                currentPageList.Add(new Response($"Page 0", $"Page 0"));
			}
			return currentPageList;
		}

        private void DisplayFriendSelector(int page)
        {

            if (Game1.activeClickableMenu != null) return;
			List<Response> allResponses = new List<Response>();
			var maxPerPage = 6;
			var friendsDict = this.getFriendsDict();
			foreach (KeyValuePair<string, string> keyValues in friendsDict)
			{
				allResponses.Add(new Response(keyValues.Key, keyValues.Value));
            }
            
			if (allResponses.Count == 0)
            {
                return;
            }
            
			List<Response> currentPageList = this.getFriendsListPage(allResponses, maxPerPage, page);

			currentPageList.Add(new Response("leave", "leave"));

            Game1.currentLocation.createQuestionDialogue(
                "Select Friend:", currentPageList.ToArray(), FriendSelectorAnswered, (NPC)null
            );
            Game1.player.Halt();
		}

        private bool HighlightOnlyGiftableItems(Item i)
        {
            return i.canBeGivenAsGift();
        }

		private void FriendSelectorAnswered(StardewValley.Farmer farmer, string answer)
        {
            if (answer.Equals("leave")) return;
			if(answer.Contains("Page")){
				int page = System.Convert.ToInt32(answer.Split(' ')[1]);
				this.currentPage = page;                
				return;
			}
			else{
                var items = new List<Item> { null };
                Game1.activeClickableMenu = new ComposeLetter(this.Monitor, answer, items, 1, 1, null, HighlightOnlyGiftableItems);

			}
        }
    }
}