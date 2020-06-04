using System;
using Nez;

namespace IceCreamJam.Components {
	class PlayerInputComponent : Component, IUpdatable {
		private bool previnputHeld;
		private bool currInputHeld;
		private Direction8? prevDirection;

		public bool InputHeld => currInputHeld;
		public bool InputStart => !previnputHeld && currInputHeld;
		public bool InputEnd => previnputHeld && !currInputHeld;

		public event Action<Direction8> OnInputStart;
		
		public bool directionChange() {
			if (previnputHeld != currInputHeld) {
				return true;
			}
			return false;
		}
		public void Update() {
			// poll the static input manager for current input information
			bool? isNorth = InputManager.yAxis.Value == 0 ? null : (bool?)(InputManager.yAxis.Value < 0);
			bool? isWest = InputManager.xAxis.Value == 0 ? null : (bool?)(InputManager.xAxis.Value < 0);

			previnputHeld = currInputHeld;
			currInputHeld = isNorth.HasValue || isWest.HasValue;

			// convert the directional input information into a Direction8
			var newDir = DirectionFromInput(isNorth, isWest);
			if (newDir.HasValue && (InputStart || prevDirection != newDir))
				OnInputStart?.Invoke(newDir.Value);
			prevDirection = newDir;

		}

		private static Direction8? DirectionFromInput(bool? isNorth, bool? isWest) {
			if (!isNorth.HasValue && !isWest.HasValue) return null;
			if (isNorth.HasValue) {
				if (isNorth.Value) {
					if (isWest.HasValue)
						return isWest.Value ? (Direction8?)Direction8.NorthWest : (Direction8?)Direction8.NorthEast;
					else return Direction8.North;
				} else {
					if (isWest.HasValue)
						return isWest.Value ? (Direction8?)Direction8.SouthWest : (Direction8?)Direction8.SouthEast;
					else return Direction8.South;
				}
			} else {
				return isWest.Value ? (Direction8?)Direction8.West : (Direction8?)Direction8.East;
			}
		}
	}
}
