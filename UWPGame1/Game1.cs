using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace UWPGame1
{
    public class Game1 : Game
    {
        private SpriteBatch _batch;
        private Texture2D _texture1;
        private SoundEffect _sound;
        private KeyboardState _keyboardPrev = new KeyboardState();
        private Effect _grayscaleEffect;

        public Game1()
        {
            GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

            gdm.PreferredBackBufferWidth = 1024;
            gdm.PreferredBackBufferHeight = 768;
            gdm.IsFullScreen = false;
            gdm.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _batch = new SpriteBatch(GraphicsDevice);

            _texture1 = Content.Load<Texture2D>("Image1");
            _sound = Content.Load<SoundEffect>("Sound/120");
            _grayscaleEffect = Content.Load<Effect>("Effects/Grayscale");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardCur = Keyboard.GetState();

            if (keyboardCur.IsKeyDown(Keys.Space) && _keyboardPrev.IsKeyUp(Keys.Space))
            {
                _sound.Play();
            }

            _keyboardPrev = keyboardCur;

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
}
