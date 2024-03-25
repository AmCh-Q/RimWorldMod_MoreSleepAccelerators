using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreSleepAccelerators.Patches
{
    public static class GetPotentiallySupplantedFacility
    {
        public static void Patch(Harmony harmony)
            => harmony.Patch(original, prefix: prefix, transpiler: transpiler);
        public static void Unpatch(Harmony harmony)
            => harmony.Unpatch(original, HarmonyPatchType.All, harmony.Id);

        public static readonly MethodBase original
            = typeof(CompAffectedByFacilities).GetMethod(
                "GetPotentiallySupplantedFacility",
                BindingFlags.Instance | BindingFlags.NonPublic);
        private delegate void ActionRef(ref ThingDef t1);
        public static readonly HarmonyMethod prefix
            = new HarmonyMethod(((ActionRef)Prefix).Method);
        public static readonly HarmonyMethod transpiler
            = new HarmonyMethod((
                (Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>)
                Transpiler).Method);

        // When vanilla tries to search for our facilities, replace it with vanilla accelerator
        private static void Prefix(ref ThingDef facilityDef)
            => facilityDef = Utility.ReplaceIfNewAccelerator(facilityDef);

        private static readonly FieldInfo
            f_ThingDef = typeof(Thing).GetField(nameof(Thing.def),
                BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo
            get_Item = AccessTools.IndexerGetter(typeof(List<Thing>), new[] { typeof(int) });
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.IsNotNull(f_ThingDef);
            Debug.IsNotNull(get_Item);
            ReadOnlyCollection<CodeInstruction> instructionList
                = instructions.ToList().AsReadOnly();
            int state = 0;
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction codeInstruction = instructionList[i];
                // Here Vanilla just got the def of a Thing
                if (i >= 2 &&
                    instructionList[i - 2].Calls(get_Item) &&
                    instructionList[i - 1].LoadsField(f_ThingDef))
                {
                    state++;
                    // If it is one of our accelerator replace it with the vanilla one
                    // To help with comparisons
                    yield return new CodeInstruction(OpCodes.Call, 
                        Utility.m_ReplaceIfNewAccelerator);
                }
                yield return codeInstruction;
            }
            Debug.AreEqual(state, 2);
        }
    }
}
