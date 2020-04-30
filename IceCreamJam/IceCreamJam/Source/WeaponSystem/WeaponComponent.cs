using IceCreamJam.Source.Components;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System.Collections.Generic;

namespace IceCreamJam.Source.WeaponSystem {
    class WeaponComponent : Component, IUpdatable {
        public SpriteAnimator animator;

        public List<Weapon> weapons;
        private int weaponIndex = 0;
        public Weapon ActiveWeapon => weapons[weaponIndex];

        public WeaponComponent(params Weapon[] weapons) : this(new List<Weapon>(weapons)) { }

        public WeaponComponent(List<Weapon> weapons) {
            this.weapons = weapons;

            foreach(Weapon w in weapons)
                w.weaponComponent = this;
        }

        public override void OnAddedToEntity() {
            this.animator = Entity.GetComponent<SpriteAnimator>();

            foreach(Weapon w in weapons) {
                Entity.Scene.AddEntity(w);
                w.defaultVisible = false;
            }

            ActiveWeapon.defaultVisible = true;
        }

        public void CycleForward() {
            ActiveWeapon.SetEnabled(false);
            ActiveWeapon.OnUnequipped();
            weaponIndex = Utility.Mod(weaponIndex + 1, weapons.Count);
            ActiveWeapon.SetEnabled(true);
            ActiveWeapon.OnEquipped();
        }

        public void CycleBackwards() {
            ActiveWeapon.SetEnabled(false);
            ActiveWeapon.OnUnequipped();
            weaponIndex = Utility.Mod(weaponIndex - 1, weapons.Count);
            ActiveWeapon.SetEnabled(true);
            ActiveWeapon.OnEquipped();
        }

        public void Shoot() {
            ActiveWeapon.Shoot();
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
            Vector2 weaponOffset;
            if(animator.CurrentFrame == 1)
                weaponOffset = new Vector2(0, -15);
            else
                weaponOffset = new Vector2(0, -16);

            foreach(Weapon w in weapons)
                w.Position = Entity.Position + weaponOffset;
        }
    }
}
