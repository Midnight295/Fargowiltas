using Fargowiltas.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI;

namespace Fargowiltas.Content.UI.StatSheet
{
    public class StatSheetText : UIElement
    {
        public Vector2 Frame;
        public string Text;
        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;
        public StatSheetText(Vector2 frame, string text) 
        {
            Frame = frame;
            Text = text;
            
            Width.Set(18, 0);
            Height.Set(36, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(4, 0);
            position += new Vector2(0, Font.MeasureString(Text).Y * 0.175f);

            Texture2D textureAtlas = FargoMutantAssets.UI.StatSheetIcons.Value;
            Rectangle rect = new(26 * (int)Frame.X, 26 * (int)Frame.Y, 26, 26);
            spriteBatch.Draw(textureAtlas, position + new Vector2(-16, 10), new Rectangle?(rect), Color.White, 0, rect.Size() * 0.5f, 1, SpriteEffects.None, 0);
      
            Utils.DrawBorderString(spriteBatch, Text, position, Color.White);
        }
    }
}
