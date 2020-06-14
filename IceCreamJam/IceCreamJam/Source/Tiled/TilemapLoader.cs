using IceCreamJam.Entities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System;
using System.Collections.Generic;

namespace IceCreamJam.Tiled {
    class TilemapLoader : SceneComponent {

        public TiledMap tiledMap;
        public event Action OnLoad;
		public bool mapIsLoaded;

		public RectangleF mapBounds;
		public List<Vector2> carSpawnPoints;

        public override void OnEnabled() {
            base.OnEnabled();
        }

        public void Load(string filePath) {
			TmxMap map = Scene.Content.LoadTiledMap(filePath);
			tiledMap = Scene.AddEntity(new TiledMap(map));

			LoadSceneInfo();
			LoadBuildings();
			OnLoad?.Invoke();
			mapIsLoaded = true;
		}

		private void LoadSceneInfo() {
			var infoLayer = tiledMap.map.GetLayer<TmxObjectGroup>(Constants.TiledLayerSceneInfo);

			carSpawnPoints = new List<Vector2>();
			foreach(TmxObject obj in infoLayer.Objects) {
				if(obj == null)
					continue;

				if(string.IsNullOrEmpty(obj.Name))
					continue;

				if(obj.Name == Constants.TiledInfoCameraBounds)
					mapBounds = new RectangleF(obj.X, obj.Y, obj.Width, obj.Height);

				if(obj.Name == Constants.TiledInfoCarSpawn)
					carSpawnPoints.Add(new Vector2(obj.X, obj.Y));
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
