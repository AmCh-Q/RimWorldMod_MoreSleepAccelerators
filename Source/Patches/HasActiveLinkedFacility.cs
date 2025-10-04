using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace MoreSleepAccelerators.Patches;

public static class HasActiveLinkedFacility
{
	public static readonly MethodBase original
		= typeof(PreceptComp_BedThought).GetMethod("HasActiveLinkedFacility", Shared.flags);

	public static void Patch(Harmony harmony)
		=> harmony.Patch(original, transpiler: Shared.transpiler_ReplaceDefIfNewAccelerator);

	public static void Unpatch(Harmony harmony)
		=> harmony.Unpatch(original, HarmonyPatchType.Transpiler, harmony.Id);
}
