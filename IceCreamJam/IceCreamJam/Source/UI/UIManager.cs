using IceCreamJam.Components;
using IceCreamJam.Entities;
using IceCreamJam.Scenes;
using IceCreamJam.WeaponSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Nez.UI;
using System.Collections.Generic;

namespace IceCreamJam.UI {
    class UIManager : Entity {
        UICanvas frame, internals;

        private static readonly Color AmmoBackground = new Color(99, 107, 145);
        private static readonly Color AmmoHighlight = new Color(89, 197, 240);

        private Truck truck;
        private PlayerWeaponComponent playerWeapon;
        private PlayerMovementComponent playerMove;

        private ShadedImage healthBar, dashBar, speedBar;
        private Texture2D speedRegular, speedMaxed;
        private float previousSpeedValue;

        private ProgressBar ammo;
        private List<Image> weaponSlots;
        private List<Sprite> weaponIcons;

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            var mainScene = (Scene as MainScene);
            truck = mainScene.truck;
            playerWeapon = truck.GetComponent<PlayerWeaponComponent>();
            playerWeapon.OnWeaponShoot += a => UpdateAmmo(a);
            playerWeapon.OnWeaponCycle += UpdateWeaponIcons;

            playerMove = truck.GetComponent<PlayerMovementComponent>();

            truck.OnDamage += (h) => UpdateHealth(h);

            SetUpUI();
        }
        private void SetUpUI() {
            frame = AddComponent(new UICanvas());
            frame.IsFullScreen = true;
            frame.RenderLayer = -1;

            internals = AddComponent(new UICanvas());
            internals.IsFullScreen = true;
            internals.RenderLayer = -1;
            internals.LayerDepth = 0.5f;

            {
                int x = 20, y = 20;

                var frameTexture = Scene.Content.LoadTexture(ContentPaths.HealthBarFrame);
                var healthFrame = frame.Stage.AddElement(new Image(frameTexture));
                healthFrame.SetPosition(x, y);
                healthFrame.SetSize(508, 58);
                
                ShadedImage AddBar(string path, Vector2 pos) {
                    var effect = Scene.Content.LoadEffect(ContentPaths.MaskEffect);
                    var texture = Scene.Content.LoadTexture(path);
                    var image = new ShadedImage(effect, texture);
                    image.SetPosition(pos.X, pos.Y);
                    image.SetSize(texture.Width * 2, texture.Height * 2);
                    internals.Stage.AddElement(image);
                    return image;
                }

                speedRegular = Scene.Content.LoadTexture(ContentPaths.SpeedInternal);
                speedMaxed = Scene.Content.LoadTexture(ContentPaths.SpeedInternalMaxed);

                healthBar = AddBar(ContentPaths.HealthInternal, new Vector2(x + 50, y + 8));
                dashBar = AddBar(ContentPaths.DashInternal, new Vector2(x + 80, y + 34));
                speedBar = AddBar(ContentPaths.SpeedInternal, new Vector2(x + 342, y + 34));

            }
            {
                var x = 20;
                var y = 140;

                var texture = Scene.Content.LoadTexture(ContentPaths.AmmoFrame);
                var ammoFrame = frame.Stage.AddElement(new Table().Top().Left());
                ammoFrame.SetPosition(x, y);
                var cell = ammoFrame.Add(new Image(texture));
                cell.Size(64, 384);

                var style = ProgressBarStyle.Create(AmmoBackground, AmmoHighlight);
                style.KnobBefore.MinWidth = style.KnobAfter.MinWidth = 24;
                style.KnobBefore.MinHeight = 0;

                ammo = internals.Stage.AddElement(new ProgressBar(0, PlayerWeaponComponent.MaxAmmo, 1, true, style));
                ammo.SetSize(0, 324);
                ammo.SetOrigin((int)Align.TopLeft);
                ammo.SetPosition(x + 16 * 2, y + 29 * 2);
            }
            {
                weaponSlots = new List<Image>();

                var table = internals.Stage.AddElement(new Table());
                table.SetOrigin((int)VerticalAlign.Bottom);
                table.SetPosition(internals.Stage.GetWidth() / 2, internals.Stage.GetHeight() - 48 - 20);

                var iconTextures = Scene.Content.LoadTexture(ContentPaths.WeaponIcons);
                weaponIcons = Sprite.SpritesFromAtlas(iconTextures, 32, 32);

                void addIcon(int size) {
                    var image = new Image(Scene.Content.LoadTexture(ContentPaths.EmptySprite));
                    table.Add(image).Size(size).Pad(5);
                    weaponSlots.Add(image);
                }

                addIcon(64);
                addIcon(96);
                addIcon(64);

                UpdateWeaponIcons();
            }
        }

        public override void Update() {
            base.Update();

            Position = truck.Position;
            if(playerMove != null)
                UpdateSpeed(playerMove.Speed / PlayerMovementComponent.maxSpeed);
        }

        private void UpdateWeaponIcons() {
            weaponSlots[0].SetDrawable(new SpriteDrawable(weaponIcons[playerWeapon.NextWeapon.iconIndex]));
            weaponSlots[1].SetDrawable(new SpriteDrawable(weaponIcons[playerWeapon.ActiveWeapon.iconIndex]));
            weaponSlots[2].SetDrawable(new SpriteDrawable(weaponIcons[playerWeapon.PreviousWeapon.iconIndex]));
        }

        private void UpdateAmmo(float value) {
            ammo.SetValue(PlayerWeaponComponent.MaxAmmo - value);
        }

        private void UpdateHealth(float value) {
            healthBar.effect.Parameters["Progress"].SetValue(1 - Mathf.Clamp(value/Truck.MaxHealth, 0, 1));
        }

        private void UpdateSpeed(float value) {
            speedBar.effect.Parameters["Progress"].SetValue(1 - Mathf.Clamp(value, 0, 1));

            if(previousSpeedValue == 1 && value < 1) 
                speedBar.SetDrawable(new SpriteDrawable(speedRegular));
            else if(previousSpeedValue < 1 && value == 1)
                speedBar.SetDrawable(new SpriteDrawable(speedMaxed));

            previousSpeedValue = value;
        }
    }
}
