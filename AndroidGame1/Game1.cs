using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using SDL2;

namespace AndroidGame1;

public class Game1 : Game
{
	public Game1()
	{
		GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

		// Typically you would load a config here...
		gdm.IsFullScreen = true;
		gdm.SynchronizeWithVerticalRetrace = true;

		IsMouseVisible = true;

		// All content loaded will be in a "Content" folder
		Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		/* This is a nice place to start up the engine, after
		 * loading configuration stuff in the constructor
		 */
		base.Initialize();

		InputEventEXT.PointerDown += (e) =>
		{
			_sound.Play();
		};
	}

	protected override void LoadContent()
	{
		// Create the batch...
		_batch = new SpriteBatch(GraphicsDevice);

		// ... then load a texture from ./Content/FNATexture.png
		_texture1 = Content.Load<Texture2D>("Image1");
		_sound = Content.Load<SoundEffect>("Sound/120");
		_grayscaleEffect = Content.Load<Effect>("Effects/Grayscale");

		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		// Clean up after yourself!
		base.UnloadContent();
	}

	private SpriteBatch _batch;
	private Texture2D _texture1;
	private SoundEffect _sound;
	private Effect _grayscaleEffect;

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		// Draw the texture to the corner of the screen
		_batch.Begin(sortMode: SpriteSortMode.Deferred,
			effect: _grayscaleEffect,
			blendState: BlendState.AlphaBlend,
			samplerState: SamplerState.PointClamp,
			depthStencilState: DepthStencilState.None,
			rasterizerState: RasterizerState.CullCounterClockwise);
		_batch.Draw(_texture1, Vector2.Zero, Color.White);
		_batch.End();

		base.Draw(gameTime);
	}
}
