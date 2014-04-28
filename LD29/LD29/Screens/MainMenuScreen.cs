#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
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
            base.LoadContent();
        }

        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            ScreenManager.SpriteBatch.Draw(ScreenManager.blankTexture, new Rectangle(0,0,ScreenManager.Game.RenderWidth,ScreenManager.Game.RenderHeight), null, new Color(200,0,0));
            ScreenManager.SpriteBatch.Draw(texLogo, new Vector2(ScreenManager.Game.RenderWidth / 2, (ScreenManager.Game.RenderHeight/2) -20), new Rectangle(0,0,195,65), Color.White, 0f, new Vector2(98,32), 1f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();

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
