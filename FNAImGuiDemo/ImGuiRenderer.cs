using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;

namespace FNAImGuiDemo
{
    public sealed class ImGuiRenderer : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;

        private BasicEffect _effect;
        private readonly RasterizerState _rasterizerState;

        private byte[] _vertexData;
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;

        private byte[] _indexData;
        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;

        private readonly Dictionary<IntPtr, Texture2D> _loadedTextures;

        private Texture2D _fontAtlasTexture;
        private int _textureId;
        private IntPtr? _fontAtlasTextureId;
        private int _scrollWheelValue;

        private readonly Dictionary<ImGuiKey, Keys> KeyMap = new();

        public ImGuiRenderer(Game game)
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            _graphicsDevice = game.GraphicsDevice;

            _loadedTextures = new Dictionary<IntPtr, Texture2D>();

            _rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false,
                ScissorTestEnable = true,
                SlopeScaleDepthBias = 0
            };

            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            ImGui.GetStyle().Colors[(int)ImGuiCol.DockingEmptyBg] = Num.Vector4.Zero;
            SetupInput(io, game);
        }

        public unsafe void RebuildFontAtlas()
        {
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out var width, out var height, out var bytesPerPixel);

            var pixels = new byte[width * height * bytesPerPixel];
            Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

            _fontAtlasTexture?.Dispose();
            _fontAtlasTexture = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
            _fontAtlasTexture.SetData(pixels);

            if (_fontAtlasTextureId.HasValue)
            {
                UnbindTexture(_fontAtlasTextureId.Value);
            }

            _fontAtlasTextureId = BindTexture(_fontAtlasTexture);
            io.Fonts.SetTexID(_fontAtlasTextureId.Value);
            io.Fonts.ClearTexData();
        }

        public IntPtr BindTexture(Texture2D texture)
        {
            var id = new IntPtr(_textureId++);

            _loadedTextures.Add(id, texture);

            return id;
        }

        public void Dispose()
        {
            _fontAtlasTexture?.Dispose();
        }

        public void UnbindTexture(IntPtr textureId)
        {
            _loadedTextures.Remove(textureId);
        }

        public void BeginLayout(GameTime gameTime)
        {
            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ImGui.NewFrame();
        }

        public void EndLayout()
        {
            ImGui.Render();

            RenderDrawData(ImGui.GetDrawData());
        }

        void SetupInput(ImGuiIOPtr io, Game game)
        {
            KeyMap[ImGuiKey.Tab] = Keys.Tab;
            KeyMap[ImGuiKey.LeftArrow] = Keys.Left;
            KeyMap[ImGuiKey.RightArrow] = Keys.Right;
            KeyMap[ImGuiKey.UpArrow] = Keys.Up;
            KeyMap[ImGuiKey.DownArrow] = Keys.Down;
            KeyMap[ImGuiKey.PageUp] = Keys.PageUp;
            KeyMap[ImGuiKey.PageDown] = Keys.PageDown;
            KeyMap[ImGuiKey.Home] = Keys.Home;
            KeyMap[ImGuiKey.End] = Keys.End;
            KeyMap[ImGuiKey.Delete] = Keys.Delete;
            KeyMap[ImGuiKey.Backspace] = Keys.Back;
            KeyMap[ImGuiKey.Enter] = Keys.Enter;
            KeyMap[ImGuiKey.Escape] = Keys.Escape;
            KeyMap[ImGuiKey.A] = Keys.A;
            KeyMap[ImGuiKey.C] = Keys.C;
            KeyMap[ImGuiKey.V] = Keys.V;
            KeyMap[ImGuiKey.X] = Keys.X;
            KeyMap[ImGuiKey.Y] = Keys.Y;
            KeyMap[ImGuiKey.Z] = Keys.Z;

            TextInputEXT.TextInput += (character) =>
            {
                if (character == '\t')
                {
                    return;
                }

                ImGui.GetIO().AddInputCharacter(character);
            };

            ImGui.GetIO().Fonts.AddFontDefault();
        }

        Effect UpdateEffect(Texture2D texture)
        {
            _effect ??= new BasicEffect(_graphicsDevice);

            var io = ImGui.GetIO();
            var offset = 0f;

            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(
                offset,
                io.DisplaySize.X + offset,
                io.DisplaySize.Y + offset,
                offset,
                -1f,
                1f
            );
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.VertexColorEnabled = true;

            return _effect;
        }

        public void UpdateInput()
        {
            var io = ImGui.GetIO();

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            foreach (var item in KeyMap)
            {
                io.AddKeyEvent(item.Key, keyboard.IsKeyDown(item.Value));
            }

            io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

            io.DisplaySize = new Num.Vector2(
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight
            );
            io.DisplayFramebufferScale = new Num.Vector2(1f, 1f);

            io.MousePos = new Num.Vector2(mouse.X, mouse.Y);

            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

            var scrollDelta = mouse.ScrollWheelValue - _scrollWheelValue;
            io.MouseWheel = scrollDelta > 0
                ? 1
                : scrollDelta < 0
                    ? -1
                    : 0;
            _scrollWheelValue = mouse.ScrollWheelValue;
        }

        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
            var lastViewport = _graphicsDevice.Viewport;
            var lastScissorBox = _graphicsDevice.ScissorRectangle;

            _graphicsDevice.BlendFactor = Color.White;
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.RasterizerState = _rasterizerState;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

            // Setup projection
            _graphicsDevice.Viewport = new Viewport(
                0,
                0,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight
            );

            UpdateBuffers(drawData);

            RenderCommandLists(drawData);

            // Restore modified state
            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                _vertexBuffer?.Dispose();

                _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _vertexBuffer = new VertexBuffer(
                    _graphicsDevice,
                    ImGuiDrawVertexDeclaration.Declaration,
                    _vertexBufferSize,
                    BufferUsage.None
                );
                _vertexData = new byte[_vertexBufferSize * ImGuiDrawVertexDeclaration.Size];
            }

            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                _indexBuffer?.Dispose();

                _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
                _indexData = new byte[_indexBufferSize * sizeof(ushort)];
            }

            // Copy ImGui's vertices and indices to a set of managed byte arrays
            var vtxOffset = 0;
            var idxOffset = 0;

            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdLists[n];

                fixed (void* vtxDstPtr = &_vertexData[vtxOffset * ImGuiDrawVertexDeclaration.Size])
                {
                    fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
                    {
                        Buffer.MemoryCopy(
                            (void*)cmdList.VtxBuffer.Data,
                            vtxDstPtr,
                            _vertexData.Length,
                            cmdList.VtxBuffer.Size * ImGuiDrawVertexDeclaration.Size
                        );
                        Buffer.MemoryCopy(
                            (void*)cmdList.IdxBuffer.Data,
                            idxDstPtr,
                            _indexData.Length,
                            cmdList.IdxBuffer.Size * sizeof(ushort)
                        );
                    }
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }

            // Copy the managed byte arrays to the gpu vertex- and index buffers
            _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * ImGuiDrawVertexDeclaration.Size);
            _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
        }

        private void RenderCommandLists(ImDrawDataPtr drawData)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            var vtxOffset = 0;
            var idxOffset = 0;

            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdLists[n];

                for (var cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    var drawCmd = cmdList.CmdBuffer[cmdi];

                    if (!_loadedTextures.ContainsKey(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException(
                            $"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings"
                        );
                    }

                    _graphicsDevice.ScissorRectangle = new Rectangle(
                        (int)drawCmd.ClipRect.X,
                        (int)drawCmd.ClipRect.Y,
                        (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                        (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                    );

                    var effect = UpdateEffect(_loadedTextures[drawCmd.TextureId]);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

#pragma warning disable CS0618 // // FNA does not expose an alternative method.
                        _graphicsDevice.DrawIndexedPrimitives(
                            primitiveType: PrimitiveType.TriangleList,
                            baseVertex: vtxOffset,
                            minVertexIndex: 0,
                            numVertices: cmdList.VtxBuffer.Size,
                            startIndex: idxOffset,
                            primitiveCount: (int)drawCmd.ElemCount / 3
                        );
#pragma warning restore CS0618
                    }

                    idxOffset += (int)drawCmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
        }
    }

    public static class ImGuiDrawVertexDeclaration
    {
        public static readonly int Size;
        public static readonly VertexDeclaration Declaration;

        static ImGuiDrawVertexDeclaration()
        {
            unsafe { Size = sizeof(ImDrawVert); }

            var position = new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0);
            var uv = new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            var color = new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0);
            Declaration = new VertexDeclaration(Size, position, uv, color);
        }
    }
}