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

        private SpriteAnim _shipAnim;

        public Ship(Texture2D spritesheet, Rectangle hitbox, List<Vector2> hitPolyPoints, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitPolyPoints, hitboxoffset)
        {
            _shipAnim = new SpriteAnim(spritesheet,0,4,16,16,50, new Vector2(8,8));

            Active = true;
        }

        public override void Update(GameTime gameTime)
        {
            _tint = Color.White;

            base.Update(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            Speed.X = MathHelper.Lerp(Speed.X, 0f, 0.01f);
            Speed.Y = MathHelper.Lerp(Speed.Y, 0f, 0.01f);

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                Speed.X-=0.05f;
                faceDir = -1;
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                Speed.X+=0.05f;
                faceDir = 1;
            }

            _shipAnim.CurrentFrame = 0;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Up))
            {
                Speed.Y -= 0.05f;
                _shipAnim.CurrentFrame = 1;
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Down))
            {
                Speed.Y += 0.05f;
                _shipAnim.CurrentFrame = 3;
            }

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
            _shipAnim.Draw(sb, Position, faceDir==-1?SpriteEffects.FlipHorizontally : SpriteEffects.None);
            base.Draw(sb);
        }
    }
}
