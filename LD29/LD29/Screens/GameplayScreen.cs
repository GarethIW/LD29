using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using LD29.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using TimersAndTweens;

namespace LD29.Screens
{
    class GameplayScreen : GameScreen
    {
        private Camera camera;
        private Map map;

        private ParticleController particleController = new ParticleController();

        private Parallax waterParallax;
        private Parallax underwaterBGParallax;

        private float waterLevel;

        private Ship playerShip;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            map = content.Load<Map>("map");

            camera = new Camera(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight, map);

            waterLevel = ScreenManager.Game.RenderHeight;
            waterParallax = new Parallax(content.Load<Texture2D>("abovewater-parallax"), 12, 0.5f, waterLevel, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false);
            underwaterBGParallax = new Parallax(content.Load<Texture2D>("underwater-bg"), 4, 1f, waterLevel + 20, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), true);

            playerShip = new Ship(content.Load<Texture2D>("playership"), new Rectangle(0,0,10,10), null, Vector2.Zero);
            playerShip.Position = new Vector2((map.TileWidth*map.Width)/2, 150);

            

            particleController.LoadContent(content);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //xpos++;
            //if (xpos == 320*3) xpos = 0;
            playerShip.Update(gameTime, map);

            

            waterLevel = 224;
            waterParallax.Position.Y = waterLevel;
            underwaterBGParallax.Position.Y = waterLevel + 20;

            camera.Target = playerShip.Position;
            camera.Target.X += playerShip.Speed.X*20f;
            camera.Update(gameTime, playerShip.underWater, waterLevel);

            if (playerShip.Position.X < 0f)
            {
                playerShip.Position.X = (map.TileWidth * map.Width) + playerShip.Speed.X;
                camera.Position.X = (playerShip.Position.X + playerShip.Speed.X * 20f) - (camera.Target.X - camera.Position.X);
                //camera.Target.X -= playerShip.Speed.X * 20f;
            }
            if (playerShip.Position.X >= (map.TileWidth * map.Width))
            {
                playerShip.Position.X = 0f + playerShip.Speed.X;
                camera.Position.X = (playerShip.Position.X + playerShip.Speed.X * 20f) - (camera.Target.X - camera.Position.X);
                //camera.Target.X += playerShip.Speed.X * 20f; ;
            }

            if (!playerShip.underWater)
            {
                if(playerShip.Position.Y > waterLevel + 10) 
                    playerShip.underWater = true;
                waterParallax.HeightScale = MathHelper.Lerp(waterParallax.HeightScale, 0.65f, 0.1f);
            }
            if (playerShip.underWater)
            {
                if (playerShip.Position.Y < waterLevel - 10)
                    playerShip.underWater = false;

                waterParallax.HeightScale = MathHelper.Lerp(waterParallax.HeightScale, 0.1f, 0.05f);
            }

            particleController.Update(gameTime, map);

            waterParallax.Update(gameTime, playerShip.Speed.X, (int)playerShip.Position.X);
            underwaterBGParallax.Update(gameTime,playerShip.Speed.X*0.5f,(int)playerShip.Position.X);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            playerShip.HandleInput(input);
            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 center = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight) / 2f;
            SpriteBatch sb = ScreenManager.SpriteBatch;

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.Draw(ScreenManager.blankTexture, new Rectangle(0,(int)waterLevel-((int)camera.Position.Y-ScreenManager.Game.RenderHeight/2),ScreenManager.Game.RenderWidth, (map.TileHeight*map.Height)-(int)waterLevel), null, new Color(0,16,65));
            sb.End();

            underwaterBGParallax.Draw(sb, camera.Position.Y);
            waterParallax.Draw(sb, false, camera.Position.Y);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.CameraMatrix);
            map.DrawLayer(sb, "fg", camera);
            playerShip.Draw(sb);
            sb.End();

            particleController.Draw(sb, camera, 1);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.End();

            waterParallax.Draw(sb, true, camera.Position.Y);


            ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);

            base.Draw(gameTime);

        }
    }
}
