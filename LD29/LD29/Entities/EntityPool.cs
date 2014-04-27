using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using LD29.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.EntityPools
{
    public class EntityPool
    {
        public List<Entity> Entities;
        public List<object> BoxCollidesWith;
        public List<object> PolyCollidesWith;


        private int _maxEntities;

        public EntityPool(int maxEntities, Func<Texture2D, Entity> createFunc, Texture2D spriteSheet)
        {
            _maxEntities = maxEntities;

            Entities = new List<Entity>();
            for(int i=0;i<maxEntities;i++) Entities.Add(createFunc(spriteSheet));

            BoxCollidesWith = new List<object>();
            PolyCollidesWith = new List<object>();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Entity e in Entities.Where(ent => ent.Active))
            {
                e.Update(gameTime);
                CheckCollisions(e);
            }
        }
        public virtual void Update(GameTime gameTime, Map gameMap)
        {
            foreach (Entity e in Entities.Where(ent => ent.Active))
            {
                e.Update(gameTime, gameMap);
                CheckCollisions(e);
            }
        }

        public virtual void HandleInput(InputState input)
        {
            foreach (Entity e in Entities.Where(ent => ent.Active)) e.HandleInput(input);
        }

        public virtual void Draw(SpriteBatch sb, Camera camera, Map gameMap)
        {
            sb.Begin(SpriteSortMode.Deferred, null,SamplerState.PointClamp,null,null,null,camera.CameraMatrix);
            foreach (Entity e in Entities.Where(ent=>ent.Active)) e.Draw(sb, gameMap); 
            sb.End();
        }

        public Entity Spawn(Action<Entity> spawnFunc)
        {
            // Find an entity in our pool that isn't active
            Entity retEntity = Entities.FirstOrDefault(ent => !ent.Active);

            // If we don't have a spare entitiy, return null
            if (retEntity == null) return null;

            // First, run our reset function to reset required default values (might be speed or life etc.)
            retEntity.Reset();

            // Then, run the spawn function to set new values (position, etc)
            spawnFunc(retEntity);

            retEntity.HitBox.Location = new Point((int)retEntity.Position.X, (int)retEntity.Position.Y);

            // Make it alive!
            retEntity.Active = true;

            return retEntity;
        }

        internal void Wrap(float off)
        {
            foreach (Entity e in Entities)
                e.Position.X += off;
        }

        private void CheckCollisions(Entity e)
        {
            foreach (object o in BoxCollidesWith)
            {
                if (o is EntityPool)
                {
                    foreach (Entity collEnt in ((EntityPool)o).Entities)
                    {
                        if (!collEnt.Active) continue;
                        if (collEnt == e) continue;

                        Rectangle intersect = Rectangle.Intersect(e.HitBox, collEnt.HitBox);
                        if (intersect.IsEmpty) continue;

                        e.OnBoxCollision(collEnt, intersect);
                    }
                }

                if (o is Entity)
                {
                    Entity collEnt = (Entity)o;

                    if (!collEnt.Active) continue;
                    if (collEnt == e) continue;

                    Rectangle intersect = Rectangle.Intersect(e.HitBox, collEnt.HitBox);
                    if (intersect.IsEmpty) continue;

                    e.OnBoxCollision(collEnt, intersect);
                    collEnt.OnBoxCollision(e, intersect);
                }
            }

            foreach (object o in PolyCollidesWith)
            {
                if (o is EntityPool)
                {
                    foreach (Entity collEnt in ((EntityPool)o).Entities)
                    {
                        if (!collEnt.Active) continue;
                        if (collEnt == e) continue;

                        bool collides = false;
                        foreach (Vector2 vector2 in e.HitPolyPoints)
                            if (Helper.IsPointInShape(vector2, collEnt.HitPolyPoints)) collides = true;
                        if (!collides) continue;

                        e.OnPolyCollision(collEnt);
                    }
                }

                if (o is Entity)
                {
                    Entity collEnt = (Entity)o;

                    if (!collEnt.Active) continue;
                    if (collEnt == e) continue;

                    bool collides = false;
                    foreach (Vector2 vector2 in e.HitPolyPoints)
                        if (Helper.IsPointInShape(vector2, collEnt.HitPolyPoints)) collides = true;
                    if (!collides) continue;

                    e.OnPolyCollision(collEnt);
                    collEnt.OnPolyCollision(e);
                }
            }
        }
    }
}
