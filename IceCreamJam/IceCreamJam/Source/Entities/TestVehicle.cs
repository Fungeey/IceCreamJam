using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
    class TestVehicle : Vehicle {

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            var texture = Scene.Content.LoadTexture(ContentPaths.BoxSprite);
            AddComponent(new SpriteRenderer(texture));
            AddComponent(new BoxCollider(32, 32));
        }
    }
}
