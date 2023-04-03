using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AdvancedJourneySearch
{
	public class AdvancedJourneySearch : Mod {
		private FieldInfo _searchField;

		public override void Load() {
			_searchField = typeof(ItemFilters.BySearch).GetField("_search", BindingFlags.Instance | BindingFlags.NonPublic);

			if (_searchField is null)
				throw new Exception("Failed to find _search field in ItemFilters.BySearch");

			On_ItemFilters.BySearch.FitsFilter += BySearchOnFitsFilter;
		}

		private bool BySearchOnFitsFilter(On_ItemFilters.BySearch.orig_FitsFilter orig, ItemFilters.BySearch self, Item entry) {
			int unusedYoyoLogo = 0, unusedResearchLine = 0;
			bool[] unusedPrefixLine = new bool[30], unusedBadPrefixLine = new bool[30];
			string[] unusedTooltipNames = new string[30];

			string searchValue = (string) _searchField.GetValue(self)!;
			int numLines = 1;
			string[] tooltipLines = new string[30];

			// Get tooltip lines
			Main.MouseText_DrawItemTooltip_GetLinesInfo(entry, ref unusedYoyoLogo, ref unusedResearchLine, 
				entry.knockBack, ref numLines, tooltipLines, unusedPrefixLine,
				unusedBadPrefixLine, unusedTooltipNames);
			
			// Split searches by pipe and trim whitespace
			List<Search> searches = searchValue.Split('|').Select(x => new Search(x)).ToList();

			// Search for the search string in the tooltip lines
			foreach (Search search in searches) {
				if (search.FitsFilter(entry, tooltipLines))
					return true;
			}

			return false;
		}
	}
}