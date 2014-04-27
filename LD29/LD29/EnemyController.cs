﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using LD29.Entities;
using LD29.Entities.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace LD29.EntityPools
{
    public class EnemyController
    {
        public static EnemyController Instance;

        public List<Enemy> Enemies;
        public List<object> BoxCollidesWith;
        public List<object> PolyCollidesWith;

        private double _spawnTime = 0;
        private double _spawnInterval = 5000;
        private int numToSpawn = 0;

        private Texture2D _spriteSheet;

        public EnemyController(Texture2D spriteSheet)
        {
            Instance = this;

            _spriteSheet = spriteSheet;

            Enemies = new List<Enemy>();
            BoxCollidesWith = new List<object>();
            PolyCollidesWith = new List<object>();
        }

        public void Update(GameTime gameTime, Map gameMap)
        {
            if (numToSpawn > 0)
            {
                _spawnTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_spawnTime >= _spawnInterval)
                {
                    _spawnTime = 0;
                 
                    SpawnRandom(gameMap);
                    numToSpawn--;
                }
            }

            if (Enemies.Count == 0 && numToSpawn > 0)
            {
                SpawnRandom(gameMap);
                numToSpawn--;
            }

            foreach (Enemy e in Enemies.Where(ent => ent.Active))
            {
                e.Update(gameTime, gameMap);
                CheckCollisions(e);
            }

            Enemies.RemoveAll(en => !en.Active);
        }

     
        public void Draw(SpriteBatch sb, Camera camera, Map gameMap)
        {
            sb.Begin(SpriteSortMode.Deferred, null,SamplerState.PointClamp,null,null,null,camera.CameraMatrix);
            foreach (Enemy e in Enemies.Where(en=>en.Active)) e.Draw(sb, gameMap); 
            sb.End();
        }

        public void SpawnInitial(int level, Map gameMap)
        {
            numToSpawn = (int)Math.Pow(level + 5, 1.2);

            for (int i = 0; i < numToSpawn; i++)
            {
                SpawnRandom(gameMap);
            }

            numToSpawn = (int)Math.Pow(level + 10, 1.2);
        }

        public void SpawnRandom(Map gameMap)
        {
            bool underWater = Helper.Random.Next(2) == 0;
            int enemyNum = Helper.Random.Next(2);

            Vector2 spawnLoc = FindSpawnLoc(gameMap, underWater);

            if (underWater)
            {
                switch (enemyNum)
                {
                    case 0:
                        Gorger gor = new Gorger(_spriteSheet, new Rectangle(0, 0, 10, 10), null, Vector2.Zero);
                        gor.Life = 20f;
                        gor.Spawn(spawnLoc);
                        Enemies.Add(gor);
                        break;
                    case 1:
                        Lunger lun = new Lunger(_spriteSheet, new Rectangle(0, 0, 10, 10), null, Vector2.Zero);
                        lun.Life = 10f;
                        lun.Spawn(spawnLoc);
                        Enemies.Add(lun);
                        break;
                }
            }
            else
            {
                switch (enemyNum)
                {
                    case 0:
                    case 1:
                        ManOWar mow = new ManOWar(_spriteSheet, new Rectangle(0, 0, 10, 10), null, Vector2.Zero);
                        mow.Life = 20f;
                        mow.Spawn(spawnLoc);
                        Enemies.Add(mow);
                        break;
                }
            }
        }

        private Vector2 FindSpawnLoc(Map gameMap, bool underWater)
        {
            Vector2 returnLoc = new Vector2();

            while (true)
            {
                if (!underWater) returnLoc.Y = Helper.RandomFloat(64, 200);
                else returnLoc.Y = Helper.RandomFloat(300, (gameMap.TileHeight*gameMap.Height) - 64);

                returnLoc.X = Helper.RandomFloat(64, (gameMap.TileWidth*gameMap.Width) - 64);

                bool coll = false;
                for(float a = 0;a<MathHelper.TwoPi;a+=0.1f)
                    if (gameMap.CheckCollision(Helper.PointOnCircle(ref returnLoc, 16, a))!=null) coll = true;

                if (!coll) break;
            }

            return returnLoc;
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
