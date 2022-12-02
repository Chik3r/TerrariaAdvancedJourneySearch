using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.Creative;
using OnItemFilters = On.Terraria.GameContent.Creative.ItemFilters;
using Terraria.ModLoader;

namespace AdvancedJourneySearch
{
	public class AdvancedJourneySearch : Mod {
		private FieldInfo _searchField;
		private Regex _modSearchRegex = new(@"@(\w)+", RegexOptions.Compiled);

		public override void Load() {
			_searchField = typeof(ItemFilters.BySearch).GetField("_search", BindingFlags.Instance | BindingFlags.NonPublic);

			if (_searchField is null)
				throw new Exception("Failed to find _search field in ItemFilters.BySearch");

			OnItemFilters.BySearch.FitsFilter += BySearchOnFitsFilter;
		}

		private bool BySearchOnFitsFilter(OnItemFilters.BySearch.orig_FitsFilter orig, ItemFilters.BySearch self, Item entry) {
			int unusedYoyoLogo = 0, unusedResearchLine = 0;
			bool[] unusedPrefixLine = new bool[30], unusedBadPrefixLine = new bool[30];
			string[] unusedTooltipNames = new string[30];

			string search = (string) _searchField.GetValue(self)!;
			int numLines = 1;
			string[] tooltipLines = new string[30];

			// Get tooltip lines
			Main.MouseText_DrawItemTooltip_GetLinesInfo(entry, ref unusedYoyoLogo, ref unusedResearchLine, 
				entry.knockBack, ref numLines, tooltipLines, unusedPrefixLine,
				unusedBadPrefixLine, unusedTooltipNames);
			
			// Split searches by pipe and trim whitespace
			string[] searches = search.Split('|');
			for (int i = 0; i < searches.Length; i++)
				searches[i] = searches[i].Trim();

			// Search for the search string in the tooltip lines
			foreach (string s in searches) {
				// Check if search contains @modname
				if (s.Contains('@')) {
					MatchCollection matches = _modSearchRegex.Matches(s);
					foreach (Match match in matches) {
						if (match.Value[1..] == entry.ModItem.Mod.Name)
							return orig(self, entry);
					}
				}
				
				for (int i = 0; i < numLines; i++) {
					if (SimpleSearch(s, tooltipLines[i]))
						return true;
				}
			}

			return false;
		}

		private bool SimpleSearch(string search, string text) {
			return text.ToLower().IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}