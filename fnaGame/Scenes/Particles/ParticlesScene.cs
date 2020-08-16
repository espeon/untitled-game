using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using fnaGame.Shared;
using Nez.Sprites;
using Nez;


namespace fnaGame.Scenes
{
	public class ParticlesScene : Scene
	{
		public override void Initialize()
		{
			ClearColor = Color.Black;
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);
			Screen.SetSize(1280, 720);

			// add the ParticleSystemSelector which handles input for the scene and a SimpleMover to move it around with the keyboard
			var particlesEntity = CreateEntity("particles");
			particlesEntity.SetPosition(Screen.Center - new Vector2(0, 200));
			particlesEntity.AddComponent(new ParticleSystemSelector());
			particlesEntity.AddComponent(new SimpleMover());


			// create a couple moons for playing with particle collisions
			var moonTex = Content.LoadTexture("Content/Textures/moon.png");

			var moonEntity = CreateEntity("moon");
			moonEntity.Position = new Vector2(Screen.Width / 2, Screen.Height / 2 + 200);
			moonEntity.AddComponent(new SpriteRenderer(moonTex));
			moonEntity.AddComponent<CircleCollider>();

			// clone the first moonEntity to create the second
			var moonEntityTwo = moonEntity.Clone(new Vector2(Screen.Width / 2 + 100, Screen.Height / 2 + 200));
			AddEntity(moonEntityTwo);
		}
	}
}