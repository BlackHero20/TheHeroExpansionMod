using System;
using System.Collections.Generic;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.Extensions;
using FileAccess = Godot.FileAccess;

namespace TheHeroExpansion.TheHeroExpansionCode.UI;
public static class HeroBetaArt
{
    private const string PrefsPath = "user://theheroexpansion_beta_art.txt";

    private static readonly HashSet<string> Enabled = new();
    private static readonly Dictionary<string, bool> BetaExistsCache = new();
    private static bool _loaded;

    private static readonly Action<NCard> UpdatePortrait =
        AccessTools.MethodDelegate<Action<NCard>>(AccessTools.Method(typeof(NCard), "UpdatePortrait"));

    private static string Key(CardModel card) => card.Id.Entry;

    public static string BetaPortraitPath(CardModel card) =>
        ("beta/" + card.Id.Entry.RemovePrefix().ToLowerInvariant() + ".png").BigCardImagePath();

    public static bool HasBetaArt(CardModel card)
    {
        string path = BetaPortraitPath(card);
        if (!BetaExistsCache.TryGetValue(path, out bool exists))
        {
            exists = ResourceLoader.Exists(path);
            BetaExistsCache[path] = exists;
        }
        return exists;
    }

    public static bool IsEnabled(CardModel card)
    {
        EnsureLoaded();
        return Enabled.Contains(Key(card));
    }
    
    public static string GetPortraitPath(CardModel card, string normalPath) =>
        IsEnabled(card) && HasBetaArt(card) ? BetaPortraitPath(card) : normalPath;

    public static void SetEnabled(CardModel card, bool enabled)
    {
        EnsureLoaded();
        string key = Key(card);
        bool changed = enabled ? Enabled.Add(key) : Enabled.Remove(key);
        if (!changed)
        {
            return;
        }
        MainFile.Logger.Info($"[HeroBetaArt] {key}: beta art {(enabled ? "ON" : "OFF")} ({BetaPortraitPath(card)})");
        Save();
        RefreshCardsInScene(key);
    }

    private static void RefreshCardsInScene(string key)
    {
        if (Engine.GetMainLoop() is not SceneTree tree)
        {
            return;
        }
        int count = 0;
        Refresh(tree.Root, key, ref count);
        MainFile.Logger.Info($"[HeroBetaArt] Refreshed {count} card node(s) for {key}");
    }

    private static void Refresh(Node node, string key, ref int count)
    {
        if (node is NCard card && GodotObject.IsInstanceValid(card) && card.Model is TheHeroExpansionCard model && Key(model) == key)
        {
            UpdatePortrait(card);
            count++;
        }
        foreach (Node child in node.GetChildren())
        {
            Refresh(child, key, ref count);
        }
    }

    private static void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }
        _loaded = true;
        if (!FileAccess.FileExists(PrefsPath))
        {
            return;
        }
        try
        {
            using FileAccess file = FileAccess.Open(PrefsPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                MainFile.Logger.Error($"[HeroBetaArt] Could not open {PrefsPath}: {FileAccess.GetOpenError()}");
                return;
            }
            foreach (string line in file.GetAsText().Split('\n'))
            {
                string key = line.Trim();
                if (key.Length > 0)
                {
                    Enabled.Add(key);
                }
            }
            MainFile.Logger.Info($"[HeroBetaArt] Loaded {Enabled.Count} card(s) with beta art enabled");
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[HeroBetaArt] Load failed: {e}");
        }
    }

    private static void Save()
    {
        try
        {
            using FileAccess file = FileAccess.Open(PrefsPath, FileAccess.ModeFlags.Write);
            if (file == null)
            {
                MainFile.Logger.Error($"[HeroBetaArt] Could not write {PrefsPath}: {FileAccess.GetOpenError()}");
                return;
            }
            file.StoreString(string.Join("\n", Enabled));
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[HeroBetaArt] Save failed: {e}");
        }
    }
}
