using Nez;
using System;

namespace IceCreamJam.Components {
	class DirectionComponent : Component {
		[Inspectable]
		private Direction8 direction;

		[Inspectable]
		internal Direction8 PreviousDirection { get; private set; }
		internal Direction8 Direction {
			get => this.direction; set {
				if (this.direction != value) {
					OnDirectionChange?.Invoke(value);
				}
				this.PreviousDirection = direction;
				this.direction = value;
			}
		}

		public event Action<Direction8> OnDirectionChange;
	}
}
