using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Fargowiltas.Common.Systems
{
    public class TextIconHandler : ITagHandler
    {
        private static int _Index = 0;
        private static string ModName = "Fargowiltas";

        public class TextIconSnippet : TextSnippet
        {
            private static Texture2D IconTexture;

            public TextIconSnippet(Texture2D texture)
            {
                IconTexture = texture;
            }
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1)
            {
                if (!justCheckingString && color != Color.Black)
                {
                    spriteBatch.Draw(IconTexture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
                size = IconTexture.Size();
                return true;
            }
            
        }

        private static Dictionary<int, string> IconIndexes = new Dictionary<int, string>
        {
            { 1, "ShimmerIcon" }
        };
        public static void RegisterNewIcon(string TextureName, string modName)
        {
            ModName = modName;
            IconIndexes.Add(_Index++, TextureName);
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            if (!int.TryParse(text, out var result) || result > IconIndexes.Keys.Max())
                return new TextSnippet(text);

            IconIndexes.TryGetValue(result, out string value);
            Texture2D texture = ModContent.Request<Texture2D>("Fargowiltas"/*ModName*/ + $"/Assets/Textures/Icons/{value}").Value;

            return new TextIconSnippet(texture)
            {
                Text = "[t" + ":" + value + "]",
                DeleteWhole = true
            };
            
        }
    }
}
