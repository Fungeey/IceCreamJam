using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace IceCreamJam.UI {
    class Crosshair : Entity {

        private SpriteAnimator animator;

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            Name = "Crosshair";
            SetupTextures();
        }

        private void SetupTextures() {
            this.animator = new SpriteAnimator();

            var texture = Scene.Content.LoadTexture(ContentPaths.Crosshair_Vertical);
            var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);
            animator.AddAnimation("Vertical", 2, sprites.ToArray()).Play("Vertical");

            texture = Scene.Content.LoadTexture(ContentPaths.Crosshair_Diagonal);
            sprites = Sprite.SpritesFromAtlas(texture, 16, 16);
            animator.AddAnimation("Diagonal", 12, sprites.ToArray());

            texture = Scene.Content.LoadTexture(ContentPaths.Crosshair_Transition);
            sprites = Sprite.SpritesFromAtlas(texture, 12, 12);
            animator.AddAnimation("TransitionTo", 30, sprites[0]);
            animator.AddAnimation("TransitionBack", 30, sprites[1]);

            animator.OnAnimationCompletedEvent += FinishTransition;
            AddComponent(animator);
        }

        public override void Update() {
            base.Update();

            Position = Scene.Camera.MouseToWorldPoint();

            if(InputManager.shoot.IsPressed)
                animator.Play("TransitionTo", SpriteAnimator.LoopMode.ClampForever);
            if(InputManager.shoot.IsReleased)
                animator.Play("TransitionBack", SpriteAnimator.LoopMode.ClampForever);
        }

        private void FinishTransition(string name) {
            if(name == "TransitionTo")
                animator.Play("Diagonal");
            else if(name == "TransitionBack")
                animator.Play("Vertical");
        }
    }
}
