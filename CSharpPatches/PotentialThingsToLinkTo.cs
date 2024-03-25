using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MoreSleepAccelerators.Patches
{
    public static class PotentialThingsToLinkTo
    {
        public static void Patch(Harmony harmony)
            => harmony.Patch(original, transpiler: transpiler);
        public static void Unpatch(Harmony harmony)
            => harmony.Unpatch(original, HarmonyPatchType.Transpiler, harmony.Id);

        public static readonly MethodBase original
            = AccessTools.EnumeratorMoveNext((
                (Func<ThingDef, IntVec3, Rot4, Map, IEnumerable<Thing>>)
                CompAffectedByFacilities.PotentialThingsToLinkTo).Method);
        public static readonly HarmonyMethod transpiler
            = new HarmonyMethod((
                (Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>)
                Transpiler).Method);

        private static readonly FieldInfo
            f_ThingDef = typeof(Thing).GetField(nameof(Thing.def),
                BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo
            get_Current = typeof(IEnumerator<Thing>).GetProperty(
                nameof(IEnumerator<Thing>.Current), typeof(Thing)).GetGetMethod();
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.IsNotNull(f_ThingDef);
            Debug.IsNotNull(get_Current);
            ReadOnlyCollection<CodeInstruction> instructionList
                = instructions.ToList().AsReadOnly();
            int state = 0;
            object localIndex = 7;
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction codeInstruction = instructionList[i];
                // Here Vanilla just loaded a Thing into local for the first time
                // We get its index
                if (state == 0 && i >= 1 &&
                    instructionList[i - 1].Calls(get_Current) &&
                    codeInstruction.opcode == OpCodes.Stloc_S)
                {
                    state = 1;
                    localIndex = codeInstruction.operand;
                }
                // Here Vanilla just loaded a Thing.def and is trying to count
                if (state > 0 && i >= 2 &&
                    instructionList[i - 2].opcode == OpCodes.Ldloc_S &&
                    instructionList[i - 2].operand.Equals(localIndex) && // Thing
                    instructionList[i - 1].LoadsField(f_ThingDef)) // Thing.def
                {
                    // If the def is one of our accerelators we replace it
                    state++;
                    yield return new CodeInstruction(OpCodes.Call,
                        Utility.m_ReplaceIfNewAccelerator);
                }
                yield return codeInstruction;
            }
            Debug.IsTrue(state > 1);
        }
    }
}
