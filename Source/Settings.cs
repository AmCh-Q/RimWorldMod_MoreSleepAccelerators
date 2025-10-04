using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace MoreSleepAccelerators;

public class Settings : ModSettings
{
	public DesignationCategoryDef?
		dfltCategory_wallMountedSleepAccelerator = null,
		dfltCategory_largeSleepAccelerator = null,
		dfltCategory_rangeSleepAccelerator = null;

	public bool enabled_SleepAcceleratorsStack;
	public bool enabled_WallMountedSleepAccelerator = true;
	public bool enabled_LargeSleepAccelerator = true;
	public bool enabled_RangeSleepAccelerator = true;
	public float powerIdle_WallMountedSleepAccelerator = 50f;
	public float powerIdle_LargeSleepAccelerator = 50f;
	public float powerIdle_RangeSleepAccelerator = 80f;
	public float powerActive_WallMountedSleepAccelerator = 400f;
	public float powerActive_LargeSleepAccelerator = 400f;
	public float powerActive_RangeSleepAccelerator = 800f;
	public float range_RangeSleepAccelerator = 6f;

	public static readonly FieldInfo
		f_basePowerConsumption = typeof(CompProperties_Power).GetField("basePowerConsumption",
			BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref enabled_SleepAcceleratorsStack, "enabled_SleepAcceleratorsStack", false);
		Scribe_Values.Look(ref enabled_WallMountedSleepAccelerator, "enabled_WallMountedSleepAccelerator", true);
		Scribe_Values.Look(ref powerIdle_WallMountedSleepAccelerator, "powerIdle_WallMountedSleepAccelerator", 50f);
		Scribe_Values.Look(ref powerActive_WallMountedSleepAccelerator, "powerActive_WallMountedSleepAccelerator", 400f);
		Scribe_Values.Look(ref enabled_LargeSleepAccelerator, "enabled_LargeSleepAccelerator", true);
		Scribe_Values.Look(ref powerIdle_LargeSleepAccelerator, "powerIdle_LargeSleepAccelerator", 50f);
		Scribe_Values.Look(ref powerActive_LargeSleepAccelerator, "powerActive_LargeSleepAccelerator", 400f);
		Scribe_Values.Look(ref enabled_RangeSleepAccelerator, "enabled_RangeSleepAccelerator", true);
		Scribe_Values.Look(ref powerIdle_RangeSleepAccelerator, "powerIdle_RangeSleepAccelerator", 80f);
		Scribe_Values.Look(ref powerActive_RangeSleepAccelerator, "powerActive_RangeSleepAccelerator", 800f);
		Scribe_Values.Look(ref range_RangeSleepAccelerator, "range_RangeSleepAccelerator", 6f);
		if (Scribe.mode == LoadSaveMode.Saving)
			ApplySettings();
	}

	public void ApplySettings()
	{
		ThingDef wallMountedSleepAccelerator = Utility.newAccelerators["WallMountedSleepAccelerator"];
		dfltCategory_wallMountedSleepAccelerator ??= wallMountedSleepAccelerator.designationCategory;
		wallMountedSleepAccelerator.designationCategory = enabled_WallMountedSleepAccelerator
			? dfltCategory_wallMountedSleepAccelerator : null;
		f_basePowerConsumption.SetValue(
			wallMountedSleepAccelerator.GetCompProperties<CompProperties_Power>(),
			powerIdle_WallMountedSleepAccelerator);
		wallMountedSleepAccelerator.GetCompProperties<CompProperties_FacilityInUse>().inUsePowerConsumption
			= powerActive_WallMountedSleepAccelerator;

		ThingDef largeSleepAccelerator = Utility.newAccelerators["LargeSleepAccelerator"];
		dfltCategory_largeSleepAccelerator ??= largeSleepAccelerator.designationCategory;
		largeSleepAccelerator.designationCategory = enabled_LargeSleepAccelerator
			? dfltCategory_largeSleepAccelerator : null;
		f_basePowerConsumption.SetValue(
			largeSleepAccelerator.GetCompProperties<CompProperties_Power>(),
			powerIdle_LargeSleepAccelerator);
		largeSleepAccelerator.GetCompProperties<CompProperties_FacilityInUse>().inUsePowerConsumption
			= powerActive_LargeSleepAccelerator;

		ThingDef rangeSleepAccelerator = Utility.newAccelerators["RangeSleepAccelerator"];
		dfltCategory_rangeSleepAccelerator ??= rangeSleepAccelerator.designationCategory;
		rangeSleepAccelerator.designationCategory = enabled_RangeSleepAccelerator
			? dfltCategory_rangeSleepAccelerator : null;
		f_basePowerConsumption.SetValue(
			rangeSleepAccelerator.GetCompProperties<CompProperties_Power>(),
			powerIdle_RangeSleepAccelerator);
		rangeSleepAccelerator.GetCompProperties<CompProperties_FacilityInUse>().inUsePowerConsumption
			= powerActive_RangeSleepAccelerator;
		rangeSleepAccelerator.GetCompProperties<CompProperties_Facility>().maxDistance
			= range_RangeSleepAccelerator;

		PatchApplier.TogglePatches(!enabled_SleepAcceleratorsStack);
	}

	private static float RoundTo(float val, float multiple) => Mathf.Round(val / multiple) * multiple;

	public void DoSettingsWindowContents(Rect inRect)
	{
		Listing_Standard listingStandard = new();
		listingStandard.Begin(inRect);
		listingStandard.CheckboxLabeled(
			"MoreSleepAccelerators.enabled_SleepAcceleratorsStack".Translate(),
			ref enabled_SleepAcceleratorsStack);

		listingStandard.CheckboxLabeled(
			"MoreSleepAccelerators.enabled_WallMountedSleepAccelerator".Translate(),
			ref enabled_WallMountedSleepAccelerator);
		Slider(ref powerIdle_WallMountedSleepAccelerator, listingStandard,
			"MoreSleepAccelerators.powerIdle", 0f, 200f, 1f);
		Slider(ref powerActive_WallMountedSleepAccelerator, listingStandard,
			"MoreSleepAccelerators.powerActive", 0f, 2000f, 10f);

		listingStandard.CheckboxLabeled(
			"MoreSleepAccelerators.enabled_LargeSleepAccelerator".Translate(),
			ref enabled_LargeSleepAccelerator);
		Slider(ref powerIdle_LargeSleepAccelerator, listingStandard,
			"MoreSleepAccelerators.powerIdle", 0f, 200f, 1f);
		Slider(ref powerActive_LargeSleepAccelerator, listingStandard,
			"MoreSleepAccelerators.powerActive", 0f, 2000f, 10f);

		listingStandard.CheckboxLabeled(
			"MoreSleepAccelerators.enabled_RangeSleepAccelerator".Translate(),
			ref enabled_RangeSleepAccelerator);
		Slider(ref powerIdle_RangeSleepAccelerator, listingStandard,
			"MoreSleepAccelerators.powerIdle", 0f, 200f, 1f);
		Slider(ref powerActive_RangeSleepAccelerator, listingStandard,
			"MoreSleepAccelerators.powerActive", 0f, 2000f, 10f);
		Slider(ref range_RangeSleepAccelerator, listingStandard,
			"MoreSleepAccelerators.range", 0f, 20f, 0.1f);
		listingStandard.End();
	}

	public static void Slider(ref float value, Listing_Standard ls,
		string labelKey, float min, float max, float step)
	{
		ls.Label(labelKey.Translate(value.ToString(step < 1f ? "0.0" : "N0")));
		value = RoundTo(ls.Slider(value, min, max), step);
	}
}
