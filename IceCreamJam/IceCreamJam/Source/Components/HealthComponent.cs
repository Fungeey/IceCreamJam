using Nez;
using System;

namespace IceCreamJam.Source.Components {
	class HealthComponent : Component {
		float health;
		float maxHealth;

		public float Health {
			get => health; set {
				float clamped = Mathf.Clamp(value, 0, maxHealth);
				if (health != clamped) OnHealthChanged?.Invoke(clamped);
				health = clamped;
			}
		}

		/// <summary>
		/// fired when health is changed, positive or negative, and includes the new value
		/// </summary>
		public event OnHealthChangedHandler OnHealthChanged;
		public delegate void OnHealthChangedHandler(float newValue);

		public HealthComponent(float maxHealth, float initialHealth) {
			this.health = Math.Min(initialHealth, maxHealth);
			this.maxHealth = maxHealth;
		}

		public HealthComponent(float health) : this(health, health) { }

		public void Damage(float damageAmount) => Health -= Math.Max(0, damageAmount);
		public void Heal(float healAmount) => Health += Math.Max(0, healAmount);
	}
}
