using ChooseBiomeCommonality.Settings;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ChooseBiomeCommonality;

[HarmonyPatch(typeof(Page), "DoBottomButtons")]
public static class DoBottomButtons_Patch
{
    public static void Postfix(Page __instance, Rect rect)
    {
        if (__instance is not Page_CreateWorldParams)
        {
            return;
        }

        var buttonRect = new Rect(rect.x, rect.y + 80f, 150f, 38f);
        if (Widgets.ButtonText(buttonRect, "CBC.configbiomes.text".Translate()))
        {
            Find.WindowStack.Add(new Dialog_SubModWindow(ChooseBiomeCommonality_Mod.instance));
        }

        TooltipHandler.TipRegion(buttonRect, "CBC.configbiomes.tooltip".Translate());
    }

    public class Dialog_SubModWindow : Window
    {
        private readonly Mod selMod;

        public Dialog_SubModWindow(Mod mod)
        {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            selMod = mod;
        }

        public override Vector2 InitialSize => new Vector2(864f, 584f);

        public override void PreClose()
        {
            base.PreClose();
            selMod?.WriteSettings();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 35f), selMod.Content.Name);
            Text.Font = GameFont.Small;
            var inRect2 = new Rect(0f, 40f, inRect.width, inRect.height - 40f - CloseButSize.y);
            selMod.DoSettingsWindowContents(inRect2);
        }
    }
}