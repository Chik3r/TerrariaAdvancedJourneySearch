#nullable enable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;

namespace AdvancedJourneySearch; 

public class Search {
	private static readonly Regex ModSearchRegex = new(@"@(\w)*", RegexOptions.Compiled);
	private static readonly Regex IdSearchRegex = new(@"id:(\d)*", RegexOptions.Compiled);
	private string SearchCriteria { get; }
	private string? ModName { get; }
	private int? Id { get; }

	public Search(string searchCriteria) {
		SearchCriteria = searchCriteria.Trim();

		if (searchCriteria.Contains('@')) {
			Match match = ModSearchRegex.Match(searchCriteria);
			if (match.Success)
				ModName = match.Value[1..];

			SearchCriteria = ModSearchRegex.Replace(SearchCriteria, "");
			SearchCriteria = SearchCriteria.Trim();
		}

		if (searchCriteria.Contains(":")) {
			Match match = IdSearchRegex.Match(SearchCriteria);
			if (match.Success && int.TryParse(match.Value[3..], out int id))
				Id = id;
			
			SearchCriteria = IdSearchRegex.Replace(SearchCriteria, "");
			SearchCriteria = SearchCriteria.Trim();
		}
	}

	public bool FitsFilter(Item input, IEnumerable<string?> tooltipLines) {
		if (ModName is not null) {
			string itemModName = input.ModItem?.Mod.Name ?? "terraria";
			
			if (!SimpleSearch(ModName, itemModName))
				return false;
		}

		if (Id is not null) {
			return input.type == Id;
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