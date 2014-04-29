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
        private PowerupController powerupController;

        private Parallax waterParallax;
        private Parallax underwaterBGParallax;
        private Parallax skyBGParallax;
        private Parallax rocksParallax;
        private Parallax cloudsParallax;

        private float waterLevel;

        private Ship playerShip;

        private HUD hud;

        private float _waveFade = 1f;
        private bool _endOfWave = false;
        private float _eowTimer = 0f;
        private bool _gameOver = false;
        private float _goFade = 0f;
        private float _goTimer = 0f;

        private bool _firstWave = true;

        private Texture2D text;

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

            text = content.Load<Texture2D>("titles");

            hud = new HUD(content.Load<Texture2D>("hud"));

            waterLevel = ScreenManager.Game.RenderHeight;
            waterParallax = new Parallax(content.Load<Texture2D>("abovewater-parallax"), 12, 0.5f, waterLevel, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, true);
            underwaterBGParallax = new Parallax(content.Load<Texture2D>("underwater-bg"), 4, 1f, waterLevel + 20, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), true, false);
            skyBGParallax = new Parallax(content.Load<Texture2D>("sky-bg"), 72, 1f, 70, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, false);
            rocksParallax = new Parallax(content.Load<Texture2D>("seabed-rocks"), 16, 0.35f, (map.TileHeight*map.Height) -15, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, false);
            cloudsParallax = new Parallax(content.Load<Texture2D>("clouds"), 16, 0.35f, 25, (map.TileWidth * map.Width), new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), false, false, true);
            
            playerShip = new Ship(content.Load<Texture2D>("playership"), new Rectangle(0,0,10,10), null, Vector2.Zero);
            playerShip.Position = new Vector2(64, 190);

            particleController.LoadContent(content);

            projectileController = new ProjectileController(1000, sheet => new Projectile(sheet, new Rectangle(0, 0, 4, 4), null, new Vector2(0, 0)) , content.Load<Texture2D>("projectiles"));
            enemyController = new EnemyController(content.Load<Texture2D>("enemies"), content.Load<Texture2D>("boss"));
            powerupController = new PowerupController(1000,sheet => new Powerup(sheet, new Rectangle(0, 0, 6, 6), null, Vector2.Zero),content.Load<Texture2D>("powerup"));

            powerupController.BoxCollidesWith.Add(playerShip);
            projectileController.BoxCollidesWith.Add(playerShip);
            projectileController.BoxCollidesWith.Add(enemyController);
            enemyController.BoxCollidesWith.Add(playerShip);
            enemyController.BoxCollidesWith.Add(projectileController);

            GameController.Reset();

            //enemyController.SpawnInitial(GameController.Wave, map);

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
            if(!_endOfWave) enemyController.Update(gameTime, map);
            projectileController.Update(gameTime, map);
            powerupController.Update(gameTime, map);


            waterParallax.Update(gameTime, playerShip.Speed.X, (int)camera.Position.X);
            underwaterBGParallax.Update(gameTime, playerShip.Speed.X * 0.5f, (int)camera.Position.X);
            skyBGParallax.Update(gameTime, playerShip.Speed.X * 0.1f, (int)camera.Position.X);
            rocksParallax.Update(gameTime, playerShip.Speed.X, (int)camera.Position.X);
            cloudsParallax.Update(gameTime, playerShip.Speed.X, (int)camera.Position.X);

            camera.Target = playerShip.Position;
            camera.Target.X += playerShip.Speed.X * 20f;

            //Enemy head = EnemyController.Instance.Enemies.FirstOrDefault(en => en is Boss && ((Boss) en).Head);
            //if (head != null)
            //{
            //    playerShip.Position = head.Position + new Vector2(0, -16);
            //    camera.Target = head.Position;
            //}

            camera.Update(gameTime, playerShip.underWater, waterLevel);

            hud.Update(gameTime, new Viewport(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight));

            if (enemyController.Enemies.Count == 0 && enemyController.NumToSpawn == 0 && !_endOfWave)
            {
                _endOfWave = true;
                TimerController.Instance.Create("", () =>
                {
                    GameController.Wave++;
                    TweenController.Instance.Create("", TweenFuncs.QuadraticEaseIn, tweenin =>
                    {
                        _waveFade = tweenin.Value;
                        if (tweenin.State == TweenState.Finished)
                        {
                            
                            MapGeneration.Generate(map);
                            enemyController.SpawnInitial(GameController.Wave,map);
                            playerShip.Reset();
                            projectileController.Reset();
                            _firstWave = false;

                            TweenController.Instance.Create("", TweenFuncs.Linear, eowtween =>
                            {
                                playerShip.Life += 0.2f;
                                _eowTimer = eowtween.Value;
                                if(eowtween.State== TweenState.Finished)
                                    TweenController.Instance.Create("", TweenFuncs.QuadraticEaseIn, tweenout =>
                                    {
                                        _waveFade = 1f - tweenout.Value;
                                        if (tweenout.State == TweenState.Finished)
                                        {
                                            _endOfWave = false;
                                        
                                        }
                                    }, 500, false, false);
                            }, 2000, false, false);
                        }
                    }, 500, false, false);
                }, GameController.Wave>0?2000:0, false);
            }

            if (playerShip.Life <= 0f && !_gameOver)
            {
                _gameOver = true;
                TimerController.Instance.Create("", () =>
                {
                    TweenController.Instance.Create("", TweenFuncs.QuadraticEaseIn, tweenin =>
                    {
                        _goFade = tweenin.Value;
                        if (tweenin.State == TweenState.Finished)
                        {
                            TweenController.Instance.Create("", TweenFuncs.Linear, eowtween =>
                            {
                                _goTimer = eowtween.Value;
                            }, 2000, false, false);
                        }
                    }, 1000, false, false);
                }, 2000, false);
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            if(input.IsNewKeyPress(Keys.Back)) MapGeneration.Generate(map);
            if (_endOfWave && _waveFade>=1f) return;

            if (_gameOver && _goTimer >= 1f)
            {
                if (input.IsNewKeyPress(Keys.X) || input.IsNewKeyPress(Keys.Space) || input.IsNewKeyPress(Keys.Enter) || input.IsNewKeyPress(Keys.Escape))
                    LoadingScreen.Load(ScreenManager, false, new MainMenuScreen());
            }

            playerShip.HandleInput(input);
            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 center = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight) / 2f;
            SpriteBatch sb = ScreenManager.SpriteBatch;

            ScreenManager.Game.GraphicsDevice.Clear(new Color(37,59,89));

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.Draw(ScreenManager.blankTexture, new Rectangle(0,(int)waterLevel-((int)camera.Position.Y-ScreenManager.Game.RenderHeight/2),ScreenManager.Game.RenderWidth, ((map.TileHeight*map.Height)+10)-(int)waterLevel), null, new Color(0,16,65));
            sb.End();

           

            underwaterBGParallax.Draw(sb, camera.Position.Y);
            skyBGParallax.Draw(sb, camera.Position.Y);
            waterParallax.Draw(sb, false, camera.Position.Y);
            rocksParallax.Draw(sb, false, camera.Position.Y);
            cloudsParallax.Draw(sb, true, camera.Position.Y);

            particleController.Draw(sb, camera, map, 0);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.CameraMatrix);
            map.DrawLayer(sb, "fg", camera);
            playerShip.Draw(sb, map, ScreenManager.Font);
            sb.End();

            enemyController.Draw(sb, camera, map);

            particleController.Draw(sb, camera, map, 1);
            projectileController.Draw(sb, camera, map);
            powerupController.Draw(sb,camera,map);

            rocksParallax.Draw(sb, true, camera.Position.Y);
            cloudsParallax.Draw(sb, false, camera.Position.Y);

            sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            sb.Draw(ScreenManager.blankTexture, new Rectangle(0, (int)waterLevel - ((int)camera.Position.Y - ScreenManager.Game.RenderHeight / 2) - 5, ScreenManager.Game.RenderWidth, ((map.TileHeight * map.Height) + 10) - (int)waterLevel), null, Color.Black * 0.4f);
            sb.End();

            //sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            //Enemy head = EnemyController.Instance.Enemies.FirstOrDefault(en => en is Boss && ((Boss)en).Head);
            //if (head != null) sb.DrawString(ScreenManager.Font, head.Position.ToString(), Vector2.One * 10, Color.White);
            //sb.End();

            waterParallax.Draw(sb, true, camera.Position.Y);

            if (_endOfWave)
            {
                sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
                sb.Draw(ScreenManager.blankTexture, new Rectangle(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), null, Color.White * (_firstWave?1f:_waveFade));
                int numdigits = GameController.Wave.ToString().Length;
                int wavepos = -38 - 10 - (numdigits*8);
                sb.Draw(text, center + new Vector2(wavepos, -16), new Rectangle(68, 66, 77, 32), Color.White * _waveFade);
                for(int i=0;i<numdigits;i++)
                    sb.Draw(text, center + new Vector2(wavepos + 77+10+ (i * 16), -16), new Rectangle(Convert.ToInt32(GameController.Wave.ToString().Substring(i,1))*32, 116, 32, 32), Color.White * _waveFade);
                sb.End();
            }

            if (_gameOver)
            {
                sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
                sb.Draw(ScreenManager.blankTexture, new Rectangle(0, 0, ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), null, Color.Black * 0.2f* _goFade);
                sb.Draw(text, center + new Vector2(-47, -32), new Rectangle(196, 0, 94, 65), Color.White * _goFade);
                sb.End();
            }

            hud.Draw(sb, new Viewport(0,0,ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight), camera, !_endOfWave, ScreenManager.Font);

            ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);

            base.Draw(gameTime);

        }
    }
}
