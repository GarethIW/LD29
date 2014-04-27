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

        protected Color _tint;

        public Enemy(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 16, 16, 100, new Vector2(8, 8));
            _idleAnim.Play();
           
            Active = true;
        }

        public void Spawn(Vector2 spawnPos)
        {
            Position = spawnPos;

            TweenController.Instance.Create("", TweenFuncs.QuadraticEaseIn, tween =>
            {
                _scale.X = 5f - (4f * tween.Value);
                _scale.Y = tween.Value;
            }, 1000, false, false);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            _idleAnim.Draw(sb,Position,SpriteEffects.None,_scale,0f,_tint);
            if (Position.X >= 0 && Position.X < 200) _idleAnim.Draw(sb, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), SpriteEffects.None, _scale, 0f, _tint);
            if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) _idleAnim.Draw(sb, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), SpriteEffects.None, _scale, 0f, _tint); 
            base.Draw(sb, gameMap);
        }
    }
}
