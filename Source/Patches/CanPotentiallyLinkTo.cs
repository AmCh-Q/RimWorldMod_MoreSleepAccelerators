using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreSleepAccelerators.Patches;

public static class CanPotentiallyLinkTo
{
	public static readonly MethodBase original
		= typeof(CompAffectedByFacilities).GetMethod(
			nameof(CompAffectedByFacilities.CanPotentiallyLinkTo));

	public static readonly HarmonyMethod transpiler
		= new(typeof(CanPotentiallyLinkTo).GetMethod(
			nameof(TranspilerMethod)));

	public static void Patch(Harmony harmony)
		=> harmony.Patch(original, transpiler: transpiler);

	public static void Unpatch(Harmony harmony)
		=> harmony.Unpatch(original, HarmonyPatchType.Transpiler, harmony.Id);

	public static IEnumerable<CodeInstruction> TranspilerMethod(
		IEnumerable<CodeInstruction> instructions)
	{
		ReadOnlyCollection<CodeInstruction> instructionList
			= instructions.ToList().AsReadOnly();

		int state = 0;
		for (int i = 0; i < instructionList.Count; i++)
		{
			CodeInstruction codeInstruction = instructionList[i];
			yield return codeInstruction;
			// If Vanilla has just finished checking CanPotentiallyLinkTo_Static()
			if (state == 0 && codeInstruction.operand is MethodInfo method
				&& method.Name == "CanPotentiallyLinkTo_Static")
			{
				state = 1;
				// We replace the facilityDef if it is one of our modded accelerators
				//   the vanilla accelerator def will be used instead for rest of the function
				// We couldn't have just used a prefix patch because the static linkable tests
				//   (range && adjacency && medbed, etc) still needs to test using our defs
				// facilityDef = ReplaceDefIfNewAccelerator(facilityDef)
				yield return new CodeInstruction(OpCodes.Ldarg_1);
				yield return new CodeInstruction(OpCodes.Call,
					Shared.m_ReplaceDefIfNewAccelerator);
				yield return new CodeInstruction(OpCodes.Starg_S, 1);
			}
			// If we are after CanPotentiallyLinkTo_Static and Vanilla is loading a def
			else if (state > 0 && codeInstruction.LoadsField(Shared.f_ThingDef))
			{
				// If the def is one of our accerelators we replace it
				state++;
				yield return new CodeInstruction(OpCodes.Call,
					Shared.m_ReplaceDefIfNewAccelerator);
			}
		}
		Debug.IsTrue(state > 1);
	}
}
