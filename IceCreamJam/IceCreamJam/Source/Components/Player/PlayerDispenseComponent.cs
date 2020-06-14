using IceCreamJam.Source.Entities;
using Nez;

namespace IceCreamJam.Source.Components {
	class PlayerDispenseComponent : Component, IUpdatable {
		public void Update() {
			if(InputManager.dispense.IsPressed)
				Entity.Scene.AddEntity(new Treat() { Position = Entity.Position, velocity = (Input.MousePosition - Entity.Position).Normalized() * 5});
		}
	}
}
