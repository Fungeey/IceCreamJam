using IceCreamJam.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using System.IO;

namespace IceCreamJam {
    public class Game1 : Nez.Core {
        public Game1() {
            Content.RootDirectory = "Content";

            ////old reformatting json stuff
            //Vector2[][] d;
            //using (var reader = File.OpenText("../../../../Content/truckcollision.json")) {
            //    d = Nez.Persistence.Json.FromJson<Vector2[][]>(reader.ReadToEnd());
            //}

            //using (var stream = File.Open("../../../../Content/truckcollision.json", FileMode.Truncate))
            //using (StreamWriter sr = new StreamWriter(stream)) {
            //    string json = Nez.Persistence.Json.ToJson(d, true);
            //    sr.Write(json);
            //}
        }

        protected override void Initialize() {
            base.Initialize();
            Nez.Scene.SetDefaultDesignResolution(1280, 720, Nez.Scene.SceneResolutionPolicy.ShowAll);
            Debug.Log(Screen.Size);
            Scene = new MainScene();
            IsMouseVisible = false;
        }
    }
}
