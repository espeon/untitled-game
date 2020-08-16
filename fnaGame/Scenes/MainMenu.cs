using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;

namespace fnaGame.Scenes
{
    public class MainMenu : Scene
    {
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
                Entity.SetScale(Mathf.Sin(Time.TimeSinceSceneLoad) * ScalingTotal + 5f);
            }
        }
        public override void OnStart()
        {
            base.OnStart();
            AddRenderer(new DefaultRenderer());
            ClearColor = new Color(25, 20, 25);

            var Logo = AddEntity(new Entity("Logo Spinner"));
            Logo.AddComponent(new SpriteRenderer(Content.LoadTexture("Content/Textures/Nez.png")));
            Logo.AddComponent(new LogoRotationComponent());
            Logo.AddComponent(new LogoScalingComponent());
            Logo.AddComponent(new CheckButton());
            //Logo.Transform.SetScale(8f);

            var Text = AddEntity(new Entity("Info Text"));
            Text.AddComponent(new TextComponent(Content.LoadBitmapFont("Content/Fonts/Pk.fnt"), "press X to start", new Vector2(-110, 200), Color.White));
            Text.Transform.SetScale(1f);
            Camera.Position = Logo.Position;
        }

        public class CheckButton : Component, IUpdatable
        {
            VirtualButton _okInput;

            public override void OnAddedToEntity()
            {
                Debug.Log("Listening for inputs...");
                _okInput = new VirtualButton();
                _okInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.X));
                _okInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));
            }
            public override void OnRemovedFromEntity()
            {
                // deregister virtual input
                _okInput.Deregister();
            }
            private void PlayButton_onClicked()
            {
                Core.StartSceneTransition(new FadeTransition(() => new DemoScene())
                {
                    //TransitionTexture = Core.Content.Load<Texture2D>("nez/textures/textureWipeTransition/wink")
                });
            }
            void IUpdatable.Update()
            {
                if (_okInput.IsDown)
                {
                    Debug.Log("next scene");
                    PlayButton_onClicked();
                }
            }
        }
    }
}
