using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ChooseBiomeCommonality.Settings;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ChooseBiomeCommonality;

[StaticConstructorOnStartup]
public static class Main
{
    private static List<BiomeDef> allBiomes;

    public static readonly Dictionary<string, string> BiomeWorkersDictionary;

    static Main()
    {
        var harmony = new Harmony("Mlie.ChooseBiomeCommonality");
        var postfix = typeof(BiomeWorker_GetScore).GetMethod("Postfix");
        BiomeWorkersDictionary = new Dictionary<string, string>();
        foreach (var biomeDef in AllBiomes)
        {
            if (biomeDef.workerClass?.FullName == null)
            {
                LogMessage($"{biomeDef.LabelCap} have no accessable workerClass to patch");
                continue;
            }

            if (BiomeWorkersDictionary.ContainsKey(biomeDef.workerClass.FullName))
            {
                continue;
            }

            BiomeWorkersDictionary[biomeDef.workerClass.FullName] = biomeDef.defName;
            var original = biomeDef.workerClass.GetMethod("GetScore");

            LogMessage($"Patching {biomeDef.workerClass}");
            harmony.Patch(original, null, new HarmonyMethod(postfix));
        }

        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static List<BiomeDef> AllBiomes
    {
        get
        {
            if (allBiomes == null || allBiomes.Count == 0)
            {
                allBiomes = (from biome in DefDatabase<BiomeDef>.AllDefsListForReading
                    orderby biome.label
                    select biome).ToList();
            }

            return allBiomes;
        }
        set => allBiomes = value;
    }

    public static void LogMessage(string message, bool forced = false, bool warning = false)
    {
        if (warning)
        {
            Log.Warning($"[ChooseBiomeCommonality]: {message}");
            return;
        }

        if (!forced && !ChooseBiomeCommonality_Mod.instance.Settings.VerboseLogging)
        {
            return;
        }

        Log.Message($"[ChooseBiomeCommonality]: {message}");
    }
}