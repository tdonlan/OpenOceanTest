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
    public class FishingPole
    {
        public Level level;

        public FishingState state;

        public Vector2 hookPosition;

        public Texture2D hookTexture;
        public Texture2D lineTexture;

        public float tension;

        //throwing
        public Vector2 throwDirection;

        //catching
        private Fish fish;

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(hookPosition, 5);
            }
        }

        public FishingPole(Level level, Vector2 hookPosition)
        {
            this.level = level;
            this.state = FishingState.Retracted;
            this.hookPosition = hookPosition;
            this.tension = 0;


            LoadContent();
        }

        private void LoadContent()
        {
            hookTexture = level.game.textureLoader.WhitePixel;
            lineTexture = level.game.textureLoader.WhitePixel;
        }


        public void ThrowLine(Vector2 direction)
        {
            hookPosition = level.player.Position;
            throwDirection = direction;
            state = FishingState.Throwing;
        }



        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (state)
            {
                case FishingState.Retracted:
                        //do nothing till player throws it        
                    break;
                case FishingState.Throwing:

                    UpdateThrowing(gameTime);
                    
                    break;
                case FishingState.Waiting:
                    UpdateWaiting(gameTime);
                    //do nothing, bob?

                    break;
                case FishingState.Hooked:
                    UpdateHooked(gameTime);
                    break;

                case FishingState.Snapped:
                    break;
                case FishingState.Caught:
                    fish = null;
                    hookPosition = level.player.Position;
                    break;
                default:
                    break;

            }
        
        }

        private void UpdateThrowing(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Math.Abs(throwDirection.X) > 1 && Math.Abs(throwDirection.Y) > 0)
            {
                hookPosition += throwDirection * elapsed;
                throwDirection *= .97f;
            }
            else
            {
                state = FishingState.Waiting;
            }
        }

        //check if a fish will grab it
        //retract if you move too much?
        private void UpdateWaiting(GameTime gameTime)
        {
            for(int i=level.fish.Count-1;i >=0;i--)
            {
                Fish f = level.fish[i];
                if (f.BoundingCircle.Intersects(BoundingCircle))
                {
                    if (level.game.r.Next(100) > 90)
                    {
                        state = FishingState.Hooked;
                        fish = f;
                        level.fish.Remove(f);
                    }
                }
            }
        }

        private void UpdateHooked(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            fish.Position = hookPosition;

            if (level.player.BoundingCircle.Intersects(fish.BoundingCircle))
            {
                state = FishingState.Caught;
            }

            else if (tension >= 100)
            {
                state = FishingState.Snapped;
                fish = null;
            }
            else
            {
                // get player reel-in vector
                Vector2 reelDirection = level.game.gameInput.currentGamePadState.ThumbSticks.Right;

                Vector2 reelVector = reelDirection  * 30; //stronger than the fish

                //get fish swimming away vector
                Vector2 fishDirection = hookPosition - level.player.Position;
                fishDirection= Helper.Normalize(fishDirection);
                Vector2 fishVector = fishDirection *200;

                //calculate tension
                Vector2 forceVector = fishVector - reelVector ;

                if (forceVector.X > 10 || forceVector.Y > 10)
                {
                    tension++;
                }
                else if (forceVector.X < -10 || forceVector.X < -10)
                {
                    tension--;
                }

                //update new hook location
                hookPosition = hookPosition + forceVector * elapsed;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (state != FishingState.Retracted)
            {
                //draw the hook

                //draw a line from the player to the hook
             

                if (fish != null)
                {
                    DrawPrimitives.drawLine(level.player.Position, hookPosition, 2, lineTexture, Color.Red, spriteBatch);
                    fish.Draw(gameTime, spriteBatch);

                }
                else
                {
                    DrawPrimitives.drawLine(level.player.Position, hookPosition, 2, lineTexture, Color.Black, spriteBatch);
                }

            }
        }


    }
}
