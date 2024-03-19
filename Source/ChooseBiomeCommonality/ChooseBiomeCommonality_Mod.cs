using System;
using System.Collections.Generic;
using System.Linq;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ChooseBiomeCommonality.Settings;

public class ChooseBiomeCommonality_Mod : Mod
{
    /// <summary>
    ///     The instance of the settings to be read by the mod
    /// </summary>
    public static ChooseBiomeCommonality_Mod instance;

    private static readonly Vector2 buttonSize = new Vector2(120f, 25f);

    private static readonly Vector2 searchSize = new Vector2(200f, 25f);

    private static readonly int buttonSpacer = 200;

    private static Listing_Standard listing_Standard;

    private static string currentVersion;

    private static Vector2 scrollPosition;

    private static string searchText = "";

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="content"></param>
    public ChooseBiomeCommonality_Mod(ModContentPack content)
        : base(content)
    {
        instance = this;
        Settings = GetSettings<ChooseBiomeCommonality_Settings>();
        if (Settings.CustomCommonalities == null)
        {
            Settings.CustomCommonalities = new Dictionary<string, float>();
        }

        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    /// <summary>
    ///     The instance-settings for the mod
    /// </summary>
    internal ChooseBiomeCommonality_Settings Settings { get; }

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
        Rect lastLabel;
        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            lastLabel = listing_Standard.Label("CBC.version.label".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }
        else
        {
            listing_Standard.Gap();
            lastLabel = listing_Standard.Label(string.Empty);
        }

        searchText =
            Widgets.TextField(
                new Rect(
                    lastLabel.position +
                    new Vector2((rect.width / 2) - (searchSize.x / 2) - (buttonSize.x / 2), 0),
                    searchSize),
                searchText);
        TooltipHandler.TipRegion(new Rect(
            lastLabel.position + new Vector2((rect.width / 2) - (searchSize.x / 2), 0),
            searchSize), "CBC.search".Translate());

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
        if (!string.IsNullOrEmpty(searchText))
        {
            allBiomes = Main.AllBiomes.Where(def =>
                    def.label.ToLower().Contains(searchText.ToLower()) || def.modContentPack?.Name.ToLower()
                        .Contains(searchText.ToLower()) == true)
                .ToList();
        }

        tabContentRect.height = allBiomes.Count * 52f;
        var scrollListing = new Listing_Standard();
        Widgets.BeginScrollView(tabFrameRect, ref scrollPosition, tabContentRect);
        scrollListing.Begin(tabContentRect);
        foreach (var biome in allBiomes)
        {
            instance.Settings.CustomCommonalities.TryAdd(biome.defName, 1);
            var biomeRect = scrollListing.GetRect(50);

            instance.Settings.CustomCommonalities[biome.defName] = (float)Math.Round(Widgets.HorizontalSlider(
                biomeRect,
                instance.Settings.CustomCommonalities[biome.defName], 0, 5f, false,
                "CBC.percent.label".Translate(
                    Math.Round(instance.Settings.CustomCommonalities[biome.defName] * 100)),
                biome.LabelCap,
                biome.modContentPack?.Name), 2);
        }

        scrollListing.End();

        Widgets.EndScrollView();
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
}