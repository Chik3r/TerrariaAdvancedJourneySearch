using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedJourneySearch; 

public class Config : ModConfig {
	public override ConfigScope Mode => ConfigScope.ClientSide;
	
	public static Config Instance => ModContent.GetInstance<Config>();
	
	[Label("Enable fuzzy search")]
	[Tooltip("Enable fuzzy search (e.g. 'w' will match 'wooden sword', 'chiar' will match 'chair')")]
	[DefaultValue(true)]
	public bool EnableFuzzySearch;
	
	[Label("Fuzzy search threshold")]
	[Tooltip("The threshold for the fuzzy search. Higher values will make the search stricter, lower values will make it more lenient.")]
	[Range(30, 100)]
	[Increment(10)]
	[DefaultValue(70)]
	[DrawTicks]
	[Slider]
	public int FuzzyThreshold;
}