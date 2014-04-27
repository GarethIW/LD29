using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;
using TimersAndTweens;
using Timer = TimersAndTweens.Timer;

namespace LD29.Entities
{
    class Ship : Entity
    {
        public bool underWater = false;

        private Color _tint = Color.White;
        private int faceDir = 1;

        private SpriteAnim _idleAnim;
        private SpriteAnim _upAnim;
        private SpriteAnim _downAnim;

        public Ship(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 16, 16, 100, new Vector2(8, 8));
            _idleAnim.Play();
            _upAnim = new SpriteAnim(spritesheet, 1, 2, 16, 16, 500, new Vector2(8, 8), false, false, 0);
            _upAnim.Pause();
            _downAnim = new SpriteAnim(spritesheet, 2, 2, 16, 16, 500, new Vector2(8, 8), false, false, 0);
            _downAnim.Pause();

            Active = true;
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            Position.Y = MathHelper.Clamp(Position.Y, 16, (gameMap.Height*gameMap.TileHeight) - 16);

            _tint = Color.White;

            _idleAnim.Update(gameTime);
            _upAnim.Update(gameTime);
            _downAnim.Update(gameTime);

            CheckMapCollisions(gameMap);

            base.Update(gameTime, gameMap);
        }

        public override void HandleInput(InputState input)
        {
            Speed.X = MathHelper.Lerp(Speed.X, 0f, 0.01f);
            Speed.Y = MathHelper.Lerp(Speed.Y, 0f, 0.01f);

            Rectangle particleRect = underWater ? new Rectangle(128, 0, 16, 16) : new Rectangle(0, 0, 3, 3);
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                Speed.X-=0.05f;
                faceDir = -1;
                ParticleController.Instance.Add(Position + new Vector2(5f,0f),
                                            new Vector2(Helper.RandomFloat(0f, 1f), Helper.RandomFloat(-0.2f, 0.2f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            underWater?Color.White:Color.Orange,
                                            ParticleFunctions.FadeInOut,
                                            underWater?0.3f:1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                Speed.X+=0.05f;
                faceDir = 1;
                ParticleController.Instance.Add(Position + new Vector2(-5f, 0f),
                                            new Vector2(Helper.RandomFloat(0f, -1f), Helper.RandomFloat(-0.2f, 0.2f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            underWater?Color.White:Color.Orange,
                                            ParticleFunctions.FadeInOut,
                                            underWater ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            }

            _idleAnim.CurrentFrame = 0;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Up))
            {
                Speed.Y -= 0.05f;
                _upAnim.Play();
                ParticleController.Instance.Add(Position + new Vector2(0f, 3f),
                                            new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, 1f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            underWater?Color.White:Color.Orange,
                                            ParticleFunctions.FadeInOut,
                                            underWater ? 0.2f : 0.75f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            }
            else _upAnim.Reset();
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Down))
            {
                Speed.Y += 0.05f;
                _downAnim.Play();
                ParticleController.Instance.Add(Position + new Vector2(0f, -3f),
                                            new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, -1f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            underWater?Color.White:Color.Orange,
                                            ParticleFunctions.FadeInOut,
                                            underWater ? 0.2f : 0.75f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            }
            else _downAnim.Reset();

            Speed.X = MathHelper.Clamp(Speed.X, -5f, 5f);
            Speed.Y = MathHelper.Clamp(Speed.Y, -1.5f, 1.5f);

            base.HandleInput(input);
        }

        public override void OnPolyCollision(Entity collided)
        {
            _tint = Color.Red;

            base.OnPolyCollision(collided);
        }

        public override void Draw(SpriteBatch sb)
        {
            if(_upAnim.State== SpriteAnimState.Playing) _upAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);
            else if(_downAnim.State== SpriteAnimState.Playing) _downAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);
            else _idleAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);

            base.Draw(sb);
        }

        private void CheckMapCollisions(Map gameMap)
        {
            // Check upward collision
            if (Speed.Y < 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Top - Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = 0;
                }

            // Check downward collision
            if (Speed.Y > 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Bottom + Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = 0;
                }

            // Check left collision
            if (Speed.X < 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Left - Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = 0;
                    }
                }

            // Check right collision
            if (Speed.X > 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Right + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = 0;
                    }
                }
        }
    }
}
