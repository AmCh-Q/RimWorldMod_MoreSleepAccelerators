using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace MoreSleepAccelerators.Patches;

public static class IsBetter
{
	public static readonly MethodBase original
		= typeof(CompAffectedByFacilities).GetMethod("IsBetter", Shared.flags);

	public static readonly HarmonyMethod prefix
		= new(typeof(IsBetter).GetMethod(nameof(PrefixMethod)));

	public static void Patch(Harmony harmony)
		=> harmony.Patch(original, prefix: prefix);

	public static void Unpatch(Harmony harmony)
		=> harmony.Unpatch(original, HarmonyPatchType.Prefix, harmony.Id);

	// When comparing two accelerators, make them of the same def so IsBetter doesn't complain
	public static void PrefixMethod(ref ThingDef facilityDef, Thing thanThisFacility)
	{
		ThingDef thanDef = thanThisFacility.def;
		if (facilityDef != thanDef &&
			facilityDef == Utility.sleepAccelerator &&
			Utility.newAccelerators.ContainsValue(thanDef))
		{
			facilityDef = thanDef;
		}
	}
}
