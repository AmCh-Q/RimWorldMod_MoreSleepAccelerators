using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace MoreSleepAccelerators.Patches
{
    public static class CanPotentiallyLinkTo
    {
        public static void Patch(Harmony harmony)
            => harmony.Patch(original, transpiler: transpiler);
        public static void Unpatch(Harmony harmony)
            => harmony.Unpatch(original, HarmonyPatchType.Transpiler, harmony.Id);

        public static readonly MethodBase original 
            = typeof(CompAffectedByFacilities).GetMethod(
                nameof(CompAffectedByFacilities.CanPotentiallyLinkTo), 
                BindingFlags.Instance | BindingFlags.Public);
        public static readonly HarmonyMethod transpiler
            = new HarmonyMethod((
                (Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>)
                Transpiler).Method);

        private static readonly MethodInfo
            m_IsPotentiallyValidFacilityForMe
                = typeof(CompAffectedByFacilities).GetMethod(
                    "IsPotentiallyValidFacilityForMe",
                    BindingFlags.Instance | BindingFlags.NonPublic);
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.IsNotNull(m_IsPotentiallyValidFacilityForMe);
            ReadOnlyCollection<CodeInstruction> instructionList
                = instructions.ToList().AsReadOnly();
            int state = 0;
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction codeInstruction = instructionList[i];
                // Here Vanilla has just finished checking IsPotentiallyValidFacilityForMe()
                if (state == 0 && i > 4 && // Not patched yet
                                           // if (!IsPotentiallyValidFacilityForMe() return false)
                    instructionList[i - 4].Calls(m_IsPotentiallyValidFacilityForMe) &&
                    instructionList[i - 3].opcode == OpCodes.Brtrue_S &&
                    instructionList[i - 2].LoadsConstant(0) &&
                    instructionList[i - 1].opcode == OpCodes.Ret)
                {
                    state = 1;
                    // We replace the facilityDef if it is one of our modded accelerators
                    //   the vanilla accelerator def will be used instead for rest of the function
                    // We couldn't have just used a prefix patch because the static linkable tests
                    //   (range && adjacency && medbed, etc) still needs to test using our defs
                    // facilityDef = ReplaceIfNewAccelerator(facilityDef)
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call,
                        Utility.m_ReplaceIfNewAccelerator);
                    yield return new CodeInstruction(OpCodes.Starg_S, 1);
                }
                yield return codeInstruction;
            }
            Debug.AreEqual(state, 1);
        }
    }
}
