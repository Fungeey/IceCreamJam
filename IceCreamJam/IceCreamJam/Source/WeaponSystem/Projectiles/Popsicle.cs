
using IceCreamJam.Entities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace IceCreamJam.WeaponSystem.Projectiles {
    class Popsicle : HomingProjectile {

        public override void Initialize(Vector2 direction, Vector2 position) {
            base.Initialize(direction, position);

            this.speed = 120;
            this.lifetime = 5;
            this.damage = 3;
            this.targetHeading = direction;
        }

        public override void SetupTextures() {
            var animator = AddComponent(new SpriteAnimator() {
                RenderLayer = Constants.RenderLayer_Bullets
            });

            var texture = Scene.Content.LoadTexture(ContentPaths.Popsicle + "Popsicle_Fly.png");
            var sprites = Sprite.SpritesFromAtlas(texture, 20, 9, 0);

            animator.AddAnimation("Fly", sprites.ToArray()).Play("Fly");
            this.renderer = animator;
        }

        public override void OnHit(CollisionResult? result) {
            base.OnHit(result);

            var hitFX = Scene.AddEntity(new AnimatedEntity());
            var dir = CalculateVector();
            hitFX.Rotation = Mathf.Atan2(dir.Y, dir.X);
            hitFX.Position = this.Position;
            
            var texture = Scene.Content.LoadTexture(ContentPaths.Popsicle_Shatter);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 23);
            
            hitFX.animator.RenderLayer = Constants.RenderLayer_Bullets;
            hitFX.animator.LayerDepth = 0.4f;
            
            hitFX.animator.AddAnimation("hit", Constants.GlobalFPS * 4, sprites.ToArray());
            hitFX.animator.Play("hit", SpriteAnimator.LoopMode.ClampForever);
            
            hitFX.animator.OnAnimationCompletedEvent += (s) => hitFX.Destroy();

            Pool<Popsicle>.Free(this);
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            if(target != null)
			    batcher.DrawLineAngle(Transform.Position, Mathf.AngleBetweenVectors(this.Position, target.Position), 10, Color.Red);

            batcher.DrawCircle(this.Position, 100f, Color.Red);
        }
    }
}
