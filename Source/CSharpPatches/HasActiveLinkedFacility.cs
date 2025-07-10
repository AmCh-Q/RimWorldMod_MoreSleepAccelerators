using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace MoreSleepAccelerators.Patches
{
	public static class HasActiveLinkedFacility
	{
		public static void Patch()
			=> PatchApplier.harmony.Patch(original,
				transpiler: Utility.transpiler_ReplaceDefIfNewAccelerator);
		public static void Unpatch()
			=> PatchApplier.harmony.Unpatch(original,
				HarmonyPatchType.Transpiler, PatchApplier.harmony.Id);

		public static readonly MethodBase original
			= typeof(PreceptComp_BedThought).GetMethod("HasActiveLinkedFacility", Utility.flags);
	}
}
