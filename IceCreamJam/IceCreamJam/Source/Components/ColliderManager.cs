using Microsoft.Xna.Framework;
using Nez;
using Nez.PhysicsShapes;
using System.Collections.Generic;

namespace IceCreamJam.Components {
	class ColliderManager : Component {
		PolygonCollider collider;

		public int ColliderIndex {
			get => this.colliderIndex; set {
				SetIndex(value);
			}
		}

		string file;
		int colliderIndex;
		List<Vector2[]> polygons;

		public ColliderManager(string shapesFile, int startingIndex = 0) {
			this.file = shapesFile;
			this.colliderIndex = startingIndex;
		}

		public override void OnAddedToEntity() {
			polygons = Entity.Scene.Content.LoadPolygons(file);

			if (collider == null)
				this.collider = Entity.GetComponent<PolygonCollider>();
		}

		public override void OnEnabled() {
			SetIndex(colliderIndex);
		}

		public void SetIndex(int i) {
			colliderIndex = i;
			((Polygon)collider.Shape).SetPoints(polygons[colliderIndex]);
		}
	}
}
