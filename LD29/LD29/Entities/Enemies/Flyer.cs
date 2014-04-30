using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.Entities.Enemies
{
    class Flyer : Enemy
    {
        private Vector2 _target;

        private bool _firing = false;
        private double projectileCoolDown = 50;
        private double projectileTime;

        private SoundEffectInstance gunLoop;

        public Flyer(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 16, 16, 100, new Vector2(8, 8), false, false, 0);
            _idleAnim.Pause();
            _hitAnim = new SpriteAnim(spritesheet, 6, 1, 16, 16, 100, new Vector2(8, 8), false, false, _idleAnim.CurrentFrame);
            _hitAnim.Pause();

            _faceDir = Helper.Random.Next(2) == 0 ? -1 : 1;

			gunLoop = AudioController.CreateInstance("minigun");
			gunLoop.IsLooped = true;
			gunLoop.Volume = 0f;
			gunLoop.Play();
			gunLoop.Pause();
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            _target.X = Position.X + (_faceDir*200);

            //if (_target.X < 0) _target.X = (gameMap.Width * gameMap.TileWidth) - _target.X;
            //if (_target.X >= (gameMap.Width * gameMap.TileWidth)) _target.X = _target.X - (gameMap.Width * gameMap.TileWidth);

            if (Helper.Random.Next(100) == 0)
                _target.Y = Helper.RandomFloat(30f, (gameMap.TileHeight*gameMap.Height) -30f);

            //if (Helper.Random.Next(500) == 0)
            //    _faceDir = -_faceDir;

            for(int dist = 0;dist<=150;dist+=50)
                if (gameMap.CheckTileCollision(new Vector2(_target.X + (dist*-_faceDir), Position.Y)))
                {
                    _target.Y = Position.Y + (Position.Y < 260f ? -50f : 50f);
                    Speed.X = MathHelper.Lerp(Speed.X, 0f, 0.1f);
                }
            //if (gameMap.CheckTileCollision(_target + new Vector2(-_faceDir * 50, 0)))
            //{
            //    _target.Y += _target.Y < 260f ? -50f : 50f;
            //    Speed.X = MathHelper.Lerp(Speed.X, 0f, 0.1f);
            //}
            //if (gameMap.CheckTileCollision(_target + new Vector2(-_faceDir * 100, 0)))
            //{
            //    _target.Y += _target.Y < 260f ? -50f : 50f;
            //    Speed.X = MathHelper.Lerp(Speed.X, 0f, 0.1f);
            //}
            //if (gameMap.CheckTileCollision(_target + new Vector2(-_faceDir * 130, 0)))
            //{
            //    _target.Y += _target.Y < 260f ? -50f : 50f;
            //    Speed.X = MathHelper.Lerp(Speed.X, 0f, 0.1f);
            //}

            //if (Speed.Length() < 0.1f) _target = Position;

            //Rotation = Helper.V2ToAngle(Speed);

            Speed = Vector2.Lerp(Speed, Vector2.Zero, 0.05f);

            if (Vector2.Distance(_target, Position) > 5f)
            {
                Vector2 dir = _target - Position;
                dir.Normalize();
                Speed += dir*0.2f;
            }

            Speed.X = MathHelper.Clamp(Speed.X, -3f, 3f);
            Speed.Y = MathHelper.Clamp(Speed.Y, -3f, 3f);


            if (Position.Y < 260f)
                ParticleController.Instance.Add(Position + new Vector2(-_faceDir*5f, 0f),
                                            new Vector2(Helper.RandomFloat(0f, -_faceDir*1f), Helper.RandomFloat(-0.2f, 0.2f)),
                                            0, Helper.RandomFloat(500, 1000), 500,
                                            false, true,
                                            Position.Y < 260f ?new Rectangle(0,0,3,3):new Rectangle(128, 0, 16, 16), 
                                            new Color(new Vector3(1f) * (0.5f + Helper.RandomFloat(0.5f))),
                                            ParticleFunctions.Smoke,
                                            Position.Y >= 260f ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);
            ParticleController.Instance.Add(Position + new Vector2(-_faceDir * 5f, 0f),
                                        new Vector2(Helper.RandomFloat(0f, -_faceDir * 0.2f), Helper.RandomFloat(-0.2f, 0.2f)),
                                        0, Helper.RandomFloat(50, 150), 50,
                                        false, true,
                                        Position.Y < 260f ? new Rectangle(0, 0, 3, 3) : new Rectangle(128, 0, 16, 16),
                                        Position.Y < 260f ? Color.LightGreen : Color.White,
                                        ParticleFunctions.FadeInOut,
                                        Position.Y >= 260f ? 0.3f : 1f, 0f, Helper.RandomFloat(-0.1f, 0.1f), 0, ParticleBlend.Alpha);

            projectileTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_firing)
            {
                Vector2 screenPos = Vector2.Transform(Position, Camera.Instance.CameraMatrix);
                float pan = (screenPos.X - (Camera.Instance.Width / 2f)) / (Camera.Instance.Width / 2f);
                if (pan < -1f || pan > 1f) gunLoop.Volume = 0f;
                else gunLoop.Volume = 0.3f;
                if (Ship.Instance.Position.Y >= 260 && Position.Y < 260) gunLoop.Volume = 0f;
                if (Ship.Instance.Position.Y < 260 && Position.Y >= 260) gunLoop.Volume = 0f;
                gunLoop.Pan = MathHelper.Clamp(pan, -1f, 1f);

                if (projectileTime <= 0)
                {
                    projectileTime = projectileCoolDown;
                    ProjectileController.Instance.Spawn(entity =>
                    {
                        ((Projectile)entity).Type = ProjectileType.Forward1;
                        ((Projectile)entity).SourceRect = new Rectangle(8, 8, 20, 8);
                        ((Projectile)entity).Life = 1000;
                        ((Projectile)entity).EnemyOwner = true;
                        ((Projectile)entity).Damage = 1f;
                        entity.Speed = new Vector2(6f*_faceDir, 0f);
                        entity.Position = Position + entity.Speed;

                    });

                }

                if (Helper.Random.Next(20) == 0)
                {
                    _firing = false;
                    gunLoop.Pause();
                }
            }
            else
            {
                if (Helper.Random.Next(50) == 0)
                {
                    _firing = true;
                    gunLoop.Resume();
                }
            }

            if (Helper.Random.Next(200) == 0 && GameController.Wave >= 8)
            {
                AudioController.PlaySFX("seeker", 0.5f, -0.1f, 0.1f, Camera.Instance, Position);
                ProjectileController.Instance.Spawn(entity =>
                {
                    ((Projectile) entity).Type = ProjectileType.Seeker;
                    ((Projectile) entity).SourceRect = new Rectangle(1, 18, 8, 4);
                    ((Projectile) entity).Life = 2000;
                    ((Projectile) entity).Scale = 1f;
                    ((Projectile) entity).EnemyOwner = true;
                    ((Projectile) entity).Damage = 5f;
                    ((Projectile) entity).Target = Position + new Vector2(_faceDir*300, 0);
                    ;
                    entity.Speed = new Vector2(0f, -0.5f);
                    entity.Position = Position + new Vector2(0, 0);
                });
            }

            //if (Vector2.Distance(Position, _target) < 1f)
            //{
            //    _idleAnim.Pause();
            //    _hitAnim.Pause();

            //    if (Helper.Random.Next(100) == 0)
            //    {
            //        _target = Position + new Vector2(Helper.RandomFloat(-30, 30), Helper.RandomFloat(-30, 30));
            //        if (_target.Y < 280) _target.Y = 280;

            //        Vector2 dir = _target - Position;
            //        dir.Normalize();
            //        Speed = dir*5f;

            //        _idleAnim.Play();
            //        _hitAnim.Play();
            //    }

            //    if (Vector2.Distance(Ship.Instance.Position, Position) < 100f)
            //    {
            //        _target = Ship.Instance.Position;
            //        if (_target.Y < 280) _target.Y = 280;

            //        Vector2 dir = _target - Position;
            //        dir.Normalize();
            //        Speed = dir * 5f;

            //        _idleAnim.Play();
            //        _hitAnim.Play();
            //    }
            //}
            //else
            //{
            //    ParticleController.Instance.Add(Position,
            //                       new Vector2(Helper.RandomFloat(-0.1f,0.1f), Helper.RandomFloat(-0.1f,0.1f)),
            //                       0, Helper.Random.NextDouble() * 1000, Helper.Random.NextDouble() * 1000,
            //                       false, false,
            //                       new Rectangle(128, 0, 16, 16), 
            //                       new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
            //                       ParticleFunctions.FadeInOut,
            //                       0.5f, 0f, 0f,
            //                       1, ParticleBlend.Alpha);
            //}

            

            base.Update(gameTime, gameMap);
        }

        public override void Die()
        {
            gunLoop.Stop();
            base.Die();
        }
    }
}
