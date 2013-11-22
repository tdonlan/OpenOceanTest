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
    public class FlockFish
    {
        public Level level;

        public Texture2D texture;

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public SpriteEffects flip;

        public TimeSpan DirectionChangeTimer;
        public float DirectionChangeTime;

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, texture.Width);
            }
        }

        public FlockFish(Level level, Vector2 Position)
        {
            this.level = level;
            this.Position = Position;

            LoadContent();

        }

        private void LoadContent()
        {
            texture = level.game.textureLoader.SmallFish;
        }

        public void Update(GameTime gameTime)
        {

            if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else
                flip = SpriteEffects.None;


            Vector2 avgPos = Position;
            Vector2 alignVector = new Vector2(0, 0);
            Vector2 seperateVector = new Vector2(0, 0);
            Vector2 cohesionVector = new Vector2(0, 0);

            foreach (FlockFish b in level.flockFish)
            {
                if (b != this)
                {
                    avgPos += b.Position;

                    float distance = DrawPrimitives.getDistance(b.Position, Position);
                    if (distance < 30)
                    {
                        seperateVector += (Position - b.Position);
                    }
                    else if(distance < 200)
                    {
                        alignVector += b.Velocity;
                    }
                }
            }

            alignVector /= level.flockFish.Count;

            avgPos /= level.flockFish.Count;

         
            cohesionVector = avgPos - Position;
            

            alignVector = Normalize(alignVector);
            cohesionVector = Normalize(cohesionVector);
            seperateVector = Normalize(seperateVector);

            Acceleration = (cohesionVector * GameConstants.cohesionWeight + alignVector * GameConstants.alignWeight + seperateVector * GameConstants.seperateWeight);

            //if we get near the "wall", swim away
            Acceleration = checkBounds(Acceleration);

          

            //move away from player
            if (level.player.BoundingCircle.Intersects(BoundingCircle))
            {
                Vector2 dir = Position - level.player.Position;
                if (dir.X != 0 && dir.Y != 0)
                    dir.Normalize();
                Acceleration = dir * GameConstants.FishSpeed;
            }

            Acceleration = Normalize(Acceleration);

            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            Vector2 NewPosition = Position +  Velocity * GameConstants.FlockFishSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (NewPosition.Y < 50)
                NewPosition.Y = 50;

            //NewPosition = level.oceanFloor.GetCollision(NewPosition);

            Position = NewPosition;

            Velocity *= .95f;
            
        }

        private Vector2 checkBounds(Vector2 Acceleration)
        {
            if (Position.X < -4000 || Position.X > 4000 || Position.Y > 2000 || Position.Y < 100)
            {
                Acceleration = new Vector2(0, 500) - Position;
           
            }
            return Acceleration;
        }

       private Vector2 Normalize(Vector2 vec)
        {
            if (vec.X != 0 && vec.Y != 0)
            {
                vec.Normalize();
            }
            return vec;
        }
        

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            

            spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(0, 0), 1, flip, 0);
        }

    }
}
