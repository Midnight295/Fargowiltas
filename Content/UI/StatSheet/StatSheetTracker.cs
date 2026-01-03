using Fargowiltas.Content.Items.Misc;
using Steamworks;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Fargowiltas.Content.UI.StatSheet
{
    public class StatTracker
    {
        public bool statsInitialized;

        public StatTracker()
        {
            statsInitialized = false;
            AddFargoStats();
        }

        public void FinalizeStats()
        {
            statsInitialized = true;
            StatRegistry.FinalizeRegistry();
        }

        private static string StatSheetLocal(string key, object arg) => Language.GetTextValue($"Mods.Fargowiltas.UI.StatSheet.{key}", arg);
        private static string StatSheetLocal(string key) => Language.GetTextValue($"Mods.Fargowiltas.UI.StatSheet.{key}");

        private static string IconPath(string key) => $"Fargowiltas/Assets/Textures/UI/{key}";

        private StatCategory FargoCreate(string key, Func<bool> condition = null) => StatCategory.Create(key, $"Mods.Fargowiltas.UI.StatSheet.{key}", IconPath($"{key}_Icon"), condition);
        

        internal void AddFargoStats()
        {
            if (statsInitialized)
                return;

            // Only used for ui logic (DO NOT ADD TO THIS)
            FargoCreate("PermaUpgrade").RegisterCategory();

            // Combat
            FargoCreate("Combat")
                .FargoStat("Life", new(0, 0), () => Main.LocalPlayer.statLifeMax2)
                .FargoStat("LifeRegen", new(4,0), () => Main.LocalPlayer.lifeRegen / 2)
                .FargoStat("Defense", new(1,0), () => Main.LocalPlayer.statDefense)
                .FargoStat("DamageReduction", new(5,0), () => DamageReduction())
                .FargoStat("KnockbackImmunity", new(2,0), Main.LocalPlayer.noKnockback.ToString)
                .FargoStat("Aggro", new(6, 0), () => Main.LocalPlayer.aggro)
                .FargoStat("ArmorPenetration", new(3,0), () => Main.LocalPlayer.GetArmorPenetration(DamageClass.Generic))
                .RegisterCategory();

            // Movement
            FargoCreate("Movement")
                .FargoStat("MovementSpeed", new(0,1), () => Math.Round(Main.LocalPlayer.moveSpeed * 100))
                .FargoStat("MaxSpeed", new(2,1), () => MaxSpeed())
                .FargoStat("Acceleration", new(1,1), () => Math.Round((1f + Main.LocalPlayer.runAcceleration) * 100f))
                .FargoStat("Deceleration", new(3,1), () => Math.Round((1f + Main.LocalPlayer.runSlowdown) * 100f))
                .FargoStat("WingTime", new(0, 7), WingTime, condition: HasWings)
                .FargoStat("WingMaxSpeed", new(1, 7), () => Math.Round(Main.LocalPlayer.FargoMutant().StatSheetWingSpeed * 32 / 6.25), condition: HasWings)
                .FargoStat("WingAscentModifier", new(2, 7), () => Math.Round(Main.LocalPlayer.FargoMutant().StatSheetMaxAscentMultiplier * 100), condition: HasWings)
                .RegisterCategory();

            // Utility
            FargoCreate("Utility")
                .FargoStat("FishingQuests", new(1, 2), () => Main.LocalPlayer.anglerQuestsFinished)
                .FargoStat("MiningSpeed", new(2, 2), () => Math.Round(Math.Min(170, 200 - Main.LocalPlayer.pickSpeed * 100)))
                .FargoStat("Luck", new(0, 2), () => Math.Round(Main.LocalPlayer.luck, 2))
                .FargoStat("ExtraPlacementRange", new(4, 2), () => Main.LocalPlayer.blockRange)
                .FargoStat("PlacementSpeed", new(3, 2), () => Main.LocalPlayer.tileSpeed)
                .RegisterCategory();

            // Melee
            FargoCreate("Melee")
                .FargoStat("MeleeDamage", new(3,3), () => Damage(DamageClass.Melee))
                .FargoStat("MeleeCritical", new(0, 3), () => Crit(DamageClass.Melee))
                .FargoStat("MeleeSpeed", new(1, 3), () => (int)Math.Round(Main.LocalPlayer.GetAttackSpeed(DamageClass.Melee) * 100))
                .FargoStat("MeleeSize", new(2, 3), () => Math.Round(Main.LocalPlayer.GetAdjustedItemScale(Main.LocalPlayer.HeldItem ?? ContentSamples.ItemsByType[ItemID.CopperBroadsword]) * 100f))
                .RegisterCategory();

            // Ranged
            FargoCreate("Ranged")
                .FargoStat("RangedDamage", new(2, 4), () => Damage(DamageClass.Ranged))
                .FargoStat("RangedCritical", new(0, 4), () => Crit(DamageClass.Ranged))
                .RegisterCategory();

            // Magic
            FargoCreate("Magic")
                .FargoStat("MagicDamage", new(4, 5), () => Damage(DamageClass.Magic))
                .FargoStat("MagicCritical", new(0, 5), () => Crit(DamageClass.Magic))
                .FargoStat("Mana", new(1, 5), () => Main.LocalPlayer.statManaMax2)
                .FargoStat("ManaRegen", new(3, 5), () => Main.LocalPlayer.manaRegen / 2)
                .FargoStat("ManaCostReduction", new(2, 5), () => Math.Round((1.0 - Main.LocalPlayer.manaCost) * 100))
                .RegisterCategory();

            // Summon
            FargoCreate("Summon")
                .FargoStat("SummonDamage", new(5, 6), () => Damage(DamageClass.Summon))
                .FargoStat("MaxMinions", new(3, 6), () => Main.LocalPlayer.maxMinions)
                .FargoStat("MaxSentries", new(4, 6), () => Main.LocalPlayer.maxTurrets)
                .FargoStat("WhipSpeed", new(1, 6), () => Math.Round(Main.LocalPlayer.GetAttackSpeed<SummonMeleeSpeedDamageClass>() * 100f))
                .FargoStat("WhipLength", new(2, 6), () => Math.Round(Main.LocalPlayer.whipRangeMultiplier * 100f))
                .RegisterCategory();
        }

        internal void AddSoulsStats()
        {
            var souls = Fargowiltas.ModLoaded["FargowiltasSouls"] ? ModLoader.GetMod("FargowiltasSouls") : null;
            if (souls != null)
            {
                StatRegistry.TryAddStatToCategory("Summon", "SummonCritical", new(0, 6), () => (int)souls.Call("GetSummonCrit"), () => StatSheetLocal("SummonCritical"), 1 + float.Epsilon);
                StatRegistry.TryAddStatToCategory("Combat", "AttackSpeed", new(7, 0), () => (int)Math.Round(MathF.Max((float)souls.Call("GetCachedAttackSpeed"), (float)souls.Call("GetAttackSpeed")) * 100), () => StatSheetLocal("AttackSpeed"));

            }
        }

        private static double Damage(DamageClass damageClass) => Math.Round(Main.LocalPlayer.GetTotalDamage(damageClass).Additive * Main.LocalPlayer.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
        private static int Crit(DamageClass damageClass) => (int)Main.LocalPlayer.GetTotalCritChance(damageClass);

        private static bool HasWings() => Main.LocalPlayer.wingTimeMax > 0;

        private static int MaxSpeed() => (int)((Main.LocalPlayer.accRunSpeed + Main.LocalPlayer.maxRunSpeed) / 2f * Main.LocalPlayer.moveSpeed * 3);

        private static string WingTime()
        {
            Player player = Main.LocalPlayer;
            if (player.wingTimeMax / 60 > 60 || player.empressBrooch && !Fargowiltas.ModLoaded["CalamityMod"])
                return StatSheetLocal("WingTimeMoreThan60Sec");
            return StatSheetLocal("WingTimeActual", Math.Round(player.wingTimeMax / 60.0, 2));
        }

        private static int DamageReduction()
        {
            float endurance = Main.LocalPlayer.endurance;
            if (FargoUtils.EternityMode)
            {
                float r = 0.15f;
                if (endurance >= r)
                    endurance = 1 - MathF.Pow(1 - r, endurance / r);
            }
            return (int)Math.Round(endurance * 100);
        }

        private static string BattleCryText()
        {
            FargoPlayer modPlayer = Main.LocalPlayer.FargoMutant();
            if (modPlayer.BattleCry)
                return $"[c/ff0000:{Language.GetTextValue("Mods.Fargowiltas.Items.BattleCry.Battle")}]";
            if (modPlayer.CalmingCry)
                return $"[c/00ffff:{Language.GetTextValue("Mods.Fargowiltas.Items.BattleCry.Calming")}]";
            return Language.GetTextValue("Mods.Fargowiltas.UI.BattleCryNone");
        }

        private static bool BattleCryCondition() => Main.LocalPlayer.HasItem(ModContent.ItemType<BattleCry>()) || Main.LocalPlayer.FargoMutant().BattleCry || Main.LocalPlayer.FargoMutant().CalmingCry;
    }
}
