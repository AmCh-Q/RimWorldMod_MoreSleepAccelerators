using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Verse;

[assembly: AssemblyVersionAttribute("1.3.0")]

namespace MoreSleepAccelerators;

public class MoreSleepAccelerators : Mod
{
	public Settings settings;

	public MoreSleepAccelerators(ModContentPack content) : base(content)
		=> settings = GetSettings<Settings>();

	public override void DoSettingsWindowContents(Rect inRect)
	{
		base.DoSettingsWindowContents(inRect);
		settings.DoSettingsWindowContents(inRect);
	}

	public override string SettingsCategory()
		=> "MoreSleepAccelerators.Name".Translate();
}

public static class PatchApplier
{
	public static readonly Harmony harmony
		= new(id: "AmCh.MoreSleepAccelerators");

	private static bool patched;

	public static void TogglePatches(bool enable)
	{
		if (patched == enable)
			return;
		// The game counts the number of linked facilities
		//   and check if it exceeds maximum allowed connections of the facility
		//   however, this mod's accelerators are considered different facilities
		//   but they should share the same maximum limit
		// When doing connect count limits
		//   these patches help counts all sleep accelerators as the vanilla version
		if (enable)
		{
			Patches.HasActiveLinkedFacility.Patch(harmony);
			Patches.CanPotentiallyLinkTo.Patch(harmony);
			Patches.PotentialThingsToLinkTo.Patch(harmony);
			Patches.GetPotentiallySupplantedFacility.Patch(harmony);
			Patches.IsBetter.Patch(harmony);
		}
		else
		{
			Patches.HasActiveLinkedFacility.Unpatch(harmony);
			Patches.CanPotentiallyLinkTo.Unpatch(harmony);
			Patches.PotentialThingsToLinkTo.Unpatch(harmony);
			Patches.GetPotentiallySupplantedFacility.Unpatch(harmony);
			Patches.IsBetter.Unpatch(harmony);
		}
		patched = enable;
	}
}
