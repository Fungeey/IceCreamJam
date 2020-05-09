using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
    class AnimatedEntity : Entity {
        public SpriteAnimator animator;
        public AnimatedEntity() {
            this.animator = AddComponent(new SpriteAnimator() {
                RenderLayer = Constants.RenderLayer_FX
            });

            Name = "AnimatedEffectEntity";
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();
        }

        public void ToggleVisible(bool visible) {
            animator.Enabled = visible;
        }
    }
}
