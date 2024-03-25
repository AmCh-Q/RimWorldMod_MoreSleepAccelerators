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
    public static class HasActiveLinkedFacility
    {
        public static void Patch(Harmony harmony)
            => harmony.Patch(original, transpiler: transpiler);
        public static void Unpatch(Harmony harmony)
            => harmony.Unpatch(original, HarmonyPatchType.Transpiler, harmony.Id);

        public static readonly MethodBase original 
            = typeof(PreceptComp_BedThought).GetMethod(
                "HasActiveLinkedFacility", BindingFlags.Instance | BindingFlags.NonPublic);
        public static readonly HarmonyMethod transpiler 
            = new HarmonyMethod((
            (Func<IEnumerable<CodeInstruction>, ILGenerator, IEnumerable<CodeInstruction>>)
            Transpiler).Method);

        private static readonly FieldInfo
            f_ThingDef = typeof(Thing).GetField(nameof(Thing.def),
                BindingFlags.Instance | BindingFlags.Public),
            f_requireFacility = typeof(PreceptComp_BedThought).GetField(
                nameof(PreceptComp_BedThought.requireFacility),
                BindingFlags.Instance | BindingFlags.Public),
            f_facilityDef = typeof(PreceptComp_BedThought.FacilityData).GetField(
                nameof(PreceptComp_BedThought.FacilityData.def),
                BindingFlags.Instance | BindingFlags.Public);
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            Debug.IsNotNull(f_ThingDef);
            Debug.IsNotNull(f_requireFacility);
            ReadOnlyCollection<CodeInstruction> instructionList
                = instructions.ToList().AsReadOnly();
            Label jumpLabel = ilg.DefineLabel();
            int state = 0;
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction codeInstruction = instructionList[i];
                // Here Vanilla is checking item.def == requireFacility.def
                if (state == 0 && i < instructionList.Count - 6 &&
                    codeInstruction.opcode == OpCodes.Ldloc_2 && // item
                    instructionList[i + 1].LoadsField(f_ThingDef) && // item.def
                    instructionList[i + 2].IsLdarg(0) && // this
                    instructionList[i + 3].LoadsField(f_requireFacility) && // this.requireFacility
                    instructionList[i + 4].LoadsField(f_facilityDef) && // this.requireFacility.def
                    instructionList[i + 5].opcode == OpCodes.Bne_Un_S) // skip is not equal
                {
                    state = 1;
                    // We check if both item and requireFacility are sleep accelerators
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldfld, f_ThingDef);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, f_requireFacility);
                    yield return new CodeInstruction(OpCodes.Ldfld, f_ThingDef);
                    yield return new CodeInstruction(OpCodes.Call,
                        Utility.m_ComparedAgainstVanilla);
                    // If yes then skip vanilla check and pretend item.def == requireFacility.def
                    yield return new CodeInstruction(OpCodes.Brtrue_S, jumpLabel);
                    // If no then do vanilla checks normally
                    yield return codeInstruction;
                    yield return instructionList[i + 1];
                    yield return instructionList[i + 2];
                    yield return instructionList[i + 3];
                    yield return instructionList[i + 4];
                    yield return instructionList[i + 5];
                    // The destination of the yes jump, which is after the vanilla checks
                    yield return instructionList[i + 6].WithLabels(jumpLabel);
                    i += 6;
                    continue;
                }
                yield return codeInstruction;
            }
            Debug.AreEqual(state, 1);
        }
    }
}
