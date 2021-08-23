using System.Collections.Generic;
using Verse;

namespace ChooseBiomeCommonality
{
    public class ChooseBiomeCommonality_Settings : ModSettings
    {
        public Dictionary<string, float> CustomCommonalities = new Dictionary<string, float>();
        private List<string> customCommonalitiesKeys;
        private List<float> customCommonalitiesValues;

        public bool VerboseLogging;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref VerboseLogging, "VerboseLogging");
            Scribe_Collections.Look(ref CustomCommonalities, "CustomCommonalities", LookMode.Value,
                LookMode.Value,
                ref customCommonalitiesKeys, ref customCommonalitiesValues);
        }

        public void ResetManualValues()
        {
            customCommonalitiesKeys = new List<string>();
            customCommonalitiesValues = new List<float>();
            CustomCommonalities = new Dictionary<string, float>();
        }
    }
}