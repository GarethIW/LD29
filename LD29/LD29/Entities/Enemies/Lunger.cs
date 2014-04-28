using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.Entities.Enemies
{
    class Lunger : Enemy
    {
        private Vector2 _target;

        public Lunger(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset)
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 5, 6, 16, 16, 100, new Vector2(8, 8), false, false, 0);
            _idleAnim.Pause();
            _hitAnim = new SpriteAnim(spritesheet, 11, 6, 16, 16, 100, new Vector2(8, 8), false, false, _idleAnim.CurrentFrame);
            _hitAnim.Pause();

            
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            _faceDir = 1;
            if (Speed.Y < 0f && Position.Y < 270) Speed.Y = -Speed.Y;

            if (Speed.Length() < 0.1f) _target = Position;

            Rotation = Helper.V2ToAngle(Speed);

            Speed = Vector2.Lerp(Speed, Vector2.Zero, 0.05f);

            if (Vector2.Distance(Position, _target) < 1f)
            {
                _idleAnim.Pause();
                _hitAnim.Pause();

                if (Helper.Random.Next(100) == 0)
                {
                    if (Ship.Instance.Position.Y > 260)
                        AudioController.PlaySFX("lancer", 1f, -0.1f, 0.1f, Camera.Instance, Position);

                    _target = Position + new Vector2(Helper.RandomFloat(-30, 30), Helper.RandomFloat(-30, 30));
                    if (_target.Y < 280) _target.Y = 280;

                    Vector2 dir = _target - Position;
                    dir.Normalize();
                    Speed = dir*5f;

                    _idleAnim.Play();
                    _hitAnim.Play();
                }

                if (Vector2.Distance(Ship.Instance.Position, Position) < 100f)
                {
                    if (Ship.Instance.Position.Y > 260)
                        AudioController.PlaySFX("lancer", 1f, -0.1f, 0.1f, Camera.Instance, Position);

                    _target = Ship.Instance.Position;
                    if (_target.Y < 280) _target.Y = 280;

                    Vector2 dir = _target - Position;
                    dir.Normalize();
                    Speed = dir * 5f;

                    _idleAnim.Play();
                    _hitAnim.Play();
                }
            }
            else
            {
                ParticleController.Instance.Add(Position,
                                   new Vector2(Helper.RandomFloat(-0.1f,0.1f), Helper.RandomFloat(-0.1f,0.1f)),
                                   0, Helper.Random.NextDouble() * 1000, Helper.Random.NextDouble() * 1000,
                                   false, false,
                                   new Rectangle(128, 0, 16, 16), 
                                   new Color(new Vector3(1f) * (0.25f + Helper.RandomFloat(0.5f))),
                                   ParticleFunctions.FadeInOut,
                                   0.5f, 0f, 0f,
                                   1, ParticleBlend.Alpha);
            }

            

            base.Update(gameTime, gameMap);
        }
    }
}
