#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using LD29;
using LD29.Entities;
using LD29.Screens;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        private Texture2D texLogo;

        private Parallax waterParallax;
        private Parallax skyBGParallax;
        private Parallax cloudsParallax;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.

            TransitionOnTime = TimeSpan.FromMilliseconds(1000);
            TransitionOffTime = TimeSpan.FromMilliseconds(500);

            MenuEntry playGameMenuEntry = new MenuEntry("Play", true);
            MenuEntry exitMenuEntry = new MenuEntry("Exit", true);

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;      
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        public override void LoadContent()
        {
            texLogo = ScreenManager.Game.Content.Load<Texture2D>("titles");

            waterParallax = new Parallax(ScreenManager.Game.Content.Load<Texture2D>("abovewater-parallax"), 12, 0.5f, 180, 960, new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, true);
            skyBGParallax = new Parallax(ScreenManager.Game.Content.Load<Texture2D>("sky-bg"), 72, 1f, 70, 960, new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, false);
            cloudsParallax = new Parallax(ScreenManager.Game.Content.Load<Texture2D>("clouds"), 16, 0.35f, 25, 960, new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, false, true);

            AudioController._songs["overwater-theme"].Volume = AudioController.MusicVolume;
            AudioController._songs["underwater-theme"].Volume = 0f;

            base.LoadContent();
        }

       

        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            waterParallax.Update(gameTime, 2f, 0);
            skyBGParallax.Update(gameTime, 0.1f, 0);
            cloudsParallax.Update(gameTime, 2f, 0);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.Game.GraphicsDevice.Clear(new Color(37, 59, 89));

            skyBGParallax.Draw(ScreenManager.SpriteBatch, 90);
            waterParallax.Draw(ScreenManager.SpriteBatch, false, 90);
            cloudsParallax.Draw(ScreenManager.SpriteBatch, true, 90);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            //ScreenManager.SpriteBatch.Draw(ScreenManager.blankTexture, new Rectangle(0,0,ScreenManager.Game.RenderWidth,ScreenManager.Game.RenderHeight), null, new Color(200,0,0));
            ScreenManager.SpriteBatch.Draw(texLogo, new Vector2(ScreenManager.Game.RenderWidth / 2, (ScreenManager.Game.RenderHeight/2) -40), new Rectangle(0,0,195,65), Color.White, 0f, new Vector2(98,32), 1f, SpriteEffects.None, 0);
            
            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            string fluff = "@garethiw and @gredgie for LD29";
            ScreenManager.SpriteBatch.DrawString(ScreenManager.FontSmall, fluff, new Vector2(ScreenManager.Game.RenderWidth / 2, ScreenManager.Game.RenderHeight - 30) + Vector2.One, Color.Black, 0f, ScreenManager.FontSmall.MeasureString(fluff) / 2, 1f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.FontSmall, fluff, new Vector2(ScreenManager.Game.RenderWidth / 2, ScreenManager.Game.RenderHeight - 30), Color.DarkGray, 0f, ScreenManager.FontSmall.MeasureString(fluff) / 2, 1f, SpriteEffects.None, 0);
            fluff = "WASD/Arrows, X/Space or 360 pad";
            ScreenManager.SpriteBatch.DrawString(ScreenManager.FontSmall, fluff, new Vector2(ScreenManager.Game.RenderWidth / 2, ScreenManager.Game.RenderHeight - 22) + Vector2.One, Color.Black, 0f, ScreenManager.FontSmall.MeasureString(fluff) / 2, 1f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.FontSmall, fluff, new Vector2(ScreenManager.Game.RenderWidth / 2, ScreenManager.Game.RenderHeight - 22), Color.DarkGray, 0f, ScreenManager.FontSmall.MeasureString(fluff) / 2, 1f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();

            cloudsParallax.Draw(ScreenManager.SpriteBatch, false, 90);
            waterParallax.Draw(ScreenManager.SpriteBatch, true,90);

            ScreenManager.FadeBackBufferToBlack(1f-TransitionAlpha);

            base.Draw(gameTime);
        }

        #region Handle Input


        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
