using IceCreamJam.Components;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
    class Building : Entity {

        private readonly string tileID;
        public Building(string tileID) {
            this.tileID = tileID;
            Name = tileID;
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            var texture = Scene.Content.LoadTexture(ContentPaths.Buildings + tileID + ".png");
            AddComponent(new SpriteRenderer(texture) { RenderLayer = Constants.RenderLayer_Buildings });

            this.Position += new Vector2(texture.Width / 2, -texture.Height / 2);

            var collider = AddComponent(new BoxCollider(128, 64));
            collider.PhysicsLayer = (int)Constants.PhysicsLayers.Buildings;
        }
    }
}
