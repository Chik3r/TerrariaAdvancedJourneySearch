#nullable enable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;

namespace AdvancedJourneySearch; 

public class Search {
	private static Regex? _modSearchRegex = null;
	private string SearchCriteria { get; }
	private string? ModName { get; }

	public Search(string searchCriteria) {
		_modSearchRegex ??= new Regex(@"@(\w)*", RegexOptions.Compiled);
		
		SearchCriteria = searchCriteria.Trim();

		if (!searchCriteria.Contains('@'))
			return;
		
		Match match = _modSearchRegex.Match(searchCriteria);
		if (match.Success)
			ModName = match.Value[1..];

		SearchCriteria = _modSearchRegex.Replace(SearchCriteria, "");
		SearchCriteria = SearchCriteria.Trim();
	}

	public bool FitsFilter(Item input, IEnumerable<string?> tooltipLines) {
		if (ModName is not null) {
			string itemModName = input.ModItem?.Mod.Name ?? "terraria";
			
			if (!SimpleSearch(ModName, itemModName))
				return false;
		}

		foreach (string? line in tooltipLines) {
			if (line is null)
				continue;

			if (SimpleSearch(SearchCriteria, line))
				return true;
		}
		
		return false;
	}

	private static bool SimpleSearch(string search, string text) {
		return text.ToLower().IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1;
	}
}