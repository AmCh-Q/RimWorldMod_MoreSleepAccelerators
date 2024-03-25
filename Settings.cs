using UnityEngine;
using RimWorld;
using Verse;

namespace MoreSleepAccelerators
{
    using Patches;
    public class Settings : ModSettings
    {
        public bool enabled_LargeSleepAccelerator = true;
        public float powerIdle_LargeSleepAccelerator = 50f;
        public float powerActive_LargeSleepAccelerator = 400f;
        public bool enabled_RangeSleepAccelerator = true;
        public float powerIdle_RangeSleepAccelerator = 80f;
        public float powerActive_RangeSleepAccelerator = 800f;
        public float range_RangeSleepAccelerator = 6f;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enabled_LargeSleepAccelerator, "enabled_LargeSleepAccelerator", true);
            Scribe_Values.Look(ref powerIdle_LargeSleepAccelerator, "powerIdle_LargeSleepAccelerator", 50f);
            Scribe_Values.Look(ref powerActive_LargeSleepAccelerator, "powerActive_LargeSleepAccelerator", 400f);
            Scribe_Values.Look(ref enabled_RangeSleepAccelerator, "enabled_RangeSleepAccelerator", true);
            Scribe_Values.Look(ref powerIdle_RangeSleepAccelerator, "powerIdle_RangeSleepAccelerator", 80f);
            Scribe_Values.Look(ref powerActive_RangeSleepAccelerator, "powerActive_RangeSleepAccelerator", 800f);
            Scribe_Values.Look(ref range_RangeSleepAccelerator, "range_RangeSleepAccelerator", 6f);
            ApplySettings();
        }
        public void ApplySettings()
        {
            if (!Utility.initialized)
                return;

            ThingDef largeSleepAccelerator = Utility.newAccelerators["LargeSleepAccelerator"];
            largeSleepAccelerator.designationCategory 
                = enabled_LargeSleepAccelerator ? Utility.ideology : null;
            Utility.f_basePowerConsumption.SetValue(
                largeSleepAccelerator.GetCompProperties<CompProperties_Power>(),
                powerIdle_LargeSleepAccelerator);
            largeSleepAccelerator.GetCompProperties<CompProperties_FacilityInUse>().inUsePowerConsumption
                = powerActive_LargeSleepAccelerator;

            ThingDef rangeSleepAccelerator = Utility.newAccelerators["RangeSleepAccelerator"];
            rangeSleepAccelerator.designationCategory
                = enabled_RangeSleepAccelerator ? Utility.ideology : null;
            Utility.f_basePowerConsumption.SetValue(
                rangeSleepAccelerator.GetCompProperties<CompProperties_Power>(),
                powerIdle_RangeSleepAccelerator);
            rangeSleepAccelerator.GetCompProperties<CompProperties_FacilityInUse>().inUsePowerConsumption
                = powerActive_RangeSleepAccelerator;
            rangeSleepAccelerator.GetCompProperties<CompProperties_Facility>().maxDistance
                = range_RangeSleepAccelerator;
        }
        private float RoundTo(float val, float multiple) => Mathf.Round(val / multiple) * multiple;
        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled(
                "MoreSleepAccelerators.enabled_LargeSleepAccelerator".Translate(), 
                ref enabled_LargeSleepAccelerator);
            listingStandard.Label("MoreSleepAccelerators.powerIdle"
                .Translate(powerIdle_LargeSleepAccelerator));
            powerIdle_LargeSleepAccelerator
                = RoundTo(listingStandard.Slider(powerIdle_LargeSleepAccelerator, 0f, 200f), 1f);
            listingStandard.Label("MoreSleepAccelerators.powerActive"
                .Translate(powerActive_LargeSleepAccelerator));
            powerActive_LargeSleepAccelerator
                = RoundTo(listingStandard.Slider(powerActive_LargeSleepAccelerator, 0f, 2000f), 10f);

            listingStandard.CheckboxLabeled(
                "MoreSleepAccelerators.enabled_RangeSleepAccelerator".Translate(),
                ref enabled_RangeSleepAccelerator);
            listingStandard.Label("MoreSleepAccelerators.powerIdle"
                .Translate(powerIdle_RangeSleepAccelerator));
            powerIdle_RangeSleepAccelerator
                = RoundTo(listingStandard.Slider(powerIdle_RangeSleepAccelerator, 0f, 200f), 1f);
            listingStandard.Label("MoreSleepAccelerators.powerActive"
                .Translate(powerActive_RangeSleepAccelerator));
            powerActive_RangeSleepAccelerator
                = RoundTo(listingStandard.Slider(powerActive_RangeSleepAccelerator, 0f, 2000f), 10f);
            listingStandard.Label("MoreSleepAccelerators.range"
                .Translate(range_RangeSleepAccelerator.ToString("0.0")));
            range_RangeSleepAccelerator
                = RoundTo(listingStandard.Slider(range_RangeSleepAccelerator, 0f, 20f), 0.1f);
            listingStandard.End();
        }
    }
}
