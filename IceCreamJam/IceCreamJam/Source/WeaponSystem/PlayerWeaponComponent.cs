using IceCreamJam.Components;
using IceCreamJam.Scenes;
using IceCreamJam.Source.Components;
using IceCreamJam.UI;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;

namespace IceCreamJam.WeaponSystem {
    class PlayerWeaponComponent : Component, IUpdatable {
        private SetAnimator setAnimator;
        private SpriteAnimator spriteAnimator;
        private AmmoComponent ammoComponent;

        public int Ammo {
            get => ammoComponent.Ammo; set => ammoComponent.Ammo = value;
        }

        public UIManager ui;

        public List<Weapon> weapons;
        public int WeaponIndex { get; private set; } = 0;
        public Weapon ActiveWeapon => weapons[WeaponIndex];
        public Weapon PreviousWeapon => weapons[Utility.Mod(WeaponIndex - 1, weapons.Count)];
        public Weapon NextWeapon => weapons[Utility.Mod(WeaponIndex + 1, weapons.Count)];

        public event Action<int> OnWeaponShoot;
        public event Action OnWeaponCycle;

        public PlayerWeaponComponent(params Weapon[] weapons) : this(new List<Weapon>(weapons)) { }

        public PlayerWeaponComponent(List<Weapon> weapons) {
            this.weapons = weapons;

            foreach (Weapon w in weapons)
                w.weaponComponent = this;
        }

        public override void OnAddedToEntity() {
            setAnimator = Entity.GetComponent<SetAnimator>();
            spriteAnimator = Entity.GetComponent<SpriteAnimator>();
            ammoComponent = Entity.GetComponent<AmmoComponent>();

            ui = (Entity.Scene as MainScene).UICanvas;

            foreach (Weapon w in weapons) {
                Entity.Scene.AddEntity(w);
                w.defaultVisible = false;
            }

            ActiveWeapon.defaultVisible = true;
        }

        public void CycleForward() {
            ActiveWeapon.SetEnabled(false);
            ActiveWeapon.OnUnequipped();
            WeaponIndex = Utility.Mod(WeaponIndex + 1, weapons.Count);
            ActiveWeapon.SetEnabled(true);
            ActiveWeapon.OnEquipped();

            OnWeaponCycle?.Invoke();
        }

        public void CycleBackwards() {
            ActiveWeapon.SetEnabled(false);
            ActiveWeapon.OnUnequipped();
            WeaponIndex = Utility.Mod(WeaponIndex - 1, weapons.Count);
            ActiveWeapon.SetEnabled(true);
            ActiveWeapon.OnEquipped();

            OnWeaponCycle?.Invoke();
        }

        public void Shoot() {
            ActiveWeapon.Shoot();
        }

        public void OnShoot() {
            int newValue = this.ammoComponent.Spend(this.ActiveWeapon.ammoCost);
            OnWeaponShoot?.Invoke(newValue);
        }

        public void Update() {
            UpdateWeapons();

            // Process Input
            if (InputManager.shoot.IsDown)
                Shoot();
            if (InputManager.switchWeapon.Value > 0)
                CycleForward();
            if (InputManager.switchWeapon.Value < 0)
                CycleBackwards();
        }

        private void UpdateWeapons() {
            Vector2 weaponOffset = setAnimator.CurrentAnimationSetName == "idle" && spriteAnimator.CurrentFrame == 1 ? new Vector2(0, -15) : new Vector2(0, -16);

            foreach (Weapon w in weapons)
                w.Position = Entity.Position + weaponOffset;
        }
    }
}
