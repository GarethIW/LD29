using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using LD29.Entities;
using LD29.EntityPools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;
using TimersAndTweens;
using LD29.Entities.Enemies;

namespace LD29.Screens
{
    class GameplayScreen : GameScreen
    {
        private Camera camera;
        private Map map;

        private ParticleController particleController = new ParticleController();
        private EnemyController enemyController;
        private ProjectileController projectileController;

        private Parallax waterParallax;
        private Parallax underwaterBGParallax;
        private Parallax rocksParallax;
        private Parallax cloudsParallax;

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
            MapGeneration.Generate(map);

            camera = new Camera(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight, map);

            //camera.Zoom = 0.5f;

            waterLevel = ScreenManager.Game.RenderHeight;
            waterParallax = new Parallax(content.Load<Texture2D>("abovewater-parallax"), 12, 0.5f, waterLevel, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, true);
            underwaterBGParallax = new Parallax(content.Load<Texture2D>("underwater-bg"), 4, 1f, waterLevel + 20, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), true, false);
            rocksParallax = new Parallax(content.Load<Texture2D>("seabed-rocks"), 16, 0.35f, (map.TileHeight*map.Height) -15, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, false);
            cloudsParallax = new Parallax(content.Load<Texture2D>("clouds"), 16, 0.35f, 25, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, false, true);
            
            playerShip = new Ship(content.Load<Texture2D>("playership"), new Rectangle(0,0,10,10), null, Vector2.Zero);
            playerShip.Position = new Vector2(64, 190);

            particleController.LoadContent(content);

            projectileController = new ProjectileController(1000, sheet => new Projectile(sheet, new Rectangle(0, 0, 4, 4), null, new Vector2(0, 0)) , content.Load<Texture2D>("projectiles"));
            projectileController.BoxCollidesWith.Add(playerShip);

            enemyController = new EnemyController(content.Load<Texture2D>("enemies"));
            enemyController.BoxCollidesWith.Add(playerShip);
            enemyController.BoxCollidesWith.Add(projectileController);

            //enemyController.SpawnInitial(1, map);

            ManOWar mow = new ManOWar(content.Load<Texture2D>("enemies"), new Rectangle(0, 0, 10, 10), null, Vector2.Zero);
            mow.Life = 3f;
            mow.Spawn(new Vector2(3000,90));
            enemyController.Enemies.Add(mow);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //xpos++;
            //if (xpos == 320*3) xpos = 0;
            playerShip.Update(gameTime, map);

            waterLevel = 260;
            waterParallax.Position.Y = waterLevel;
            underwaterBGParallax.Position.Y = waterLevel + 20;

            if (playerShip.Position.X < 0f)
            {
                playerShip.Position.X = (map.TileWidth * map.Width) + playerShip.Speed.X;
                camera.Position.X = (playerShip.Position.X + playerShip.Speed.X * 20f) - (camera.Target.X - camera.Position.X);
                //particleController.Wrap((map.TileWidth*map.Width));
                //projectileController.Wrap((map.TileWidth * map.Width));
            }
            if (playerShip.Position.X >= (map.TileWidth * map.Width))
            {
                playerShip.Position.X = 0f + playerShip.Speed.X;
                camera.Position.X = (playerShip.Position.X + playerShip.Speed.X * 20f) - (camera.Target.X - camera.Position.X);
                //particleController.Wrap(-(map.TileWidth * map.Width));
                //projectileController.Wrap(-(map.TileWidth * map.Width));
                //camera.Target.X += playerShip.Speed.X * 20f;
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
                {
                    playerShip.underWater = false;
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 pos = new Vector2(Helper.RandomFloat(-5f, 5f), 0f);
                        Color col = Color.Lerp(new Color(0,81,147), new Color(211,234,254), Helper.RandomFloat(0f,1f));
                        particleController.Add(playerShip.Position + pos,
                                               (pos * 0.1f) + new Vector2(playerShip.Speed.X, playerShip.Speed.Y * Helper.RandomFloat(0.25f, 2f)),
                                               0,2000,500, true, true, new Rectangle(0, 0,3,3),
                                               col, particle => { ParticleFunctions.FadeInOut(particle);
                                                                    if (particle.Position.Y > waterLevel)
                                                                        particle.State = ParticleState.Done;
                                               }, 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 1, ParticleBlend.Alpha);
                    }
                }

                waterParallax.HeightScale = MathHelper.Lerp(waterParallax.HeightScale, 0.1f, 0.05f);
            }

            particleController.Update(gameTime, map);
            enemyController.Update(gameTime, map);
            projectileController.Update(gameTime, map);

            waterParallax.Update(gameTime, playerShip.Speed.X, (int)camera.Position.X);
            underwaterBGParallax.Update(gameTime,playerShip.Speed.X*0.5f,(int)camera.Position.X);
            rocksParallax.Update(gameTime, playerShip.Speed.X, (int)camera.Position.X);
            cloudsParallax.Update(gameTime, playerShip.Speed.X, (int)camera.Position.X);

            camera.Target = playerShip.Position;
            camera.Target.X += playerShip.Speed.X * 20f;
            camera.Update(gameTime, playerShip.underWater, waterLevel);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            if(input.IsNewKeyPress(Keys.Back)) MapGeneration.Generate(map);
            playerShip.HandleInput(input);
            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 center = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight) / 2f;
            SpriteBatch sb = ScreenManager.SpriteBatch;

            ScreenManager.Game.GraphicsDevice.Clear(new Color(91,143,217));

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.Draw(ScreenManager.blankTexture, new Rectangle(0,(int)waterLevel-((int)camera.Position.Y-ScreenManager.Game.RenderHeight/2),ScreenManager.Game.RenderWidth, ((map.TileHeight*map.Height)+10)-(int)waterLevel), null, new Color(0,16,65));
            sb.End();

            underwaterBGParallax.Draw(sb, camera.Position.Y);
            waterParallax.Draw(sb, false, camera.Position.Y);
            rocksParallax.Draw(sb, false, camera.Position.Y);
            cloudsParallax.Draw(sb, true, camera.Position.Y);

            particleController.Draw(sb, camera, map, 0);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.CameraMatrix);
            map.DrawLayer(sb, "fg", camera);
            playerShip.Draw(sb, map);
            sb.End();

            enemyController.Draw(sb, camera, map);

            particleController.Draw(sb, camera, map, 1);
            projectileController.Draw(sb, camera, map);

            rocksParallax.Draw(sb, true, camera.Position.Y);
            cloudsParallax.Draw(sb, false, camera.Position.Y);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.Draw(ScreenManager.blankTexture, new Rectangle(0, (int)waterLevel - ((int)camera.Position.Y - ScreenManager.Game.RenderHeight / 2) - 5, ScreenManager.Game.RenderWidth, ((map.TileHeight * map.Height) + 10) - (int)waterLevel), null, Color.Black * 0.5f);
            sb.End();

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.End();

            waterParallax.Draw(sb, true, camera.Position.Y);


            ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);

            base.Draw(gameTime);

        }
    }
}
