using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace MoreSleepAccelerators.Patches;

public static class PotentialThingsToLinkTo
{
	public static readonly MethodBase original
		= AccessTools.EnumeratorMoveNext(typeof(CompAffectedByFacilities)
			.GetMethod(nameof(CompAffectedByFacilities.PotentialThingsToLinkTo)));

	public static void Patch(Harmony harmony)
		=> harmony.Patch(original, transpiler: Shared.transpiler_ReplaceDefIfNewAccelerator);

	public static void Unpatch(Harmony harmony)
		=> harmony.Unpatch(original, HarmonyPatchType.Transpiler, harmony.Id);
}
