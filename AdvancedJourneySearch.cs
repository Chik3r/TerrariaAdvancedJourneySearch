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
		private Regex _modSearchRegex;

		public override void Load() {
			_searchField = typeof(ItemFilters.BySearch).GetField("_search", BindingFlags.Instance | BindingFlags.NonPublic);
			_modSearchRegex = new Regex(@"@(\w)*", RegexOptions.Compiled);

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

			// Search for the search string in the tooltip lines
			foreach (string s in searches) {
				string modifiedSearch = s;
				
				// Check if search contains @modname
				if (s.Contains('@')) {
					Match match = _modSearchRegex.Match(modifiedSearch);

					string modName = entry.ModItem?.Mod.Name ?? "terraria";
					if (match.Success) {
						if (!SimpleSearch(match.Value[1..], modName))
							return false;
					}
					modifiedSearch = _modSearchRegex.Replace(modifiedSearch, "");
				}

				modifiedSearch = modifiedSearch.Trim();
				for (int i = 0; i < numLines; i++) {
					if (SimpleSearch(modifiedSearch, tooltipLines[i]))
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