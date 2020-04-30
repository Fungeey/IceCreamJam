using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
    class SpriteEntity : Entity {
        public SpriteRenderer renderer;
        public bool defaultVisible = true;

        public SpriteEntity(Texture2D texture) : this(texture, 0, 0) { }

        public SpriteEntity(Texture2D texture, int renderLayer, float layerDepth) {
            this.renderer = AddComponent(new SpriteRenderer(texture) {
                RenderLayer = renderLayer,
                LayerDepth = layerDepth
            });
            ToggleVisible(defaultVisible);
        }

        public void ToggleVisible(bool visible) {
            renderer.Enabled = visible;
        }
    }
}
