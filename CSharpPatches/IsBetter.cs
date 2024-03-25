using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreSleepAccelerators.Patches
{
    public static class IsBetter
    {
        public static void Patch(Harmony harmony)
            => harmony.Patch(original, prefix: prefix);
        public static void Unpatch(Harmony harmony)
            => harmony.Unpatch(original, HarmonyPatchType.Prefix, harmony.Id);

        public static readonly MethodBase original
            = typeof(CompAffectedByFacilities).GetMethod(
                "IsBetter", BindingFlags.Instance | BindingFlags.NonPublic);
        private delegate void FuncRef(Thing t1, ref ThingDef t2, ref bool t3);
        public static readonly HarmonyMethod prefix
            = new HarmonyMethod(((FuncRef)Prefix).Method);

        private static void Prefix(Thing thanThisFacility, ref ThingDef facilityDef, ref bool __result)
        {
            ThingDef thanThisFacilityDef = thanThisFacility.def;
            // If vanilla tries to compare different accelerators defs, we fix it
            if ((facilityDef != thanThisFacilityDef) &&
                (facilityDef == Utility.sleepAccelerator || 
                Utility.newAccelerators.ContainsValue(facilityDef)) && 
                (thanThisFacilityDef == Utility.sleepAccelerator || 
                Utility.newAccelerators.ContainsValue(thanThisFacilityDef)))
                facilityDef = thanThisFacilityDef;
            return;
        }
    }
}
