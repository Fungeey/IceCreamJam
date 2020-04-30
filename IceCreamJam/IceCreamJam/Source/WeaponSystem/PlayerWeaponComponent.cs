using IceCreamJam.Components;
using IceCreamJam.Scenes;
using IceCreamJam.UI;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;

namespace IceCreamJam.WeaponSystem {
    class PlayerWeaponComponent : Component, IUpdatable {

        public const int MaxAmmo = 50;
        public int ammo;
        public UIManager ui;

        public List<Weapon> weapons;
        public int WeaponIndex { get; private set; } = 0;
        public Weapon ActiveWeapon => weapons[WeaponIndex];
        public Weapon PreviousWeapon => weapons[Utility.Mod(WeaponIndex - 1, weapons.Count)];
        public Weapon NextWeapon => weapons[Utility.Mod(WeaponIndex + 1, weapons.Count)];
        public PlayerAnimationComponent animationComponent;

        public event Action<int> OnWeaponShoot;
        public event Action OnWeaponCycle;

        public PlayerWeaponComponent(params Weapon[] weapons) : this(new List<Weapon>(weapons)) { }

        public PlayerWeaponComponent(List<Weapon> weapons) {
            this.weapons = weapons;

            foreach(Weapon w in weapons)
                w.weaponComponent = this;

            ammo = MaxAmmo;
        }

        public override void OnAddedToEntity() {
            ui = (Entity.Scene as MainScene).UICanvas;

            foreach(Weapon w in weapons) {
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
            ui.UpdateAmmo(ammo -= ActiveWeapon.ammoCost);
            OnWeaponShoot?.Invoke(ammo);
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
            if(animationComponent == null)
                animationComponent = Entity.GetComponent<PlayerAnimationComponent>();

            // TODO: Hook this up with the new SetAnimator so the weapons bob too.

            Vector2 weaponOffset =  new Vector2(0, -16);
            //if(animationComponent.Animator.CurrentFrame == 1)
            //    weaponOffset = new Vector2(0, -15);
            //else
            //    weaponOffset = new Vector2(0, -16);

            foreach(Weapon w in weapons)
                w.Position = Entity.Position + weaponOffset;
        }
    }
}
