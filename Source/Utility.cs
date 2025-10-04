using System.Collections.Generic;
using Verse;

namespace MoreSleepAccelerators;

[StaticConstructorOnStartup]
public static class Utility
{
	public static Settings settings = LoadedModManager.GetMod<MoreSleepAccelerators>().GetSettings<Settings>();
	public static ThingDef sleepAccelerator;
	public static readonly Dictionary<string, ThingDef> newAccelerators;

	static Utility()
	{
		sleepAccelerator = ThingDef.Named("SleepAccelerator");
		newAccelerators = new()
		{
			{ "LargeSleepAccelerator", ThingDef.Named("LargeSleepAccelerator") },
			{ "RangeSleepAccelerator", ThingDef.Named("RangeSleepAccelerator") },
			{ "WallMountedSleepAccelerator", ThingDef.Named("WallMountedSleepAccelerator") },
		};
		settings.ApplySettings();
	}
}
