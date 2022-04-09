using ChooseBiomeCommonality.Settings;

namespace ChooseBiomeCommonality;

public class BiomeWorker_GetScore
{
    public static void Postfix(object __instance, ref float __result)
    {
        if (__result <= 0)
        {
            return;
        }

        var fullName = __instance.GetType().FullName;
        if (fullName == null || !Main.BiomeWorkersDictionary.ContainsKey(fullName) ||
            !ChooseBiomeCommonality_Mod.instance.Settings.CustomCommonalities.ContainsKey(
                Main.BiomeWorkersDictionary[fullName]))
        {
            return;
        }

        var multiplier =
            ChooseBiomeCommonality_Mod.instance.Settings.CustomCommonalities[Main.BiomeWorkersDictionary[fullName]];
        if (multiplier == 1)
        {
            return;
        }

        __result *= multiplier;
        Main.LogMessage(
            $"Changed {Main.BiomeWorkersDictionary[fullName]} by {multiplier}, resulting in {__result}");
    }
}