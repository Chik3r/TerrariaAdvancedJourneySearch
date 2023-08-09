using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedJourneySearch; 

public class Config : ModConfig {
	public override ConfigScope Mode => ConfigScope.ClientSide;
	
	public static Config Instance => ModContent.GetInstance<Config>();
	
	[DefaultValue(true)]
	public bool EnableFuzzySearch;
	
	[Range(30, 100)]
	[Increment(10)]
	[DefaultValue(70)]
	[DrawTicks]
	[Slider]
	public int FuzzyThreshold;
}