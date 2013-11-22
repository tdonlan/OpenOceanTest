using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace OceanTest
{
    public class Seaweed
    {
        public Level level;

        public Vector2 rootPosition;
        public List<Leaf> leaves = new List<Leaf>();

        public Texture2D texture;

        public int height;

        public Seaweed(Level lev, Vector2 position, int height)
        {
            this.level = lev;
            this.rootPosition = position;
            this.height = height;

            LoadContent();

        }

        public void LoadContent()
        {
            texture = level.game.textureLoader.SeaWeed;

            leaves.Add(new Leaf(level, rootPosition,getRandomRotation()));

            for (int i = 1; i < height; i++)
            {
                leaves.Add(new Leaf(level, leaves[i-1].AnchorPosition, getRandomRotation()));
            }
        }

        private float getRandomRotation()
        {
            return level.game.r.Next(-1, 2) * (float)level.game.r.NextDouble();
        }


        public void Update(GameTime gameTime)
        {



            for (int i = 0; i < leaves.Count; i++)
            {
                if (i == 0)
                {
                    leaves[i].Update(gameTime, rootPosition);
                }
                else
                {
                    leaves[i].Update(gameTime, leaves[i - 1].AnchorPosition);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for(int i =leaves.Count-1;i>=0;i--)
                leaves[i].Draw(gameTime, spriteBatch);
        }
    }

    public class Leaf
    {
        public  Level lev;
        public Vector2 position;
       

        public Vector2 origin;
        public float rotation;

        private float newRotation;
        private TimeSpan leafTimer;
        private float leafTime;

        public Texture2D texture;
        public float offset;

        //returns the position child leaves are anchored
        public Vector2 AnchorPosition
        {
            get
            {
                return position + new Vector2((float)Math.Sin(rotation) * offset, (float)Math.Cos(rotation) * -offset);
            }
        }

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(position, texture.Height);
            }
        }

        public Leaf(Level lev, Vector2 position, float rotation)
        {
            this.lev = lev;
            this.position = position;
            this.rotation = rotation;

            this.leafTime = lev.game.r.Next(1,5);
            leafTimer = TimeSpan.FromSeconds(leafTime);

            LoadContent();
        }

        private void LoadContent()
        {
            texture = lev.game.textureLoader.SeaWeed;
            origin = new Vector2(texture.Width / 2, texture.Height);
            offset = texture.Height / 2;
        }

        public void Update( GameTime gameTime, Vector2 position)
        {
            this.position = position;

      

            leafTimer -= gameTime.ElapsedGameTime;
            if(leafTimer < TimeSpan.Zero)
            {
                leafTimer = TimeSpan.FromSeconds(leafTime);
                newRotation = lev.game.r.Next(-1,2) * (float)lev.game.r.NextDouble();
            }

            //if the player swims by, move with it
            if(lev.player.BoundingCircle.Intersects(BoundingCircle))
            {
                if (lev.player.Velocity.X < 0)
                {
                    newRotation = -1f;
                }
                else
                {
                    newRotation = 1f;
                }
            }

            rotation = MathHelper.Lerp(rotation,newRotation,.01f);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

    }
}
