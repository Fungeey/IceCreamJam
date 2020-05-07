
using IceCreamJam.Entities.Enemies;
using IceCreamJam.Components;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace IceCreamJam.WeaponSystem.Projectiles {
    class BananaBig : Projectile {

        private float awayVelocity;
        private int hits;
        private Collider otherCollider;

        public override void Initialize(Vector2 direction, Vector2 position) {
            base.Initialize(direction, position);
            this.Name = "BananaBig";
            this.speed = 600;
            this.damage = 3;
            this.lifetime = 1.5f;
            this.hits = 0;
            otherCollider = null;

            if(this.renderer != null)
                (this.renderer as SpriteAnimator).Play($"Fly{hits}");

            this.awayVelocity = 1;
            this.Rotation = 0;
        }

        public override void SetupTextures() {
            var animator = AddComponent(new SpriteAnimator() {
                RenderLayer = Constants.Layer_Bullets
            });

            var texture = Scene.Content.LoadTexture(ContentPaths.Banana_Big);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32, 0);

            for(int i = 0; i < 3; i++)
                animator.AddAnimation($"Fly{i}", Constants.GlobalFPS*4, sprites.GetRange(i * 8, 8).ToArray());
            animator.Play("Fly0");

            this.renderer = animator;
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            (collider as CircleCollider).SetRadius(16);
            collider.LocalOffset = Vector2.Zero;
            collider.IsTrigger = true;
        }

        public override void Update() {
            base.Update();
            awayVelocity =  -((1 - lifeComponent.progress) * 2 - 1);

            if(collider.CollidesWithAny(out var result)) {
                if(otherCollider == result.Collider)
                    return;

                if(result.Collider.Entity is Enemy)
                    (result.Collider.Entity as Enemy).Damage(damage);

                if(hits == 2)
                    Pool<BananaBig>.Free(this);
                else
                    (this.renderer as SpriteAnimator).Play($"Fly{++hits}");
                this.otherCollider = result.Collider;
            } else {
                this.otherCollider = null;
            }
        }

        public override Vector2 CalculateVector() {
            return PlayerMovementComponent.getCurrentVelocity() * Time.DeltaTime + direction * speed * Time.DeltaTime * awayVelocity;
        }

        public override void OnHit(CollisionResult? result) {
            base.OnHit(result);

            // Death by timeout
            if(!result.HasValue)
                Pool<BananaBig>.Free(this);
        }
    }
}
