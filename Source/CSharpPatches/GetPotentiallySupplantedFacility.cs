using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreSleepAccelerators.Patches
{
    public static class GetPotentiallySupplantedFacility
    {
        public static void Patch()
            => PatchApplier.harmony.Patch(original, 
                prefix: prefix, 
                transpiler: Utility.transpiler_ReplaceDefIfNewAccelerator);
        public static void Unpatch()
            => PatchApplier.harmony.Unpatch(original, HarmonyPatchType.All, PatchApplier.harmony.Id);

        public static readonly MethodBase original
            = typeof(CompAffectedByFacilities).GetMethod(
                "GetPotentiallySupplantedFacility", Utility.flags);
        private delegate void ActionRef(ref ThingDef t1);
        public static readonly HarmonyMethod prefix
            = new HarmonyMethod(((ActionRef)Prefix).Method);

        // When vanilla tries to search for our facilities, replace it with vanilla accelerator
        private static void Prefix(ref ThingDef facilityDef)
            => facilityDef = Utility.ReplaceDefIfNewAccelerator(facilityDef);
    }
}
