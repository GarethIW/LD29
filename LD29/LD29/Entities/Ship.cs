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

        private double projectileCoolDown1 = 50;
        private double projectileCoolDown2 = 1000;

        private double projectileTime1;
        private double projectileTime2;

        private int powerUpLevel = 3;

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
            projectileTime1 -= gameTime.ElapsedGameTime.TotalMilliseconds;
            projectileTime2 -= gameTime.ElapsedGameTime.TotalMilliseconds;

            Position.Y = MathHelper.Clamp(Position.Y, 16, (gameMap.Height*gameMap.TileHeight) - 16);

            _tint = Color.White;

            _idleAnim.Update(gameTime);
            _upAnim.Update(gameTime);
            _downAnim.Update(gameTime);

            CheckMapCollisions(gameMap);

            Speed.X = MathHelper.Clamp(Speed.X, -5f, 5f);
            Speed.Y = MathHelper.Clamp(Speed.Y, -1.5f, 1.5f);

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
                if (!underWater)
                    ParticleController.Instance.Add(Position + new Vector2(5f, 0f),
                                            new Vector2(Helper.RandomFloat(0f, 1f), Helper.RandomFloat(-0.2f, 0.2f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            new Color(new Vector3(1f) * (0.5f + Helper.RandomFloat(0.5f))),
                                            ParticleFunctions.Smoke,
                                            underWater ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
                ParticleController.Instance.Add(Position + new Vector2(5f,0f),
                                            new Vector2(Helper.RandomFloat(0f, 0.2f), Helper.RandomFloat(-0.2f, 0.2f)),
                                            0, underWater?Helper.RandomFloat(500, 1000):Helper.RandomFloat(50, 150), 50,
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
                if (!underWater)
                    ParticleController.Instance.Add(Position + new Vector2(-5f, 0f),
                                            new Vector2(Helper.RandomFloat(0f, -1f), Helper.RandomFloat(-0.2f, 0.2f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            new Color(new Vector3(1f) * (0.5f + Helper.RandomFloat(0.5f))),
                                            ParticleFunctions.Smoke,
                                            underWater ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
                ParticleController.Instance.Add(Position + new Vector2(-5f, 0f),
                                            new Vector2(Helper.RandomFloat(0f, -0.2f), Helper.RandomFloat(-0.2f, 0.2f)),
                                            0, underWater ? Helper.RandomFloat(500, 1000) : Helper.RandomFloat(50, 150), 50,
                                            false, true,
                                            particleRect,
                                            underWater ? Color.White : Color.Orange,
                                            ParticleFunctions.FadeInOut,
                                            underWater ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            }

            _idleAnim.CurrentFrame = 0;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Up))
            {
                Speed.Y -= 0.05f;
                _upAnim.Play();
                if (!underWater)
                    ParticleController.Instance.Add(Position + new Vector2(0f, 3f),
                                            new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, 1f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            new Color(new Vector3(1f) * (0.5f + Helper.RandomFloat(0.5f))),
                                            ParticleFunctions.Smoke,
                                            underWater ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
                ParticleController.Instance.Add(Position + new Vector2(0f, 3f),
                                            new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, 0.2f)),
                                            0, underWater ? Helper.RandomFloat(500, 1000) : Helper.RandomFloat(50, 150), 50,
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
                if (!underWater)
                    ParticleController.Instance.Add(Position + new Vector2(0f, -3f),
                                            new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, -1f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            particleRect,
                                            new Color(new Vector3(1f) * (0.5f + Helper.RandomFloat(0.5f))),
                                            ParticleFunctions.Smoke,
                                            underWater ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
                ParticleController.Instance.Add(Position + new Vector2(0f, -3f),
                                            new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, -0.2f)),
                                            0, underWater ? Helper.RandomFloat(500, 1000) : Helper.RandomFloat(50, 150), 50,
                                            false, true,
                                            particleRect,
                                            underWater ? Color.White : Color.Orange,
                                            ParticleFunctions.FadeInOut,
                                            underWater ? 0.2f : 0.75f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            }
            else _downAnim.Reset();



            if (input.CurrentKeyboardState.IsKeyDown(Keys.X))
            {
                if (projectileTime1 <= 0)
                {
                    projectileTime1 = projectileCoolDown1;


                    if (powerUpLevel != 1)
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile) entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile) entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false; 
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f*faceDir,0f);
                            entity.Position = Position + new Vector2(faceDir*8, 0);
                            
                        });

                    if (powerUpLevel == 1)
                    {
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile)entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile)entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f * faceDir, 0f);
                            entity.Position = Position + new Vector2(faceDir * 8, -5);

                        });
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile)entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile)entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f * faceDir, 0f);
                            entity.Position = Position + new Vector2(faceDir * 8, 5);

                        });
                    }

                    if (powerUpLevel >= 2)
                    {
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile) entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile) entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f*faceDir, -0.5f);
                            entity.Position = Position + new Vector2(faceDir*8, 0);
                        });
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile)entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile)entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f * faceDir, 0.5f);
                            entity.Position = Position + new Vector2(faceDir * 8, 0);
                        });
                    }

                    

                }

                if (projectileTime2 <= 0)
                {
                    projectileTime2 = projectileCoolDown2;

                    if (powerUpLevel >= 3)
                    {
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile) entity).Type = ProjectileType.Bomb;
                            ((Projectile) entity).SourceRect = new Rectangle(16, 0, 8, 8);
                            ((Projectile) entity).Life = 5000;
                            ((Projectile) entity).Scale = 0.5f;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 3f;
                            entity.Speed = new Vector2(1f*faceDir, 0f);
                            entity.Position = Position + new Vector2(0, 5);
                        });
                    }
                }
            }

            base.HandleInput(input);
        }

        public override void OnBoxCollision(Entity collided, Rectangle intersect)
        {
            _tint = Color.Red;

            base.OnBoxCollision(collided, intersect);
        }

        public override void OnPolyCollision(Entity collided)
        {
            _tint = Color.Red;

            base.OnPolyCollision(collided);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            if(_upAnim.State== SpriteAnimState.Playing) _upAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);
            else if(_downAnim.State== SpriteAnimState.Playing) _downAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);
            else _idleAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);

            base.Draw(sb, gameMap);
        }

        private void CheckMapCollisions(Map gameMap)
        {
            // Check upward collision
            if (Speed.Y < 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Top + Speed.Y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.Y = -Speed.Y/2;
                        return;
                    }
                }

            // Check downward collision
            if (Speed.Y > 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Bottom + Speed.Y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.Y = -Speed.Y/2;
                        return;
                    }
                }

            // Check left collision
            if (Speed.X < 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Left + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = -Speed.X/2;
                        return;
                    }
                }

            // Check right collision
            if (Speed.X > 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Right + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = -Speed.X/2;
                        return;
                    }
                }
        }
    }
}
