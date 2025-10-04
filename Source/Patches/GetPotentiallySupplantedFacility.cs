using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace MoreSleepAccelerators.Patches;

public static class GetPotentiallySupplantedFacility
{
	public static readonly MethodBase original
		= typeof(CompAffectedByFacilities).GetMethod(
			"GetPotentiallySupplantedFacility", Shared.flags);

	public static readonly HarmonyMethod prefix
		= new(typeof(GetPotentiallySupplantedFacility).GetMethod(nameof(PrefixMethod)));

	public static void Patch(Harmony harmony)
		=> harmony.Patch(original,
			prefix: prefix,
			transpiler: Shared.transpiler_ReplaceDefIfNewAccelerator);

	public static void Unpatch(Harmony harmony)
		=> harmony.Unpatch(original, HarmonyPatchType.All, harmony.Id);

	// When vanilla tries to search for our facilities, replace it with vanilla accelerator
	public static void PrefixMethod(ref ThingDef facilityDef)
		=> facilityDef = Shared.ReplaceDefIfNewAccelerator(facilityDef);
}
