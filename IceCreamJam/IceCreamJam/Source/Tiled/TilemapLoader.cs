using Nez;
using Nez.Tiled;
using System;

namespace IceCreamJam.Tiled {
    class TilemapLoader : SceneComponent {

        public TiledMap tiledMap;
        public event Action OnLoad;

        public override void OnEnabled() {
            base.OnEnabled();
        }

        public void Load(string filePath) {
            TmxMap map = Scene.Content.LoadTiledMap(filePath);
            tiledMap = Scene.AddEntity(new TiledMap(map));

            OnLoad?.Invoke();
        }
    }
}
