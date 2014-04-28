using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.Entities
{
    public class Powerup:Entity
    {
        public Powerup(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            Speed = Vector2.Lerp(Speed, Vector2.Zero, 0.05f);

            if (Vector2.Distance(Ship.Instance.Position, Position) < 50f)
            {
                Vector2 dir = Ship.Instance.Position - Position;
                dir.Normalize();
                Speed += dir*0.25f;
            }
                //Position = Vector2.Lerp(Position, Ship.Instance.Position, 0.1f);

            if (gameMap.CheckCollision(Position + new Vector2(Speed.X, 0)).GetValueOrDefault())
            {
                Speed.X = -(Speed.X * (0.1f + Helper.RandomFloat(0.4f)));
                Speed.Y *= 0.9f;
            }

            if (gameMap.CheckCollision(Position + new Vector2(0, Speed.Y)).GetValueOrDefault())
            {
                Speed.Y = -(Speed.Y * (0.1f + Helper.RandomFloat(0.4f)));
                Speed.X *= 0.9f;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb, Map gameMap)
        {
            sb.Draw(SpriteSheet, Position, null, Color.White, 0f, new Vector2(3,3), 1f, SpriteEffects.None, 0);
            if (Position.X >= 0 && Position.X < 200) sb.Draw(SpriteSheet, Position + new Vector2(gameMap.Width * gameMap.TileWidth, 0), null, Color.White, Rotation, new Vector2(3,3), 1f, SpriteEffects.None, 0);
            if (Position.X >= (gameMap.Width * gameMap.TileWidth) - 200 && Position.X < (gameMap.Width * gameMap.TileWidth)) sb.Draw(SpriteSheet, Position - new Vector2(gameMap.Width * gameMap.TileWidth, 0), null, Color.White, Rotation, new Vector2(3, 3), 1f, SpriteEffects.None, 0);
            
            base.Draw(sb, gameMap);
        }

        public override void Reset()
        {
            
            base.Reset();
        }
    }

    
}
