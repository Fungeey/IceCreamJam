using IceCreamJam.Entities;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Source.Components {
	class CameraFollowComponent : Component, IUpdatable {

		private const bool doClampBounds = true;

		public Entity target;
		public RectangleF bounds;
		private readonly Camera camera;
		private readonly Truck truck;

		private const float mouseInfluence = 100;
		private const float lerpSmoothness = 10;

		public CameraFollowComponent(Camera camera, Entity target) {
			this.camera = camera;
			this.target = target;
			if(target is Truck truck)
				this.truck = truck;

			camera.Position = target.Position;
		}

		public void Update() {
			var lerpPos = (target.Position - Entity.Position) / lerpSmoothness;

			var halfScreen = camera.Bounds.Size * 0.5f;

			if(doClampBounds) {
				camera.Position = Vector2.Clamp(Entity.Position + lerpPos, bounds.Location + halfScreen, bounds.Max - halfScreen) + GetMouseOffset();
				return;
			}
			camera.Position = Entity.Position + lerpPos + GetMouseOffset();
		}

		private Vector2 GetMouseOffset() {
			if(truck == null)
				return Vector2.Zero;

			return truck.GetVectorToMouse() * 1/mouseInfluence;
		}
	}
}
