using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.UI;

namespace IceCreamJam.UI {
    class ShadedImage : Image {

        public Effect effect;

        public ShadedImage(Effect effect) : this(effect, null) { }

        public ShadedImage(Effect effect, Texture2D texture, Scaling scaling = Scaling.Stretch, int align = 1) : base(texture, scaling, align) {
            this.effect = effect;
        }

        public override void Draw(Batcher batcher, float parentAlpha) {

            //https://www.reddit.com/r/gamedev/comments/435dkp/how_to_add_pixel_shaders_to_monogame/
            //https://mysteriousspace.com/2019/01/05/pixel-shaders-in-monogame-a-tutorial-of-sorts-for-2019/

            batcher.End();
            batcher.Begin(effect);
            base.Draw(batcher, parentAlpha);
            batcher.End();
            batcher.Begin();
        }
    }
}
