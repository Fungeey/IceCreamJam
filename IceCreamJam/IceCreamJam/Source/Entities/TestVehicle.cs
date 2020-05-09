using IceCreamJam.Components;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
    class TestVehicle : Vehicle {

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            Name = "Vehicle";

            var texture = Scene.Content.LoadTexture(ContentPaths.BoxSprite);
            AddComponent(new SpriteRenderer(texture) { RenderLayer = Constants.RenderLayer_Vehicles });
            AddComponent(new CircleCollider(16));
            AddComponent(new RenderSorterComponent());
        }
    }
}
