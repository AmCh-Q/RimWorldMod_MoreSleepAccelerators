using HarmonyLib;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace MoreSleepAccelerators.Patches;

public static class Shared
{
	public const BindingFlags flags =
		BindingFlags.Instance | BindingFlags.Static |
		BindingFlags.Public | BindingFlags.NonPublic;

	public static readonly FieldInfo
		f_ThingDef = typeof(Thing).GetField(nameof(Thing.def));

	public static readonly MethodInfo m_ReplaceDefIfNewAccelerator
		= typeof(Shared).GetMethod(nameof(ReplaceDefIfNewAccelerator));

	public static readonly HarmonyMethod transpiler_ReplaceDefIfNewAccelerator
		= new(typeof(Shared).GetMethod(nameof(TranspilerMethod_ReplaceDefIfNewAccelerator)));

	public static ThingDef ReplaceDefIfNewAccelerator(ThingDef def)
		=> Utility.newAccelerators.ContainsValue(def) ? Utility.sleepAccelerator : def;

	public static IEnumerable<CodeInstruction> TranspilerMethod_ReplaceDefIfNewAccelerator(
		IEnumerable<CodeInstruction> instructions)
	{
		ReadOnlyCollection<CodeInstruction> instructionList
			= instructions.ToList().AsReadOnly();
		int state = 0;
		for (int i = 0; i < instructionList.Count; i++)
		{
			CodeInstruction codeInstruction = instructionList[i];
			yield return codeInstruction;
			if (codeInstruction.LoadsField(f_ThingDef)) // Thing.def
			{
				// If the def is one of our accerelators we replace it
				state++;
				yield return new CodeInstruction(OpCodes.Call,
					m_ReplaceDefIfNewAccelerator);
			}
		}
		Debug.IsTrue(state > 0);
	}
}
