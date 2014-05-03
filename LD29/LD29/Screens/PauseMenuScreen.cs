#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using LD29;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        private Texture2D text;



        private MenuEntry optionsMenuEntry;



        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game", true);
            optionsMenuEntry = new MenuEntry("Music", true);
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game", true);



            resumeGameMenuEntry.Selected += resumeGameMenuEntry_Selected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

            IsPopup = true;
        }

        public override void LoadContent()
        {
            text = ScreenManager.Game.Content.Load<Texture2D>("titles");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            optionsMenuEntry.Text = "Music: " + (AudioController.Music ? "On" : "Off");
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            AudioController.Togglemusic();
        }

        void resumeGameMenuEntry_Selected(object sender, EventArgs e)
        {
            ExitScreen();
        }


        #endregion

        #region Handle Input


        protected override void OnCancel(object sender, EventArgs e)
        {
            ExitScreen();
            base.OnCancel(sender, e);
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new MainMenuScreen());
            
        }

        void SaveGameMenuEntrySelected(object sender, EventArgs e)
        {
            OnCancel(sender, e);
        }

        void LoadGameMenuEntrySelected(object sender, EventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, false, new MultiplayerGameplayScreen());
                                                           
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new MainMenuScreen());
        }


        public override void Draw(GameTime gameTime)
        {

            ScreenManager.FadeBackBufferToBlack(0.3f*TransitionAlpha);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            ScreenManager.SpriteBatch.Draw(text, new Vector2(ScreenManager.Game.RenderWidth / 2, (ScreenManager.Game.RenderHeight / 2) - 20), new Rectangle(293, 35, 103, 31), Color.White, 0f, new Vector2(51, 15), new Vector2(1f+TransitionPosition, 1f-TransitionPosition), SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
