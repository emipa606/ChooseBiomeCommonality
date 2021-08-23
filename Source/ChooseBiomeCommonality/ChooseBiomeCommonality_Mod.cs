using System;
using System.Collections.Generic;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ChooseBiomeCommonality.Settings
{
    public class ChooseBiomeCommonality_Mod : Mod
    {
        /// <summary>
        ///     The instance of the settings to be read by the mod
        /// </summary>
        public static ChooseBiomeCommonality_Mod instance;

        private static readonly Vector2 buttonSize = new Vector2(120f, 25f);

        private static readonly int buttonSpacer = 200;

        private static Listing_Standard listing_Standard;

        private static string currentVersion;

        private static Vector2 scrollPosition;


        /// <summary>
        ///     The private settings
        /// </summary>
        private ChooseBiomeCommonality_Settings settings;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="content"></param>
        public ChooseBiomeCommonality_Mod(ModContentPack content)
            : base(content)
        {
            instance = this;

            if (instance.Settings.CustomCommonalities == null)
            {
                instance.Settings.CustomCommonalities = new Dictionary<string, float>();
            }

            currentVersion =
                VersionFromManifest.GetVersionFromModMetaData(
                    ModLister.GetActiveModWithIdentifier("Mlie.ChooseBiomeCommonality"));
        }

        /// <summary>
        ///     The instance-settings for the mod
        /// </summary>
        internal ChooseBiomeCommonality_Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GetSettings<ChooseBiomeCommonality_Settings>();
                }

                return settings;
            }

            set => settings = value;
        }

        /// <summary>
        ///     The settings-window
        /// </summary>
        /// <param name="rect"></param>
        public override void DoSettingsWindowContents(Rect rect)
        {
            base.DoSettingsWindowContents(rect);

            listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect);
            var labelPoint = listing_Standard.Label("CBC.resetall.label".Translate(), -1F,
                "CBC.resetall.tooltip".Translate());
            DrawButton(() =>
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        "CBC.resetall.confirm".Translate(),
                        delegate { instance.Settings.ResetManualValues(); }));
                }, "CBC.resetall.button".Translate(),
                new Vector2(labelPoint.position.x + buttonSpacer, labelPoint.position.y));

            listing_Standard.CheckboxLabeled("CBC.logging.label".Translate(), ref Settings.VerboseLogging,
                "CBC.logging.tooltip".Translate());
            if (currentVersion != null)
            {
                listing_Standard.Gap();
                GUI.contentColor = Color.gray;
                listing_Standard.Label("CBC.version.label".Translate(currentVersion));
                GUI.contentColor = Color.white;
            }

            listing_Standard.End();

            var scrollContainer = rect.ContractedBy(10);
            scrollContainer.height -= listing_Standard.CurHeight;
            scrollContainer.y += listing_Standard.CurHeight;
            Widgets.DrawBoxSolid(scrollContainer, Color.grey);
            var innerContainer = scrollContainer.ContractedBy(1);
            Widgets.DrawBoxSolid(innerContainer, new ColorInt(42, 43, 44).ToColor);
            var tabFrameRect = innerContainer.ContractedBy(5);
            tabFrameRect.y += 15;
            tabFrameRect.height -= 15;
            var tabContentRect = tabFrameRect;
            tabContentRect.x = 0;
            tabContentRect.y = 0;
            tabContentRect.width -= 20;
            var allBiomes = Main.AllBiomes;
            tabContentRect.height = allBiomes.Count * 50f;
            var scrollListing = new Listing_Standard();
            BeginScrollView(ref scrollListing, tabFrameRect, ref scrollPosition, ref tabContentRect);

            foreach (var biome in allBiomes)
            {
                if (!instance.Settings.CustomCommonalities.ContainsKey(biome.defName))
                {
                    instance.Settings.CustomCommonalities[biome.defName] = 1;
                }

                switch (instance.Settings.CustomCommonalities[biome.defName])
                {
                    case > 1:
                        GUI.color = Color.green;
                        break;
                    case < 1:
                        GUI.color = Color.red;
                        break;
                    default:
                        GUI.color = Color.white;
                        break;
                }

                var biomeRect = scrollListing.GetRect(50);
                instance.Settings.CustomCommonalities[biome.defName] = (float)Math.Round(Widgets.HorizontalSlider(
                    biomeRect,
                    instance.Settings.CustomCommonalities[biome.defName], 0, 5f, false,
                    "CBC.percent.label".Translate(
                        Math.Round(instance.Settings.CustomCommonalities[biome.defName] * 100)),
                    biome.LabelCap,
                    biome.modContentPack?.Name), 2);
                GUI.color = Color.white;
            }

            EndScrollView(ref scrollListing, ref tabContentRect, tabFrameRect.width, scrollListing.CurHeight);
            Settings.Write();
        }

        /// <summary>
        ///     The title for the mod-settings
        /// </summary>
        /// <returns></returns>
        public override string SettingsCategory()
        {
            return "Choose Biome Commonalities";
        }


        private static void DrawButton(Action action, string text, Vector2 pos)
        {
            var rect = new Rect(pos.x, pos.y, buttonSize.x, buttonSize.y);
            if (!Widgets.ButtonText(rect, text, true, false, Color.white))
            {
                return;
            }

            SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera();
            action();
        }

        private static void BeginScrollView(ref Listing_Standard listingStandard, Rect rect, ref Vector2 position,
            ref Rect viewRect)
        {
            Widgets.BeginScrollView(rect, ref position, viewRect);
            rect.height = 100000f;
            rect.width -= 20f;
            listingStandard.Begin(rect.AtZero());
        }

        private void EndScrollView(ref Listing_Standard listingStandard, ref Rect viewRect, float width, float height)
        {
            viewRect = new Rect(0f, 0f, width, height);
            Widgets.EndScrollView();
            listingStandard.End();
        }
    }
}