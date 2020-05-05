using Nez;
using System;

namespace IceCreamJam.Source.Components {
	class AmmoComponent : Component {
		// TODO: deduplicate code between this and health component by using some sort of... generic class?
		int ammo;
		int maxAmmo;

		public int Ammo {
			get => ammo; set {
				int clamped = Mathf.Clamp(value, 0, maxAmmo);
				if (ammo != clamped) OnAmmoChanged?.Invoke(clamped);
				ammo = clamped;
			}
		}

		public int MaxAmmo => maxAmmo;

		/// <summary>
		/// fired when ammo is changed, positive or negative, and includes the new value
		/// </summary>
		public event OnAmmoChangedHandler OnAmmoChanged;
		public delegate void OnAmmoChangedHandler(int newValue);

		public AmmoComponent(int maxAmmo, int initialAmmo) {
			this.ammo = Math.Min(initialAmmo, maxAmmo);
			this.maxAmmo = maxAmmo;
		}

		public AmmoComponent(int ammo) : this(ammo, ammo) { }

		public int Spend(int value) {
			Ammo -= Math.Max(0, value);
			return Ammo;
		}

		public int Reload(int value) {
			Ammo += Math.Max(0, value);
			return Ammo;
		}
	}
}
