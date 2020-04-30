using IceCreamJam.Scenes;
using IceCreamJam.WeaponSystem;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Textures;
using Nez.UI;
using System.Collections.Generic;

namespace IceCreamJam.UI {
    class UIManager : Entity {
        UICanvas frame, internals;

        private static readonly Color AmmoBackground = new Color(99, 107, 145);
        private static readonly Color AmmoHighlight = new Color(89, 197, 240);

        private PlayerWeaponComponent weapon;

        ProgressBar ammo;
        List<Image> weaponSlots;
        private List<Sprite> weaponIcons;

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            weapon = (Scene as MainScene).truck.GetComponent<PlayerWeaponComponent>();
            weapon.OnWeaponShoot += a => UpdateAmmo(a);
            weapon.OnWeaponCycle += UpdateWeaponIcons;

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
                table.SetPosition(internals.Stage.GetWidth() / 2, 48 + 20);

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

        private void UpdateWeaponIcons() {
            weaponSlots[0].SetDrawable(new SpriteDrawable(weaponIcons[weapon.NextWeapon.iconIndex]));
            weaponSlots[1].SetDrawable(new SpriteDrawable(weaponIcons[weapon.ActiveWeapon.iconIndex]));
            weaponSlots[2].SetDrawable(new SpriteDrawable(weaponIcons[weapon.PreviousWeapon.iconIndex]));
        }

        public void UpdateAmmo(float value) {
            ammo.SetValue(PlayerWeaponComponent.MaxAmmo - value);
        }
    }
}
