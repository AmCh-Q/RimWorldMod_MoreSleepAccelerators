using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MoreSleepAccelerators.Patches
{
	public static class PotentialThingsToLinkTo
	{
		public static void Patch()
			=> PatchApplier.harmony.Patch(original,
				transpiler: Utility.transpiler_ReplaceDefIfNewAccelerator);
		public static void Unpatch()
			=> PatchApplier.harmony.Unpatch(original,
				HarmonyPatchType.Transpiler, PatchApplier.harmony.Id);

		public static readonly MethodBase original
			= AccessTools.EnumeratorMoveNext((
				(Func<ThingDef, IntVec3, Rot4, Map, IEnumerable<Thing>>)
				CompAffectedByFacilities.PotentialThingsToLinkTo).Method);
	}
}
