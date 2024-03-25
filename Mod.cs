using HarmonyLib;
using UnityEngine;
using Verse;

namespace MoreSleepAccelerators
{
    public class MoreSleepAccelerators : Mod
    {
        public Settings settings;
        public MoreSleepAccelerators(ModContentPack content) : base(content)
        {
            settings = GetSettings<Settings>();
            LongEventHandler.QueueLongEvent(delegate
            {
                Utility.Init();
                PatchApplier.ApplyPatches();
                settings.ApplySettings();
            }, "MoreSleepAccelerators.Mod.ctor", false, null);
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory() 
            => "MoreSleepAccelerators.Name".Translate();
    }
    public static class PatchApplier
    {
        public static readonly Harmony harmony 
            = new Harmony(id: "AmCh.MoreSleepAccelerators");
        public static void ApplyPatches()
        {
            // The game counts the number of linked facilities
            //   and check if it exceeds maximum allowed connections of the facility
            //   however, this mod's accelerators are considered different facilities
            //   but they should share the same maximum limit
            // When doing connect count limits
            //   these patches help counts all sleep accelerators as the vanilla version
            Patches.HasActiveLinkedFacility.Patch(harmony);
            Patches.CanPotentiallyLinkTo.Patch(harmony);
            Patches.PotentialThingsToLinkTo.Patch(harmony);
            Patches.GetPotentiallySupplantedFacility.Patch(harmony);
            Patches.IsBetter.Patch(harmony);
        }
    }
}
