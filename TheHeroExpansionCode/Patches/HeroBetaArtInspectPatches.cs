using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens;
using TheHeroExpansion.TheHeroExpansionCode.Cards;
using TheHeroExpansion.TheHeroExpansionCode.UI;

namespace TheHeroExpansion.TheHeroExpansionCode.Patches;
[HarmonyPatch(typeof(NInspectCardScreen))]
public static class HeroBetaArtInspectPatches
{
    private sealed class State
    {
        public NTickbox? Tickbox;
        public Label? Label;
        public bool Positioned;
        
        public float AnchorX;             
        public float RowY;
        public float BoxSize;
        public Vector2 UpgradeOriginalPos;
        public float UpgradeRowWidth;      
        public Control? SeparateUpgradeLabel; 
        public Vector2 UpgradeLabelOriginalPos;
    }
    
    private const float GapFromCenter = 40f;

    private const float LabelGap = 8f;

    private static readonly ConditionalWeakTable<NInspectCardScreen, State> States = new();

    private static readonly AccessTools.FieldRef<NInspectCardScreen, NTickbox> UpgradeTickbox =
        AccessTools.FieldRefAccess<NInspectCardScreen, NTickbox>("_upgradeTickbox");

    private static readonly AccessTools.FieldRef<NInspectCardScreen, List<CardModel>?> Cards =
        AccessTools.FieldRefAccess<NInspectCardScreen, List<CardModel>?>("_cards");

