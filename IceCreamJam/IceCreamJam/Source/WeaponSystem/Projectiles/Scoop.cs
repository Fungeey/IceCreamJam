
using IceCreamJam.Entities;
using IceCreamJam.Components;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace IceCreamJam.WeaponSystem.Projectiles {
    class Scoop : Projectile {

        private ScoopType type;
        private Vector2 initV = new Vector2(0,0);
        public Vector2 truckVelocity;

        public void Initialize(Vector2 direction, Vector2 position, ScoopType type) {
            base.Initialize(direction, position);

            initV = PlayerMovementComponent.getCurrentVelocity() * 0.7f;

            this.cost = 1;
            this.damage = 1;
            this.speed = 300;
            this.lifetime = 1;

            /*
            if (!PlayerMovementComponent.turning) {
                initV = PlayerMovementComponent.getCurrentVelocity();
            }
            */
            // If this scoop has been reused, set it to use the correct texture.
            if(!IsNewProjectile && this.type != type) {
                this.type = type;
                SetScoopTextures();
            }

            this.type = type;
        }

        public override void SetupTextures() {
            var animator = AddComponent(new SpriteAnimator() {
                RenderLayer = Constants.Layer_Bullets
            });

            this.renderer = animator;
            SetScoopTextures();
        }

        private void SetScoopTextures() {
            var texture = Scene.Content.LoadTexture(ContentPaths.Scoop + $"Scoop_{(char)type}.png");
            var sprites = Sprite.SpritesFromAtlas(texture, 20, 9, 0);

            (this.renderer as SpriteAnimator).AddAnimation("Fly", Constants.GlobalFPS, sprites.ToArray()).Play("Fly");
        }

        public override Vector2 CalculateVector() {
            var dot = Vector2.Dot(Vector2.Normalize(truckVelocity), direction);
            //if(dot > 0)
            //    return (direction * speed * Time.DeltaTime) + dot * truckVelocity;
            //return (direction * speed * Time.DeltaTime);
            return ((initV  + direction * speed) * Time.DeltaTime);
        }
        //CHANGED

        public override void OnHit(CollisionResult? result) {
            base.OnHit(result);

            var hitFX = Scene.AddEntity(new AnimatedEntity());

            hitFX.Position = this.Position;
            hitFX.Rotation = Random.NextAngle();

            var texture = Scene.Content.LoadTexture(ContentPaths.Scoop_Splat);
            var sprites = Sprite.SpritesFromAtlas(texture, 25, 25);

            hitFX.animator.AddAnimation("hit", Constants.GlobalFPS * 2,
                sprites.GetRange(TypeIndex(type) * 6, 6).ToArray()
            );

            hitFX.animator.Play("hit", SpriteAnimator.LoopMode.ClampForever);
            hitFX.animator.OnAnimationCompletedEvent += (s) => hitFX.Destroy();

            // Free this projectile
            Pool<Scoop>.Free(this);
        }

        public static ScoopType GetNext(ScoopType t) {
            if(t == (ScoopType)'C') return (ScoopType)'V';
            if(t == (ScoopType)'V') return (ScoopType)'S';
            return (ScoopType)'C';
        }

        public static int TypeIndex(ScoopType t) {
            if(t == (ScoopType)'C') return 0;
            if(t == (ScoopType)'V') return 1;
            return 2;
        }

        public enum ScoopType {
            Chocolate = 'C',
            Vanilla = 'V',
            Strawberry = 'S'
        }
    }
}
