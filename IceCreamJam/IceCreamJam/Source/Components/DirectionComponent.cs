using Nez;
using System;

namespace IceCreamJam.Components {
	class DirectionComponent : Component {
		[Inspectable]
		private Direction8 direction;

		public Direction8 Direction {
			get => this.direction; set {
				if (this.direction != value) OnDirectionChange?.Invoke(value);
				this.direction = value;
			}
		}

		public event Action<Direction8> OnDirectionChange;
	}
}
