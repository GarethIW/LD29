using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using TimersAndTweens;

namespace LD29.Entities
{
    public class Enemy : Entity
    {
        protected SpriteAnim _idleAnim;
        protected Vector2 _scale;

        protected Color _tint = Color.White;

        protected int _faceDir = -1;

        public float Life;

        public Enemy(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            Active = true;
        }

        public void Spawn(Vector2 spawnPos)
        {
            Position = spawnPos;

            TweenController.Instance.Create("", TweenFuncs.QuadraticEaseIn, tween =>
            {
                _scale.X = 8f - (7f * tween.Value);
                _scale.Y = tween.Value;
                if (tween.State == TweenState.Finished) _scale = Vector2.One;
            }, 1000, false, false);
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            _idleAnim.Update(gameTime);

            if (Speed.X <= 0f) _faceDir = -1;
            else _faceDir = 1;

            CheckMapCollisions(gameMap);

            base.Update(gameTime, gameMap);
        }

        public override void OnBoxCollision(Entity collided, Rectangle intersect)
        {
            if (collided is Projectile)
            {
                collided.Active = false;

                if (!((Projectile) collided).EnemyOwner)
                {
                    TweenController.Instance.Create("", TweenFuncs.Linear, tween =>
                    {
                        _tint = new Color(1f,1f-tween.Value,1f-tween.Value);
                        if (tween.State == TweenState.Finished) _tint = Color.White;
                    }, 100, false, false);
                }
            }

            base.OnBoxCollision(collided, intersect);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            _idleAnim.Draw(sb,Position, _faceDir==-1?SpriteEffects.None:SpriteEffects.FlipHorizontally,_scale,0f,_tint);
            if (Position.X >= 0 && Position.X < 200) _idleAnim.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, 0f, _tint);
            if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _idleAnim.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), _faceDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, _scale, 0f, _tint); 
            base.Draw(sb, gameMap);
        }

        public virtual void CheckMapCollisions(Map gameMap)
        {
            // Check upward collision
            if (Speed.Y < 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Top - Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = -Speed.Y;
                }

            // Check downward collision
            if (Speed.Y > 0)
                for (int x = HitBox.Left + 2; x <= HitBox.Right - 2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Bottom + Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = -Speed.Y;
                }

            // Check left collision
            if (Speed.X < 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Left - Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = -Speed.X;
                    }
                }

            // Check right collision
            if (Speed.X > 0)
                for (int y = HitBox.Top + 2; y <= HitBox.Bottom - 2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Right + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = -Speed.X;
                    }
                }
        }
    }
}
