using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace MoreSleepAccelerators.Patches
{
	public static class CanPotentiallyLinkTo
	{
		public static void Patch()
			=> PatchApplier.harmony.Patch(original,
				transpiler: Utility.transpiler_ReplaceDefIfNewAccelerator_AfterStaticChecks);
		public static void Unpatch()
			=> PatchApplier.harmony.Unpatch(original, HarmonyPatchType.Transpiler, PatchApplier.harmony.Id);

		public static readonly MethodBase original
			= typeof(CompAffectedByFacilities).GetMethod(
				nameof(CompAffectedByFacilities.CanPotentiallyLinkTo));

		private static readonly MethodInfo
			m_IsPotentiallyValidFacilityForMe
				= typeof(CompAffectedByFacilities).GetMethod(
					"IsPotentiallyValidFacilityForMe", Utility.flags);
	}
}
