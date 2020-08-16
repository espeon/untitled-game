using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tiled;

using fnaGame.Controllers;
using fnaGame.Shared;

using HeatWaveShader.Effects;

namespace fnaGame.Scenes
{
    /// <summary>
    /// Simple test scene
    /// </summary>
    public class DemoScene : Scene
    {
        int ScalingFactor = 3;
        FollowCamera camera;
        TmxMap map;

        HeatDistortionPointPostProcessor HeatDistortionPostProcessor;
        public class LogoRotationComponent : Component, IUpdatable
        {
            const float RotationTotal = 0.25f;
            void IUpdatable.Update()
            {
                Entity.Rotation = Mathf.Sin(Time.TimeSinceSceneLoad) * RotationTotal;
            }
        }
        public class LogoScalingComponent : Component, IUpdatable
        {
            const float ScalingTotal = .5f;
            void IUpdatable.Update()
            {
                Entity.SetScale(Mathf.Sin(Time.TimeSinceSceneLoad) * ScalingTotal + 2f);
            }
        }
        public override void Initialize()
        {
            SetDesignResolution(360, 180, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(360 * ScalingFactor, 180 * ScalingFactor);
            AddRenderer(new DefaultRenderer());

            // load up our TiledMap
            map = Content.LoadTiledMap("Content/Tilemaps/testMap.tmx");
            var spawnObject = map.GetObjectGroup("objects").Objects["spawn"];

            var tiledEntity = CreateEntity("tiled-map-entity");
            var tiledMapRenderer = tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));

            tiledMapRenderer.SetRenderLayer(3);


            // set up the player/char controller
            var playerEntity = CreateEntity("player", new Vector2(30, 60));
            playerEntity.AddComponent(new PlayerController());
            playerEntity.AddComponent(new CameraShake());
            playerEntity.AddComponent(new BoxCollider(-8, -16, 16, 32));
            playerEntity.AddComponent(new TiledMapMover(map.GetLayer<TmxLayer>("main")));
            playerEntity.AddComponent(new TriggerListener());

            //set camera bounds (see Controllers/CameraBounds.cs)
            var topLeft = new Vector2(map.TileWidth - 13, map.TileWidth - 13);
            var bottomRight = new Vector2(map.TileWidth * (map.Width + 3),
                map.TileWidth * (map.Height + 3));
            tiledEntity.AddComponent(new CameraBounds(topLeft, bottomRight));

            // add in spinny Nez logo background
            var Logo = AddEntity(new Entity("Logo Spinner"));
            Logo.AddComponent(new SpriteRenderer(Content.LoadTexture("Content/Textures/Nez.png")));
            Logo.SetPosition(Screen.Center / 2);
            Logo.AddComponent(new LogoRotationComponent());
            Logo.AddComponent(new LogoScalingComponent());
            Logo.GetComponent<SpriteRenderer>().RenderLayer = 5;

            Debug.DrawHollowRect(Camera.Bounds, Color.Green, 10000);

            // make it so camera follows player
            camera = Camera.Entity.AddComponent(new FollowCamera(playerEntity));
            Debug.DrawHollowRect(camera.Deadzone, Color.Red, 10000);

            var effect = Content.LoadEffect("Content/Shaders/HeatDistortionPoint.fxb");
            HeatDistortionPostProcessor = new HeatDistortionPointPostProcessor(0, effect);
            AddPostProcessor(HeatDistortionPostProcessor);
        }
        public override void Update()
        {
            base.Update();
            HeatDistortionPostProcessor.update();
            if (Nez.Input.LeftMouseButtonPressed)
            {
                Debug.Log("Left mouse button pressed.");
                PlayPostProcessor(Nez.Input.MousePosition);
            }
        }

        public void PlayPostProcessor(Vector2 pos)
        {
            pos.X = pos.X / (map.TileWidth * 16);
            pos.Y = pos.Y / (map.TileHeight * 16);
            HeatDistortionPostProcessor.play(pos, 750f);
        }
    }
}
