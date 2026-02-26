using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace FNAWinGame1;

public class Game1 : Game
{
	private Texture2D _texture1;

	public Game1()
	{
		GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

		// Typically you would load a config here...
		gdm.PreferredBackBufferWidth = 800;
		gdm.PreferredBackBufferHeight = 600;
		gdm.IsFullScreen = false;
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

		InputEventEXT.KeyDown += (e) =>
		{
			if (e.Key == Keys.F1)
			{
				if (TextInputEXT.IsTextInputActive())
					TextInputEXT.StopTextInput();
				else
					TextInputEXT.StartTextInput();
			}
		};

		TextInputEXT.ImeTextInput += (character) =>
		{
			Console.WriteLine($"[Text Input] {character}");
		};

		TextInputEXT.TextEditing += (compStr, cursorPosition, length) =>
		{
			if (string.IsNullOrEmpty(compStr)) return;

			compStr = compStr.Insert(cursorPosition, "|");

			Console.WriteLine("--------[Text Composition]--------");
			Console.WriteLine($"CompString {compStr}");
			Console.WriteLine($"CompCursor {cursorPosition}");
			Console.WriteLine("==================================");
		};

		TextInputEXT.TextEditingCandidates += (candidates, selected, horizontal) =>
		{
			for (int i = 0; i < candidates.Length; i++)
			{
				if (i == selected)
					Console.WriteLine($"*{i+1}.Candidates: {candidates[i]}");
				else
					Console.WriteLine($"{i+1}.Candidates: {candidates[i]}");
			}
			Console.WriteLine($"Candidate Size: {candidates.Length}, selection: {selected}, horizontal: {horizontal}");
		};
	}

	protected override void LoadContent()
	{
		// Create the batch...
		_batch = new SpriteBatch(GraphicsDevice);

		// Start and stop IME to fix wrong composition cursor bug
		TextInputEXT.StartTextInput();
		TextInputEXT.StopTextInput();

		_texture1 = Content.Load<Texture2D>("Image1");

		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		// Clean up after yourself!
		base.UnloadContent();
	}

	private SpriteBatch _batch;

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		// Draw the texture to the corner of the screen
		_batch.Begin(sortMode: SpriteSortMode.Deferred,
			blendState: BlendState.AlphaBlend,
			samplerState: SamplerState.PointClamp,
			depthStencilState: DepthStencilState.None,
			rasterizerState: RasterizerState.CullCounterClockwise);
		_batch.Draw(_texture1, Vector2.Zero, Color.White);
		_batch.End();


		base.Draw(gameTime);
	}
}
