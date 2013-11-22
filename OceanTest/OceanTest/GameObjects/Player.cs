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
    public class Player
    {
        public Level level;

        public Texture2D texture;
        private Vector2 origin;

        private float propRotation;
        private float propSpeed;
        public Texture2D propTexture;
        public Vector2 propOrigin;

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public float rotation;

        public SpriteEffects flip;

        public bool inWater = true;

        //fishing pole
        FishingPole fishingPole;

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, texture.Width);
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)(Position.X - origin.X), (int)(Position.Y - origin.Y), (int)origin.X, (int)origin.Y);
            }
        }

        public Vector2 PropPosition
        {
            get
            {
                if (flip == SpriteEffects.FlipHorizontally)
                {
                    return Position + new Vector2((float)Math.Cos(rotation) * 65,(float)Math.Sin(rotation) * 65);
                }
                else
                {
                    return Position + new Vector2((float)Math.Cos(rotation) * -65, (float)Math.Sin(rotation) * -65);
                }
            }

        }

        public Player(Level level, Vector2 Position)
        {
            this.level = level;
            this.Position = Position;

            fishingPole = new FishingPole(level, Position);
         

            LoadContent();

        }

        private void LoadContent()
        {
            texture = level.game.textureLoader.Sub;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            propTexture = level.game.textureLoader.Prop;
            propOrigin = new Vector2(propTexture.Width / 2, propTexture.Height / 2);

          
                
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            fishingPole.Update(gameTime);

            GetInput(gameTime);
            if (inWater)
            {
                UpdatePosition(elapsed);
            }
            else
            {
                UpdatePositionAir(elapsed);
            }

            UpdateRotation(gameTime);
            UpdateProp(gameTime);
        }

        private void GetInput(GameTime gameTime)
        {

            //movement
            Acceleration = new Vector2(level.game.gameInput.HMovement * GameConstants.PlayerSpeed, level.game.gameInput.VMovement * GameConstants.PlayerSpeed);

            if (level.game.gameInput.IsNewKeyPress(Keys.Space))
            {
                fishingPole.ThrowLine(Velocity * 3);
            }

        }

        private void UpdatePosition(float elapsed)
        {
            Vector2 previousPosition = this.Position;

            Velocity += Acceleration * elapsed;
            
            Vector2 NewPosition;

             NewPosition.X = Position.X + (Velocity.X * elapsed);
             NewPosition.Y = Position.Y + (Velocity.Y * elapsed);
            //Vector2 NewPosition = Position + (Velocity * elapsed);

            // NewPosition = level.oceanFloor.GetCollision(NewPosition);
                
             Position = NewPosition;

             Collision.HandleCollisions(level, BoundingRectangle, ref Position);

             // If the collision stopped us from moving, reset the velocity to zero.
             if (Position.X == previousPosition.X)
                 Velocity.X = 0;

             if (Position.Y == previousPosition.Y)
                 Velocity.Y = 0;
           
            
            Velocity *= 0.95f;



            if (Position.Y < 0)
            {
                inWater = false;
            }

        }

        private void UpdateRotation(GameTime gameTime)
        {

            if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else if(Velocity.X > 0)
                flip = SpriteEffects.None;


           float newRotation;

           if (Math.Abs(Acceleration.X) < 0.1f && Math.Abs(Acceleration.Y) < 0.1f && Position.Y > 0)
           {
               if (rotation < Math.PI * -1)
               {
                   newRotation = (float)Math.PI * -2;
               }
               else
               {
                   newRotation = 0;
               }
                   rotation = MathHelper.Lerp(rotation,newRotation, .05f);
           }
           else
           {
               newRotation = (float)Math.Atan2(Velocity.Y, Velocity.X);
               if (flip == SpriteEffects.FlipHorizontally)
                   newRotation -= (float)Math.PI;

               rotation = newRotation;
           
           }
        }

        private void UpdateProp(GameTime gameTime)
        {
            //update speed
            propSpeed = MathHelper.Clamp(Math.Abs(Acceleration.X) + Math.Abs(Acceleration.Y), 1, 15);

            //update rotation
            propRotation += propSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

        private void UpdatePositionAir(float elapsed)
        {
            Velocity += new Vector2(0,GameConstants.Gravity) * elapsed;


            Position += Velocity * elapsed;

            if (Position.Y > 0)
            {
                inWater = true;
            }
        }


  

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            fishingPole.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, 1, flip, 0);

            spriteBatch.Draw(propTexture, PropPosition, null, Color.White, propRotation, propOrigin, 1, SpriteEffects.None, 0);
            DrawPrimitives.DrawRectangle(BoundingRectangle, level.game.textureLoader.WhitePixel, Color.Pink, spriteBatch, false, 2);

        }

    }
}
