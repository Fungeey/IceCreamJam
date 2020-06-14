using IceCreamJam.Entities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System;

namespace IceCreamJam.Tiled {
    class TilemapLoader : SceneComponent {

        public TiledMap tiledMap;
        public event Action OnLoad;
        public RectangleF cameraBounds;

        public override void OnEnabled() {
            base.OnEnabled();
        }

        public void Load(string filePath) {
			TmxMap map = Scene.Content.LoadTiledMap(filePath);
			tiledMap = Scene.AddEntity(new TiledMap(map));

			LoadSceneInfo();
			LoadBuildings();
			OnLoad?.Invoke();
		}

		private void LoadSceneInfo() {
			var infoLayer = tiledMap.map.GetLayer<TmxObjectGroup>(Constants.TiledLayerSceneInfo);
			foreach(TmxObject obj in infoLayer.Objects) {
				if(obj == null)
					continue;

				obj.Properties.TryGetValue(Constants.TiledPropertyInfoType, out var value);
				if(!string.IsNullOrEmpty(value) && value == Constants.TiledInfoCameraBounds)
					cameraBounds = new RectangleF(obj.X, obj.Y, obj.Width, obj.Height);
			}
		}

		private void LoadBuildings() {
			var backgroundLayer = tiledMap.map.GetLayer<TmxLayer>(Constants.TiledLayerBuildings);
			foreach(TmxLayerTile t in backgroundLayer.Tiles) {
				if(t == null)
					continue;

				t.TilesetTile.Properties.TryGetValue(Constants.TiledPropertyID, out var value);

				if(!string.IsNullOrEmpty(value)) {
					Scene.AddEntity(new Building(value) {
						Position = tiledMap.map.TileToWorldPosition(t.Position) + new Vector2(0, Constants.TiledCellSize),
					});
				}
			}
		}
	}
}
