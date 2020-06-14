using IceCreamJam.Entities;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Source.Components {
	class CameraFollowComponent : Component, IUpdatable {

		public Entity target;
		public RectangleF bounds;
		private readonly Camera camera;
		private readonly Truck truck;

		private const float mouseInfluence = 100;
		private const float lerpSmoothness = 10;

		public CameraFollowComponent(Camera camera, Entity target) {
			this.camera = camera;
			this.target = target;
			if(target is Truck)
				truck = (Truck)target;
		}

		public void Update() {
			var lerpPos = (target.Position - Entity.Position) / lerpSmoothness;

			var halfScreen = new Vector2(camera.Bounds.Width, camera.Bounds.Height) * 0.5f;
			var cameraMax = new Vector2(bounds.X + bounds.Width - halfScreen.X, bounds.Y + bounds.Height - halfScreen.Y);

			Entity.Position = Vector2.Clamp(Entity.Position + lerpPos, new Vector2(bounds.X, bounds.Y) + halfScreen, cameraMax) + GetMouseOffset();
		}

		private Vector2 GetMouseOffset() {
			if(truck == null)
				return Vector2.Zero;

			return truck.GetVectorToMouse() * 1/mouseInfluence;
		}
	}
}
