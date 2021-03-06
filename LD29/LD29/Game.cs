using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameStateManagement;
using LD29.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if WINDOWS || LINUX
using OpenTK;
#endif
using TimersAndTweens;
using Color = Microsoft.Xna.Framework.Color;
using Point = System.Drawing.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace LD29
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private int MAX_SCALE = 5;
        private int MIN_SCALE = 1;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private ScreenManager screenManager;

        private TimerController timerController = new TimerController();
        private TweenController tweenController = new TweenController();

        public int RenderWidth = 320;
        public int RenderHeight = 180;
        public int DisplayScale = 4;

        private RenderTarget2D renderTarget;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            MAX_SCALE = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width/RenderWidth;
            ChangeDisplayScale();

            screenManager = new ScreenManager(this);
            screenManager.Initialize();

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            screenManager.LoadContent();
            renderTarget = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            AudioController.LoadContent(Content);

            // Add the first screen here, usually either MainMenuScreen or GameplayScreen if you just want to go straight to gameplay
            //screenManager.AddScreen(new ExampleGameplayScreen());
            //screenManager.AddScreen(new GameplayScreen());
            screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            AudioController.Update(gameTime);

            screenManager.Update(gameTime);

            //timerController.Update(gameTime);
            //tweenController.Update(gameTime);

            // Allows the game to exit
            if (screenManager.Input.CurrentKeyboardState.IsKeyDown(Keys.F12)) this.Exit();

            // PgUp/PgDn change the display scaling
            if (screenManager.Input.CurrentKeyboardState.IsKeyDown(Keys.PageDown) && !screenManager.Input.LastKeyboardState.IsKeyDown(Keys.PageDown) && DisplayScale > MIN_SCALE)
            {
                DisplayScale--;
                ChangeDisplayScale();
            }
            if (screenManager.Input.CurrentKeyboardState.IsKeyDown(Keys.PageUp) && !screenManager.Input.LastKeyboardState.IsKeyDown(Keys.PageUp) && DisplayScale < MAX_SCALE)
            {
                DisplayScale++;
                ChangeDisplayScale();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // First, draw our game to the 1:1 rendertarget
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);
            screenManager.Draw(gameTime);

            // Then, draw the 1:1 rendertarget upscaled to our display resolution
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            // We use PointClamp sampling throughout for nearest-neighbour scaling
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(renderTarget, 
                             new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)/2, null, 
                             Color.White, 0f, 
                             new Vector2(renderTarget.Width,renderTarget.Height)/2, 
                             DisplayScale,
                             SpriteEffects.None, 1);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ToggleFullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            ChangeDisplayScale();
        }

        void ChangeDisplayScale()
        {
			#if MACOS
            graphics.PreferredBackBufferWidth = RenderWidth * DisplayScale;
            graphics.PreferredBackBufferHeight = RenderHeight * DisplayScale;
            graphics.ApplyChanges();
			this.Window.Window.Center();
			#endif

			#if WINDOWS || LINUX
            if (RenderWidth*DisplayScale == graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width)
            {
                ((OpenTKGameWindow) Window).Window.WindowState = WindowState.Fullscreen;
                graphics.IsFullScreen = true;
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                graphics.ApplyChanges();
            }
            else
            {
                ((OpenTKGameWindow) Window).Window.WindowState = WindowState.Normal;
                graphics.PreferredBackBufferWidth = RenderWidth * DisplayScale;
                graphics.PreferredBackBufferHeight = RenderHeight * DisplayScale;
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
                ((OpenTKGameWindow)Window).Window.Location = new Point((graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2) - (RenderWidth * DisplayScale) / 2, (graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2) - (RenderHeight * DisplayScale) / 2);
                ((OpenTKGameWindow) Window).Window.ClientSize = new Size(RenderWidth*DisplayScale,
                    RenderHeight*DisplayScale);
			}
			#endif
            

                //

        }




        
    }
}

