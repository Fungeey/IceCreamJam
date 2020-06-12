using IceCreamJam.Components;
using IceCreamJam.Source.Components;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace IceCreamJam.Entities.Civilians {
    class Civilian : NPC {
        private SpriteAnimator animator;
        private Mover mover;
        private CivilianStateMachine state;
        public Civilian() {
            Name = "Civilian";
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            var animator = new SpriteAnimator() { RenderLayer = Constants.RenderLayer_Civilian };
            this.animator = AddComponent(animator);

            AddComponent(new CivilianComponent());
            state = AddComponent(new CivilianStateMachine());
            this.mover = AddComponent(new Mover());
            AddComponent(new CircleCollider(10) { PhysicsLayer = (int)Constants.PhysicsLayers.NPC });
        }

        public void Move(Vector2 vector) {
            mover.Move(vector, out var result);

            Flip(vector.X < 0);
        }

        public void Flip(bool flip) {
            animator.FlipX = flip;
        }

		public override void Update() {
            base.Update();

            state.stateMachine.Update(Time.DeltaTime);
		}
	}
}
