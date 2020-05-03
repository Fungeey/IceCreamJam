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

			foreach(Node node in roadSystem.nodes) {
				batcher.DrawCircle(node.position, 50, Color.Blue, 5);
				foreach(Connection c in node.connections.Values) {
					batcher.DrawLine(node.position, node.position + Direction8Ext.ToVector2(c.direction) * 50, Color.Chartreuse, 5);
				}
			}

			if(roadSystem.truckTargetNode != null)
				batcher.DrawHollowRect(new Rectangle((int)roadSystem.truckTargetNode.position.X - 25, 
					(int)roadSystem.truckTargetNode.position.Y - 25, 50, 50), Color.White, 5);
		}
	}
}
