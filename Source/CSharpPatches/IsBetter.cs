using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace MoreSleepAccelerators.Patches
{
	public static class IsBetter
	{
		public static void Patch()
			=> PatchApplier.harmony.Patch(original,
				prefix: prefix);
		public static void Unpatch()
			=> PatchApplier.harmony.Unpatch(original,
				HarmonyPatchType.Prefix, PatchApplier.harmony.Id);

		public static readonly MethodBase original
			= typeof(CompAffectedByFacilities).GetMethod("IsBetter", Utility.flags);

		private delegate void ActionRef(ref ThingDef t1, Thing t2);

		public static readonly HarmonyMethod prefix
			= new HarmonyMethod(((ActionRef)Prefix).Method);

		// When comparing two accelerators, make them of the same def so IsBetter doesn't complain
		private static void Prefix(ref ThingDef facilityDef, Thing thanThisFacility)
		{
			ThingDef thanDef = thanThisFacility.def;
			if (facilityDef != thanDef &&
				facilityDef == Utility.sleepAccelerator &&
				Utility.newAccelerators.ContainsValue(thanDef))
				facilityDef = thanDef;
		}
	}
}
