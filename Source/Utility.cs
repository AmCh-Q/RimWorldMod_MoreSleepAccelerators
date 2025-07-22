using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace MoreSleepAccelerators
{
	public static class Utility
	{
		public static bool initialized;
		public static ThingDef? sleepAccelerator;
		public static DesignationCategoryDef? ideology;
		public static Dictionary<string, ThingDef>? newAccelerators;
		public const BindingFlags flags =
			BindingFlags.Instance | BindingFlags.Static |
			BindingFlags.Public | BindingFlags.NonPublic;
		public static readonly FieldInfo
			f_ThingDef = typeof(Thing).GetField(nameof(Thing.def));
		public static readonly FieldInfo
			f_basePowerConsumption = typeof(CompProperties_Power)
				.GetField("basePowerConsumption", flags);

		public static void Init()
		{
			sleepAccelerator = ThingDef.Named("SleepAccelerator");
#if v1_3
			ideology = DefDatabase<DesignationCategoryDef>.GetNamed("Misc");
#else
			ideology = DefDatabase<DesignationCategoryDef>.GetNamed("Ideology");
#endif
			newAccelerators = new Dictionary<string, ThingDef>()
			{
				{ "LargeSleepAccelerator", ThingDef.Named("LargeSleepAccelerator") },
				{ "RangeSleepAccelerator", ThingDef.Named("RangeSleepAccelerator") },
				{ "WallMountedSleepAccelerator", ThingDef.Named("WallMountedSleepAccelerator") },
			};
			initialized = true;
		}

		public static readonly MethodInfo
			m_ReplaceDefIfNewAccelerator
				= ((Func<ThingDef, ThingDef>)ReplaceDefIfNewAccelerator).Method;

		public static readonly HarmonyMethod
			transpiler_ReplaceDefIfNewAccelerator = new(
					((Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>)
					ReplaceDefIfNewAccelerator).Method
				),
			transpiler_ReplaceDefIfNewAccelerator_AfterStaticChecks = new(
					((Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>)
					ReplaceDefIfNewAccelerator_AfterStaticChecks).Method
				);

		public static ThingDef ReplaceDefIfNewAccelerator(ThingDef def)
			=> newAccelerators!.ContainsValue(def) ? sleepAccelerator! : def;

		public static IEnumerable<CodeInstruction> ReplaceDefIfNewAccelerator(
			IEnumerable<CodeInstruction> instructions)
		{
			Debug.IsNotNull(f_ThingDef);
			ReadOnlyCollection<CodeInstruction> instructionList
				= instructions.ToList().AsReadOnly();
			int state = 0;
			for (int i = 0; i < instructionList.Count; i++)
			{
				CodeInstruction codeInstruction = instructionList[i];
				if (i >= 1 &&
					instructionList[i - 1].LoadsField(Utility.f_ThingDef)) // Thing.def
				{
					// If the def is one of our accerelators we replace it
					state++;
					yield return new CodeInstruction(OpCodes.Call,
						m_ReplaceDefIfNewAccelerator);
				}
				yield return codeInstruction;
			}
			Debug.IsTrue(state > 0);
		}

		private static readonly MethodInfo
			m_IsPotentiallyValidFacilityForMe
				= typeof(CompAffectedByFacilities).GetMethod(
					"IsPotentiallyValidFacilityForMe", flags);
		public static IEnumerable<CodeInstruction> ReplaceDefIfNewAccelerator_AfterStaticChecks(
			IEnumerable<CodeInstruction> instructions)
		{
			Debug.IsNotNull(Utility.f_ThingDef);
			Debug.IsNotNull(m_IsPotentiallyValidFacilityForMe);
			ReadOnlyCollection<CodeInstruction> instructionList
				= instructions.ToList().AsReadOnly();
			int state = 0;
			for (int i = 0; i < instructionList.Count; i++)
			{
				CodeInstruction codeInstruction = instructionList[i];
				// Here in CompAffectedByFacilities.IsPotentiallyValidFacilityForMe
				// Vanilla has just finished checking IsPotentiallyValidFacilityForMe()
				if (state == 0 && i >= 2 &&
					instructionList[i - 2].LoadsConstant(0) &&
					instructionList[i - 1].opcode == OpCodes.Ret)
				{
					state = 1;
					// We replace the facilityDef if it is one of our modded accelerators
					//   the vanilla accelerator def will be used instead for rest of the function
					// We couldn't have just used a prefix patch because the static linkable tests
					//   (range && adjacency && medbed, etc) still needs to test using our defs
					// facilityDef = ReplaceDefIfNewAccelerator(facilityDef)
					yield return new CodeInstruction(OpCodes.Ldarg_1);
					yield return new CodeInstruction(OpCodes.Call,
						m_ReplaceDefIfNewAccelerator);
					yield return new CodeInstruction(OpCodes.Starg_S, 1);
				}
				if (state > 0 && i >= 1 &&
					instructionList[i - 1].LoadsField(f_ThingDef)) // Thing.def
				{
					// If the def is one of our accerelators we replace it
					state++;
					yield return new CodeInstruction(OpCodes.Call,
						m_ReplaceDefIfNewAccelerator);
				}
				yield return codeInstruction;
			}
			Debug.IsTrue(state > 1);
		}
	}
}
