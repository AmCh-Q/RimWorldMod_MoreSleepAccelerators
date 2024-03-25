using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MoreSleepAccelerators
{
    public static class Utility
    {
        public static bool initialized = false;
        public static ThingDef sleepAccelerator;
        public static DesignationCategoryDef ideology;
        public static Dictionary<string, ThingDef> newAccelerators;
        public static readonly FieldInfo
            f_basePowerConsumption = typeof(CompProperties_Power)
                .GetField("basePowerConsumption", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void Init()
        {
            sleepAccelerator = ThingDef.Named("SleepAccelerator");
            ideology = DefDatabase<DesignationCategoryDef>.GetNamed("Ideology");
            newAccelerators = new Dictionary<string, ThingDef>()
            {
                { "LargeSleepAccelerator", ThingDef.Named("LargeSleepAccelerator") },
                { "RangeSleepAccelerator", ThingDef.Named("RangeSleepAccelerator") },
            };
            initialized = true;
        }

        public static readonly MethodInfo
            m_ReplaceIfNewAccelerator
                = ((Func<ThingDef, ThingDef>)ReplaceIfNewAccelerator).Method,
            m_ComparedAgainstVanilla
                = ((Func<ThingDef, ThingDef, bool>)ComparedAgainstVanilla).Method;
        public static ThingDef ReplaceIfNewAccelerator(ThingDef def)
            => newAccelerators.ContainsValue(def) ? sleepAccelerator : def;
        public static bool ComparedAgainstVanilla(ThingDef compared, ThingDef vanilla)
            => vanilla == sleepAccelerator && newAccelerators.ContainsValue(compared);
    }
}
