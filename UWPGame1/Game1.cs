using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UWPGame1
{
    public class Game1 : Game
    {
        static int _coreWindowWidth;
        static int _coreWindowHeight;

        static Game1()
        {
            var scale = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var window = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            _coreWindowWidth = (int)(0.5f + scale * window.Bounds.Width);
            _coreWindowHeight = (int)(0.5f + scale * window.Bounds.Height);

            GraphicsDeviceManager.DefaultBackBufferWidth = _coreWindowWidth;
            GraphicsDeviceManager.DefaultBackBufferHeight = _coreWindowHeight;
        }
        private SpriteBatch _batch;
        private Texture2D _texture1;
        private SoundEffect _sound;
        private KeyboardState _keyboardPrev = new KeyboardState();
        private Effect _grayscaleEffect;

        public Game1()
        {
            GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);
            var b = Window.ClientBounds;

            gdm.PreferredBackBufferWidth = _coreWindowWidth;
            gdm.PreferredBackBufferHeight = _coreWindowHeight;
            gdm.IsFullScreen = false;
            gdm.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            InputEventEXT.KeyDown += (e) =>
            {
                if (e.Key == Keys.F1)
                {
                    _sound.Play();

                    if (TextInputEXT.IsTextInputActive())
                        TextInputEXT.StopTextInput();
                    else
                        TextInputEXT.StartTextInput();
                }

                if (e.Key == Keys.F2)
                {
                    SDL3.SDL.SDL_ClearComposition(TextInputEXT.WindowHandle);
                }
            };

            TextInputEXT.ImeTextInput += (character) =>
            {
                Debug.WriteLine($"[Text Input] {character}");
            };

            TextInputEXT.TextEditing += (compStr, cursorPosition, length) =>
            {
                if (string.IsNullOrEmpty(compStr)) return;

                compStr = compStr.Insert(cursorPosition, "|");

                Debug.WriteLine("--------[Text Composition]--------");
                Debug.WriteLine($"CompString {compStr}");
                Debug.WriteLine($"CompCursor {cursorPosition}");
                Debug.WriteLine("==================================");
            };

            TextInputEXT.TextEditingCandidates += (candidates, selected, horizontal) =>
            {
                for (int i = 0; i < candidates.Length; i++)
                {
                    if (i == selected)
                        Debug.WriteLine($"*{i+1}.Candidates: {candidates[i]}");
                    else
                        Debug.WriteLine($"{i+1}.Candidates: {candidates[i]}");
                }
                Debug.WriteLine($"Candidate Size: {candidates.Length}, selection: {selected}, horizontal: {horizontal}");
            };

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
