using Fargowiltas.Items.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Fargowiltas.Buffs
{
    public class Omnistation : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omnistation");
            Description.SetDefault("Effects of all vanilla stations");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                //sunflower
                Main.SceneMetrics.HasSunflower = false;
                player.buffImmune[BuffID.Sunflower] = true;
                player.moveSpeed += 0.1f;
                player.moveSpeed *= 1.1f;
                player.sunflower = true;

                //campfire
                Main.SceneMetrics.HasCampfire = true;
                player.buffImmune[BuffID.Campfire] = true;

                //heart lantern
                Main.SceneMetrics.HasHeartLantern = false;
                player.buffImmune[BuffID.HeartLamp] = true;
                player.lifeRegen += 2;

                //star bottle
                Main.SceneMetrics.HasStarInBottle = false;
                player.buffImmune[BuffID.StarInBottle] = true;
                player.manaRegenBonus += 2;

                Main.SceneMetrics.HasGardenGnome = true;

                //bast
                Main.SceneMetrics.HasCatBast = false;
                player.buffImmune[BuffID.CatBast] = true;
                player.statDefense += 5;
            }

            int type = Framing.GetTileSafely(player.Center).type;
            if (type == ModContent.TileType<OmnistationSheet>() || type == ModContent.TileType<OmnistationSheet2>())
            {
                player.AddBuff(BuffID.Honey, 30 * 60 - 1);
                player.AddBuff(BuffID.SugarRush, 120 * 60 - 1);
            }
        }
    }
}