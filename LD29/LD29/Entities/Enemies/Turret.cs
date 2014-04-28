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
    class Turret : Enemy
    {
        private Vector2 _target;

        private bool _firing;
        private float _turretRot = 0f;
        private float _turretRotTarget = 0f;

        private double projectileCoolDown = 50;
        private double projectileTime;

        private SoundEffectInstance gunLoop;


        public Turret(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 4, 2, 16, 16, 200, new Vector2(8, 8), true, false, 0);
            _idleAnim.Play();
            _hitAnim = new SpriteAnim(spritesheet, 10, 2, 16, 16, 200, new Vector2(8, 8), true, false, _idleAnim.CurrentFrame);
            _hitAnim.Play();

            gunLoop = new SoundEffectInstance(AudioController._effects["minigun"]);
            gunLoop.IsLooped = true;
            gunLoop.Play();
            gunLoop.Pause();

            Speed.X = Helper.Random.Next(2) == 0 ? -0.1f : 0.1f;
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            projectileTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (!gameMap.CheckTileCollision(Position + new Vector2(_faceDir*8, 10))) Speed.X = -Speed.X;

            if (Helper.Random.Next(20) == 0)
                _turretRotTarget = Helper.RandomFloat(MathHelper.PiOver2);

            _turretRot = MathHelper.Lerp(_turretRot, _turretRotTarget, 0.01f);

            if (_firing)
            {
                Vector2 screenPos = Vector2.Transform(Position, Camera.Instance.CameraMatrix);
                float pan = (screenPos.X - (Camera.Instance.Width / 2f)) / (Camera.Instance.Width / 2f);
                if (pan < -1f || pan > 1f) gunLoop.Volume = 0f;
                else gunLoop.Volume = 0.3f;
                if (Ship.Instance.Position.Y >= 260) gunLoop.Volume = 0f;
                gunLoop.Pan = pan;

                if (projectileTime <= 0)
                {
                    projectileTime = projectileCoolDown;
                    ProjectileController.Instance.Spawn(entity =>
                    {
                        ((Projectile) entity).Type = ProjectileType.Forward1;
                        ((Projectile) entity).SourceRect = new Rectangle(8, 8, 20, 8);
                        ((Projectile) entity).Life = 1000;
                        ((Projectile) entity).EnemyOwner = true;
                        ((Projectile) entity).Damage = 1f;
                        entity.Speed = Helper.AngleToVector(_turretRot, -5f);
                        entity.Speed.X *= -_faceDir;
                        entity.Position = Position + entity.Speed;

                    });

                }

                if (Helper.Random.Next(20) == 0)
                {
                    gunLoop.Pause();
                    _firing = false;
                }
            }
            else
            {
                if (Helper.Random.Next(50) == 0)
                {
                    gunLoop.Resume();
                    _firing = true;
                }
            }

            base.Update(gameTime, gameMap);
        }

        public override void Die()
        {
            gunLoop.Stop();
            base.Die();
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            base.Draw(sb, gameMap);

            sb.Draw(SpriteSheet, Position+new Vector2(0,4), new Rectangle(32, 64, 16, 16), Color.White,
                _turretRot*-_faceDir, new Vector2(8, 9), _scale, _faceDir == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            if(_hitAlpha>0f)
                sb.Draw(SpriteSheet, Position + new Vector2(0, 4), new Rectangle(32, 160, 16, 16), Color.White * _hitAlpha,
                    _turretRot * -_faceDir, new Vector2(8, 8), _scale, _faceDir == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