    private static readonly AccessTools.FieldRef<NInspectCardScreen, int> Index =
        AccessTools.FieldRefAccess<NInspectCardScreen, int>("_index");

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NInspectCardScreen.Open))]
    private static void OpenPostfix(NInspectCardScreen __instance)
    {
        try
        {
            NInspectCardScreen screen = __instance;
            State state = States.GetValue(screen, _ => new State());
            if (state.Tickbox == null || !GodotObject.IsInstanceValid(state.Tickbox))
            {
                state.Positioned = false;
                CreateTickbox(screen, state);
            }
            if (!state.Positioned)
            {
                Callable.From(() => Callable.From(() => Position(screen, state)).CallDeferred()).CallDeferred();
            }
            Sync(screen);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[HeroBetaArt] Open patch failed: {e}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("SetCard")]
    private static void SetCardPostfix(NInspectCardScreen __instance)
    {
        try
        {
            Sync(__instance);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[HeroBetaArt] SetCard patch failed: {e}");
        }
    }

    private static CardModel? CurrentCard(NInspectCardScreen screen)
    {
        List<CardModel>? cards = Cards(screen);
        int index = Index(screen);
        return cards != null && index >= 0 && index < cards.Count ? cards[index] : null;
    }

    private static void CreateTickbox(NInspectCardScreen screen, State state)
    {
        NTickbox upgrade = UpgradeTickbox(screen);
        
        const int flags = (int)(Node.DuplicateFlags.Groups | Node.DuplicateFlags.Scripts | Node.DuplicateFlags.UseInstantiation);
        if (upgrade.Duplicate(flags) is not NTickbox tickbox)
        {
            MainFile.Logger.Error("[HeroBetaArt] Duplicating the upgrade tickbox failed");
            return;
        }
        tickbox.Name = "HeroBetaArtTickbox";
        tickbox.Visible = false;
        tickbox.FocusMode = Control.FocusModeEnum.None;
        
        AdoptDescendants(upgrade, tickbox, tickbox);
        
        if (tickbox.FindChild("TickboxVisuals", true, false) is CanvasItem visuals && visuals.Material is ShaderMaterial material)
        {
            visuals.Material = (Material)material.Duplicate();
        }

        HideLabels(tickbox);
        screen.AddChild(tickbox);
        tickbox.IsTicked = false;
        tickbox.Toggled += tb => OnToggled(screen, tb.IsTicked);

        var label = new Label
        {
            Name = "HeroBetaArtLabel",
            Text = "Beta Art",
            Visible = false,
            MouseFilter = Control.MouseFilterEnum.Stop,
        };
        if (screen.GetNodeOrNull<Control>("%ShowUpgradeLabel") is Control upgradeLabel)
        {
            if (upgradeLabel.GetThemeFont("font") is Font font)
            {
                label.AddThemeFontOverride("font", font);
            }
            int fontSize = upgradeLabel.GetThemeFontSize("font_size");
            if (fontSize > 0)
            {
                label.AddThemeFontSizeOverride("font_size", fontSize);
            }
            label.AddThemeColorOverride("font_color", upgradeLabel.GetThemeColor("font_color"));
        }
        label.GuiInput += e =>
        {
            if (e is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }
                && tickbox.MouseFilter != Control.MouseFilterEnum.Ignore)
            {
                tickbox.ForceToggleTick();
            }
        };
        screen.AddChild(label);

        state.Tickbox = tickbox;
        state.Label = label;
        MainFile.Logger.Info("[HeroBetaArt] Inspect screen tickbox created");
    }

    private static void Position(NInspectCardScreen screen, State state)
    {
        try
        {
            if (!GodotObject.IsInstanceValid(screen) || state.Tickbox == null || state.Label == null)
            {
                return;
            }
            NTickbox upgrade = UpgradeTickbox(screen);
            float left = upgrade.GlobalPosition.X;
            float right = left + upgrade.Size.X;
            if (screen.GetNodeOrNull<Control>("%ShowUpgradeLabel") is Control upgradeLabel)
            {
                right = Mathf.Max(right, upgradeLabel.GlobalPosition.X + upgradeLabel.Size.X);
                if (!upgrade.IsAncestorOf(upgradeLabel))
                {
                    state.SeparateUpgradeLabel = upgradeLabel;
                    state.UpgradeLabelOriginalPos = upgradeLabel.GlobalPosition;
                }
            }

            state.BoxSize = upgrade.Size.Y;
            state.RowY = upgrade.GlobalPosition.Y;
            state.UpgradeOriginalPos = upgrade.GlobalPosition;
            state.UpgradeRowWidth = right - left;
            state.AnchorX = (left + right) * 0.5f + 50f;
            state.Tickbox.Size = new Vector2(state.BoxSize, state.BoxSize);
            state.Positioned = true;

            MainFile.Logger.Info($"[HeroBetaArt] Captured layout: anchorX={state.AnchorX} rowY={state.RowY} upgradeRowWidth={state.UpgradeRowWidth} " +
                                 $"upgradeParent={upgrade.GetParent()?.GetType().Name} separateLabel={state.SeparateUpgradeLabel != null}");
            Sync(screen);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[HeroBetaArt] Positioning failed: {e}");
        }
    }

    private static void Sync(NInspectCardScreen screen)
    {
        if (!States.TryGetValue(screen, out State? state) || state.Tickbox == null || state.Label == null
            || !GodotObject.IsInstanceValid(state.Tickbox))
        {
            return;
        }
        CardModel? card = CurrentCard(screen);
        bool ours = card is TheHeroExpansionCard;
        bool hasBeta = ours && HeroBetaArt.HasBetaArt(card!);
        bool visible = state.Positioned && ours;

        Color modulate = hasBeta ? Colors.White : new Color(1f, 1f, 1f, 0.5f);
        Control.MouseFilterEnum filter = hasBeta ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;

        state.Tickbox.Visible = visible;
        state.Tickbox.Modulate = modulate;
        state.Tickbox.MouseFilter = filter;
        state.Label.Visible = visible;
        state.Label.Modulate = modulate;
        state.Label.MouseFilter = filter;
        
        state.Tickbox.IsTicked = hasBeta && HeroBetaArt.IsEnabled(card!);

        if (state.Positioned)
        {
            Layout(screen, state, visible);
        }
    }
    
    private static void Layout(NInspectCardScreen screen, State state, bool showBeta)
    {
        NTickbox upgrade = UpgradeTickbox(screen);
        float upgradeLeft = state.UpgradeOriginalPos.X;
        float betaLeft = 0f;

        if (showBeta)
        {
            float labelWidth = Mathf.Max(state.Label!.Size.X, state.Label.GetMinimumSize().X);
            float betaRowWidth = state.BoxSize + LabelGap + labelWidth;
            if (upgrade.Visible)
            {
                upgradeLeft = state.AnchorX - GapFromCenter - state.UpgradeRowWidth;
                betaLeft = state.AnchorX + GapFromCenter;
            }
            else
            {
                betaLeft = state.AnchorX - betaRowWidth * 0.5f;
            }
        }

        float shift = upgradeLeft - state.UpgradeOriginalPos.X;
        upgrade.GlobalPosition = state.UpgradeOriginalPos + new Vector2(shift, 0f);
        if (state.SeparateUpgradeLabel != null && GodotObject.IsInstanceValid(state.SeparateUpgradeLabel))
        {
            state.SeparateUpgradeLabel.GlobalPosition = state.UpgradeLabelOriginalPos + new Vector2(shift, 0f);
        }

        if (showBeta)
        {
            state.Tickbox!.GlobalPosition = new Vector2(betaLeft, state.RowY);
            state.Label!.GlobalPosition = new Vector2(betaLeft + state.BoxSize + LabelGap, state.RowY + (state.BoxSize - state.Label.Size.Y) * 0.5f);
        }
    }

    private static void OnToggled(NInspectCardScreen screen, bool ticked)
    {
        try
        {
            if (CurrentCard(screen) is TheHeroExpansionCard card && HeroBetaArt.HasBetaArt(card))
            {
                HeroBetaArt.SetEnabled(card, ticked);
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[HeroBetaArt] Toggle failed: {e}");
        }
    }

    private static void AdoptDescendants(Node source, Node copy, Node owner)
    {
        int count = Math.Min(source.GetChildCount(), copy.GetChildCount());
        for (int i = 0; i < count; i++)
        {
            Node sourceChild = source.GetChild(i);
            Node copyChild = copy.GetChild(i);
            if (copyChild.Owner == null)
            {
                copyChild.Owner = owner;
            }
            if (sourceChild.UniqueNameInOwner)
            {
                copyChild.UniqueNameInOwner = true;
            }
            AdoptDescendants(sourceChild, copyChild, owner);
        }
    }

    private static void HideLabels(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Label label)
            {
                label.Visible = false;
            }
            HideLabels(child);
        }
    }
}