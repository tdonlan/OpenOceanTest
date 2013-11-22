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
    public class Fish
    {
        public Level level;

        public FishType type;

        public Texture2D texture;

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public SpriteEffects flip;

        public float speed;

        public TimeSpan DirectionChangeTimer;
        public float DirectionChangeTime;

        public Circle BoundingCircle

        {
            get
            {
                return new Circle(Position, texture.Width);
            }
        }

        public Fish(Level level, Vector2 Position, FishType type)
        {
            this.level = level;
            this.Position = Position;
            this.type = type;

            LoadContent();

            
            DirectionChangeTimer = TimeSpan.FromSeconds(DirectionChangeTime);
            Acceleration = GetRandomDirection();

            

        }

        private void LoadContent()
        {
            switch (type)
            {
                case FishType.Normal:
                    texture = level.game.textureLoader.Fish;
                    speed = GameConstants.FishSpeed;
                    DirectionChangeTime = level.game.r.Next(1, 10);
                    break;
                case FishType.Shark:
                    texture = level.game.textureLoader.Shark;
                    speed = GameConstants.SharkSpeed;
                    DirectionChangeTime = level.game.r.Next(10, 20);
                    break;
                default:
                    texture = level.game.textureLoader.Fish;
                    speed = GameConstants.FishSpeed;
                    DirectionChangeTime = level.game.r.Next(1, 10);
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //change direction
            DirectionChangeTimer -= gameTime.ElapsedGameTime;
            if (DirectionChangeTimer <= TimeSpan.Zero)
            {
                DirectionChangeTimer = TimeSpan.FromSeconds(DirectionChangeTime);
                Acceleration = GetRandomDirection();
            }

            //if we get near the "wall", swim away
            Acceleration = checkBounds(Acceleration);

            //move away from player
            if (level.player.BoundingCircle.Intersects(BoundingCircle))
            {
                Vector2 dir = Position - level.player.Position;
                if (dir.X != 0 && dir.Y != 0)
                    dir.Normalize();
                Acceleration = dir * speed;
            }

            UpdatePosition(elapsed);
        }

        private Vector2 GetRandomDirection()
        {
            return new Vector2((float)level.game.r.Next(-1, 2) * (float)level.game.r.NextDouble() * speed,
                (float)level.game.r.Next(-1, 2) * (float)level.game.r.NextDouble() * speed / 4);
        }


        private Vector2 Normalize(Vector2 vec)
        {
            if (vec.X != 0 && vec.Y != 0)
            {
                vec.Normalize();
            }
            return vec;
        }

        private void UpdatePosition(float elapsed)
        {

            Velocity += Acceleration * elapsed;

            if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else
                flip = SpriteEffects.None;

            Vector2 NewPosition;

             NewPosition.X = Position.X + (Velocity.X * elapsed);
             NewPosition.Y = Position.Y + (Velocity.Y * elapsed);
            //Vector2 NewPosition = Position + (Velocity * elapsed);

            if (NewPosition.Y < 50)
                NewPosition.Y = 50;


            //NewPosition = level.oceanFloor.GetCollision(NewPosition);


            Position.X = NewPosition.X;
            Position.Y = NewPosition.Y;

            Velocity *= 0.95f;

        }

        private Vector2 checkBounds(Vector2 Acceleration)
        {
            if (type == FishType.Normal)
            {
                if (Position.X < -4000 || Position.X > 4000 || Position.Y > 3000 || Position.Y < 100)
                {
                    Vector2 dir = new Vector2(0, 1000) - Position;
                    dir = Normalize(dir);
                    Acceleration = dir * speed;
                    

                }
            }
            else
            {
                if (Position.X < -4000 || Position.X > 4000 || Position.Y > 4500 || Position.Y < 1000)
                {
                    Vector2 dir = new Vector2(0, 3000) - Position;
                    dir = Normalize(dir);
                    Acceleration = dir * speed;

                }
            }
            return Acceleration;
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White,0,new Vector2(0,0),1,flip,0);
        }

    }
}
