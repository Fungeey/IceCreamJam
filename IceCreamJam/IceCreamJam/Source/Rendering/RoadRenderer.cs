using IceCreamJam.RoadSystem;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Rendering {
	class RoadRenderer : DefaultRenderer {

		RoadSystemComponent roadSystem;

		public override void OnAddedToScene(Scene scene) {
			base.OnAddedToScene(scene);
			roadSystem = scene.GetSceneComponent<RoadSystemComponent>();
		}

		protected override void DebugRender(Scene scene, Camera cam) {
			base.DebugRender(scene, cam);

			var batcher = Graphics.Instance.Batcher;
		}
	}
}
