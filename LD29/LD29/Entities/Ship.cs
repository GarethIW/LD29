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
        public static Ship Instance;

        public bool underWater = false;
        public float Life = 100f;

        private float _hitAlpha = 0f;
        private int faceDir = 1;

        private SpriteAnim _idleAnim;
        private SpriteAnim _upAnim;
        private SpriteAnim _downAnim;
        private SpriteAnim _idleHitAnim;
        private SpriteAnim _upHitAnim;
        private SpriteAnim _downHitAnim;

        private double projectileCoolDown1 = 50;
        private double projectileCoolDown2 = 1000;

        private double projectileTime1;
        private double projectileTime2;

        public int PowerUpMeter = 0;

        public int PowerUpLevel = 0;

        public Ship(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            Instance = this;

            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 16, 16, 100, new Vector2(8, 8));
            _idleAnim.Play();
            _upAnim = new SpriteAnim(spritesheet, 1, 2, 16, 16, 500, new Vector2(8, 8), false, false, 0);
            _upAnim.Pause();
            _downAnim = new SpriteAnim(spritesheet, 2, 2, 16, 16, 500, new Vector2(8, 8), false, false, 0);
            _downAnim.Pause();

            _idleHitAnim = new SpriteAnim(spritesheet, 3, 1, 16, 16, 100, new Vector2(8, 8));
            _idleHitAnim.Play();
            _upHitAnim = new SpriteAnim(spritesheet, 4, 2, 16, 16, 500, new Vector2(8, 8), false, false, 0);
            _upHitAnim.Pause();
            _downHitAnim = new SpriteAnim(spritesheet, 5, 2, 16, 16, 500, new Vector2(8, 8), false, false, 0);
            _downHitAnim.Pause();

            Active = true;
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {

            if (!Active)
            {
                Speed = Vector2.Zero;
                return;
            }

            projectileTime1 -= gameTime.ElapsedGameTime.TotalMilliseconds;
            projectileTime2 -= gameTime.ElapsedGameTime.TotalMilliseconds;

            Position.Y = MathHelper.Clamp(Position.Y, 16, (gameMap.Height*gameMap.TileHeight) - 16);

            _idleAnim.Update(gameTime);
            _upAnim.Update(gameTime);
            _downAnim.Update(gameTime);
            _idleHitAnim.Update(gameTime);
            _upHitAnim.Update(gameTime);
            _downHitAnim.Update(gameTime);

            CheckMapCollisions(gameMap);

            Speed.X = MathHelper.Clamp(Speed.X, -5f, 5f);
            Speed.Y = MathHelper.Clamp(Speed.Y, -1.5f, 1.5f);

            if (_hitAlpha > 0f) _hitAlpha -= 0.02f;

            if (Life <= 0f) Die();

            if (PowerUpMeter >= 20 && PowerUpLevel<4)
            {
                PowerUpLevel++;
                PowerUpMeter = PowerUpMeter-20;
            }

            base.Update(gameTime, gameMap);
        }

        public override void HandleInput(InputState input)
        {
            if (!Active) return;

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
                _upHitAnim.Play();
                if (!underWater)
                    ParticleController.Instance.Add(Position + new Vector2(0f, 3f),
                        new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, 1f)),
                        0, Helper.RandomFloat(500, 1000), 500,
                        false, true,
                        particleRect,
                        new Color(new Vector3(1f)*(0.5f + Helper.RandomFloat(0.5f))),
                        ParticleFunctions.Smoke,
                        underWater ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
                ParticleController.Instance.Add(Position + new Vector2(0f, 3f),
                    new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, 0.2f)),
                    0, underWater ? Helper.RandomFloat(500, 1000) : Helper.RandomFloat(50, 150), 50,
                    false, true,
                    particleRect,
                    underWater ? Color.White : Color.Orange,
                    ParticleFunctions.FadeInOut,
                    underWater ? 0.2f : 0.75f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            }
            else
            {
                _upAnim.Reset();
                _upHitAnim.Reset();
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Down))
            {
                Speed.Y += 0.05f;
                _downAnim.Play();
                _downHitAnim.Play();
                if (!underWater)
                    ParticleController.Instance.Add(Position + new Vector2(0f, -3f),
                        new Vector2(Helper.RandomFloat(-0.3f, 0.3f), Helper.RandomFloat(0f, -1f)),
                        0, Helper.RandomFloat(500, 1000), 500,
                        false, true,
                        particleRect,
                        new Color(new Vector3(1f)*(0.5f + Helper.RandomFloat(0.5f))),
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
            else
            {
                _downAnim.Reset();
                _downHitAnim.Reset();
            }



            if (input.CurrentKeyboardState.IsKeyDown(Keys.X))
            {
                if (projectileTime1 <= 0)
                {
                    projectileTime1 = projectileCoolDown1;


                    if (PowerUpLevel != 1)
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile) entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile) entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false; 
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f*faceDir,0f);
                            entity.Position = Position + new Vector2(faceDir*10, 0);
                            
                        });

                    if (PowerUpLevel == 1)
                    {
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile)entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile)entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f * faceDir, 0f);
                            entity.Position = Position + new Vector2(faceDir * 10, -2);

                        });
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile)entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile)entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f * faceDir, 0f);
                            entity.Position = Position + new Vector2(faceDir * 10, 2);

                        });
                    }

                    if (PowerUpLevel >= 2)
                    {
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile) entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile) entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f*faceDir, -0.5f);
                            entity.Position = Position + new Vector2(faceDir*10, 0);
                        });
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile)entity).Type = ProjectileType.Forward1;
                            ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                            ((Projectile)entity).Life = 1000;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 1f;
                            entity.Speed = new Vector2(6f * faceDir, 0.5f);
                            entity.Position = Position + new Vector2(faceDir * 10, 0);
                        });
                    }

                    

                }

                if (projectileTime2 <= 0)
                {
                    projectileTime2 = projectileCoolDown2;

                    if (PowerUpLevel >= 3)
                    {
                        ProjectileController.Instance.Spawn(entity =>
                        {
                            ((Projectile) entity).Type = ProjectileType.Bomb;
                            ((Projectile) entity).SourceRect = new Rectangle(16, 0, 8, 8);
                            ((Projectile) entity).Life = 5000;
                            ((Projectile) entity).Scale = 0.5f;
                            ((Projectile)entity).EnemyOwner = false;
                            ((Projectile)entity).Damage = 10f;
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
            if (collided is Enemy)
            {
                _hitAlpha = 1f;
                Life -= 0.5f;
            }

            if (collided is Projectile && ((Projectile) collided).EnemyOwner)
            {
                _hitAlpha = 1f;
                Life -= 1f;
            }

            if (collided is Powerup)
            {
                collided.Active = false;
                PowerUpMeter++;
            }

            base.OnBoxCollision(collided, intersect);
        }

        public override void OnPolyCollision(Entity collided)
        {

            base.OnPolyCollision(collided);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            if (!Active) return;

            if(_upAnim.State== SpriteAnimState.Playing) _upAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);
            else if(_downAnim.State== SpriteAnimState.Playing) _downAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);
            else _idleAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);

            if (_hitAlpha > 0f)
            {
                if (_upHitAnim.State == SpriteAnimState.Playing)
                    _upHitAnim.Draw(sb, Position, faceDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f, 0f, Color.White * _hitAlpha);
                else if (_downHitAnim.State == SpriteAnimState.Playing)
                    _downHitAnim.Draw(sb, Position, faceDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f, 0f, Color.White * _hitAlpha);
                else
                    _idleHitAnim.Draw(sb, Position, faceDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f, 0f, Color.White * _hitAlpha);
            }

            base.Draw(sb, gameMap);
        }

        public void Die()
        {
            Life = 0f;
            Active = false;

         
            for (float a = 0f; a <= MathHelper.TwoPi; a += 0.1f)
            {
                Vector2 loc = Helper.PointOnCircle(ref Position, 7, a);
                Vector2 speed = loc - Position;
                speed.Normalize();

                Color c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                ParticleController.Instance.Add(loc,
                    speed * Helper.RandomFloat(1f, 3f),
                    100, 3000, 1000,
                    true, true,
                    new Rectangle(0, 0, 2, 2),
                    c,
                    part =>
                    {
                        ParticleFunctions.FadeInOut(part);
                    },
                    1f, 0f, 0f,
                    1, ParticleBlend.Additive);

                loc = Helper.PointOnCircle(ref Position, 3, a);
                speed = loc - Position;
                speed.Normalize();

                c = new Color(new Vector3(1.0f, (float)Helper.Random.NextDouble(), 0.0f)) * (0.7f + ((float)Helper.Random.NextDouble() * 0.3f));
                ParticleController.Instance.Add(loc,
                    speed * Helper.RandomFloat(1f, 3f),
                    100, 3000, 1000,
                    true, true,
                    new Rectangle(0, 0, 2, 2),
                    c,
                    part =>
                    {
                        ParticleFunctions.FadeInOut(part);
                    },
                    1f, 0f, 0f,
                    1, ParticleBlend.Additive);
            }
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
