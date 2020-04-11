﻿using IceCreamJam.Source.Content;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace IceCreamJam.Source.WeaponSystem.Projectiles {
    class BananaSmall : Projectile {

        private int dmg;
        private Collider otherCollider;
        private float startAngle;

        public override void Initialize(Vector2 direction, Vector2 position) {
            base.Initialize(direction, position);
            this.Name = "BananaSmall";
            this.speed = 5;
            this.damage = 1;
            this.lifetime = 2;
            this.dmg = 0;

            if(this.renderer != null)
                (this.renderer as SpriteAnimator).Play($"Fly{dmg}");

            this.otherCollider = null;
            this.startAngle = this.Rotation + Mathf.Deg2Rad * 90;
        }

        public override void SetupTextures() {
            var animator = AddComponent(new SpriteAnimator() {
                RenderLayer = Constants.Layer_Bullets
            });

            var texture = Scene.Content.LoadTexture(ContentPaths.Banana_Small);
            var sprites = Sprite.SpritesFromAtlas(texture, 24, 24, 0);

            for(int i = 0; i < 3; i++)
                animator.AddAnimation($"Fly{i}", Constants.GlobalFPS * 4, sprites.GetRange(i * 8, 8).ToArray());
            animator.Play("Fly0");

            this.renderer = animator;
        }

        public override Vector2 CalculateVector() {
            return new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation)) * speed;
        }

        public override void Update() {
            base.Update();
            CheckCollision();

            // Rotate
            this.Rotation = startAngle + lifeComponent.progress * Mathf.Deg2Rad * 360;
        }

        private void CheckCollision() {
            if(collider.CollidesWithAny(out var result)) {
                if(otherCollider == result.Collider)
                    return;

                if(dmg == 2)
                    Pool<BananaSmall>.Free(this);
                else
                    (this.renderer as SpriteAnimator).Play($"Fly{++dmg}");
                this.otherCollider = result.Collider;
            } else {
                this.otherCollider = null;
            }
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            (collider as CircleCollider).SetRadius(12);
            collider.LocalOffset = Vector2.Zero;
            collider.IsTrigger = true;
        }

        public override void OnHit(CollisionResult? result) {
            // Death by timeout
            if(!result.HasValue)
                Pool<BananaSmall>.Free(this);
        }
    }
}