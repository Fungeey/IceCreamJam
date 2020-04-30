using IceCreamJam.Components;
using Nez;

namespace IceCreamJam.Entities {
    class NPC : Entity {
        public override void OnAddedToScene() {
            base.OnAddedToScene();

            AddComponent(new NPCComponent());
        }
    }
}
