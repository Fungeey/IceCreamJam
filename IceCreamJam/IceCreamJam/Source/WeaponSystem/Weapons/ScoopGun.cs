using IceCreamJam.Components;
using IceCreamJam.Entities;
using IceCreamJam.WeaponSystem.Projectiles;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Textures;
using System;

namespace IceCreamJam.WeaponSystem.Weapons {
    class ScoopGun : Weapon {

        private Scoop.ScoopType type = Scoop.ScoopType.Chocolate;
        private SpriteEntity coneDecal;
        private EntitySpringComponent coneSpring;
        private AnimatedEntity shootFX;

        private ArcadeRigidbody truckrb;

        public ScoopGun() {
            projectileType = typeof(Scoop);
            Name = "ScoopGun";
            ReloadTime = 0.1f;
            texturePath = ContentPaths.Scoop_Base;
            weaponMountOffset = new Vector2(0, -5);
            ammoCost = 1;
            iconIndex = 0;
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();
            var coneTexture = Scene.Content.LoadTexture(ContentPaths.Scoop_Cone);
            coneDecal = Scene.AddEntity(new SpriteEntity(coneTexture, Constants.RenderLayer_Truck, 0.5f));
            coneDecal.ToggleVisible(defaultVisible);
            coneSpring = coneDecal.AddComponent(new EntitySpringComponent(this, weaponMountOffset, 5));

            shootFX = Scene.AddEntity(new AnimatedEntity());
            AddFXAnimation();
            shootFX.ToggleVisible(defaultVisible);

            truckrb = weaponComponent.GetComponent<ArcadeRigidbody>();
        }

        private void AddFXAnimation() {
            void AddAnimation(Scoop.ScoopType type) {
                var texture = Scene.Content.LoadTexture(ContentPaths.Scoop + $"Scoop_FX_{(char)type}.png");
                var sprites = Sprite.SpritesFromAtlas(texture, 8, 23);
                shootFX.animator.AddAnimation(Enum.GetName(typeof(Scoop.ScoopType), type), Constants.GlobalFPS, sprites.ToArray());
            }

            AddAnimation(Scoop.ScoopType.Chocolate);
            AddAnimation(Scoop.ScoopType.Vanilla);
            AddAnimation(Scoop.ScoopType.Strawberry);

            shootFX.animator.RenderLayer = Constants.RenderLayer_Truck;
            shootFX.animator.LayerDepth = 0.4f;
        }

        public override void OnEquipped() {
            base.OnEquipped();
            coneDecal.Position = coneSpring.TargetPosition;
            coneSpring.RelativePosition = Vector2.Zero;

            coneDecal.ToggleVisible(true);
            shootFX.ToggleVisible(true);
        }

        public override void OnUnequipped() {
            base.OnUnequipped();
            coneDecal.ToggleVisible(false);
            shootFX.ToggleVisible(false);
        }

        public override Projectile InstantiateProjectile(Vector2 pos) {
            var scene = weaponComponent.Entity.Scene;
            var dir = Vector2.Normalize(scene.Camera.MouseToWorldPoint() - coneSpring.TargetPosition);

            type = Scoop.GetNext(type);
            var s = Pool<Scoop>.Obtain();
            s.Initialize(dir, pos + weaponMountOffset + dir * 4, type);
            s.truckVelocity = truckrb.Velocity * Time.DeltaTime;

            // Shock the cone
            coneSpring.Shock(-dir * 3);
            return s;
        }

        public override void OnShoot() {
            base.OnShoot();

            // Trigger shoot Fx
            shootFX.animator.Play(Enum.GetName(typeof(Scoop.ScoopType), type), Nez.Sprites.SpriteAnimator.LoopMode.ClampForever);
            shootFX.ToggleVisible(true);
        }

        public override void Update() {
            base.Update();

            // Update cone and Shoot fx
            var dir = Vector2.Normalize(Scene.Camera.MouseToWorldPoint() - coneSpring.TargetPosition);
            coneDecal.Rotation = shootFX.Rotation = Mathf.Atan2(dir.Y, dir.X);

            // Offset fx from cone
            shootFX.Position = coneSpring.TargetPosition + coneSpring.RelativePosition + dir * 8;
        }
    }
}
