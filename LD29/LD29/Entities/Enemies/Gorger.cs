using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.Entities.Enemies
{
    class Gorger : Enemy
    {
        public Gorger(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 3, 6, 16, 16, 100, new Vector2(8, 8), false, false, 0);
            _idleAnim.Pause();
            _hitAnim = new SpriteAnim(spritesheet, 9, 6, 16, 16, 100, new Vector2(8, 8), false, false, _idleAnim.CurrentFrame);
            _hitAnim.Pause();

            Speed.Y = Helper.RandomFloat(-0.2f, 0.2f);
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            if (Speed.Y < 0f && Position.Y < 250) Speed.Y = -Speed.Y;

            if (Helper.Random.Next(50) == 0)
                _faceDir = Helper.Random.Next(2) == 0 ? -1 : 1;

            if (_idleAnim.State == SpriteAnimState.Paused)
            {
                

                _idleAnim.Reset();
                _hitAnim.Reset();
                _idleAnim.Play();
                _hitAnim.Play();
            }

            if (_idleAnim.CurrentFrame == 4 && (_idleAnim.TargetFrameTime - _idleAnim.CurrentFrameTime) <= 10)
            {
                if(Ship.Instance.Position.Y>260)
                    AudioController.PlaySFX("gorgershoot", 1f, -0.1f, 0.1f, Camera.Instance, Position);

                ProjectileController.Instance.Spawn(entity =>
                {
                    ((Projectile)entity).Type = ProjectileType.GorgerAcid;
                    ((Projectile)entity).SourceRect = new Rectangle(0, 0, 1, 1);
                    entity.HitBox = new Rectangle(0, 0, 16, 16);
                    ((Projectile)entity).Life = 10000;
                    ((Projectile)entity).EnemyOwner = true;
                    ((Projectile)entity).Damage = 0.5f;
                    entity.Speed = new Vector2(Helper.RandomFloat(1f, 4f) * _faceDir, 0f);
                    entity.Position = Position + new Vector2(_faceDir * 10, 0);
                });
            }

            if (_idleAnim.CurrentFrame == 1 && (_idleAnim.TargetFrameTime - _idleAnim.CurrentFrameTime) <= 10)
            {
                _idleAnim.CurrentFrameTime = 0;
                _idleAnim.CurrentFrame = 0;
            }

            if (Helper.Random.Next(100) == 0 && _idleAnim.CurrentFrame <=1)
            {
                _idleAnim.Reset();
                _hitAnim.Reset();
                _idleAnim.CurrentFrame = 2;
                _hitAnim.CurrentFrame = 2;
                _idleAnim.Play();
                _hitAnim.Play();
            }

            

            base.Update(gameTime, gameMap);
        }
    }
}
