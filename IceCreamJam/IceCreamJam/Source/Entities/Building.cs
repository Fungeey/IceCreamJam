using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
    class Building : Entity {
        public override void OnAddedToScene() {
            base.OnAddedToScene();


            var texture = Scene.Content.LoadTexture(ContentPaths.BoxSprite);
            AddComponent(new SpriteRenderer(texture));
            var collider = AddComponent(new BoxCollider());

            collider.PhysicsLayer = (int)Constants.PhysicsLayers.Buildings;
            
        }
    }
}
