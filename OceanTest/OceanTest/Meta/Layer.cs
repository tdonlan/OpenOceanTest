using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace OceanTest
{
    class Layer
    {
        public Level level;

        public Texture2D bgTexture;

        Color c;
      

        public Layer(Level level)
        {

            this.level = level;

            bgTexture = level.game.textureLoader.BGWater;


            
        }

        public void Draw(SpriteBatch spriteBatch, float cameraPosition, float cameraPositionY)
        {

            spriteBatch.Draw(bgTexture, new Vector2(cameraPosition, cameraPositionY), Color.White);
        }
        
        /*
        //we can only scroll this the margin of the texture > screen size
        private void DrawOne(SpriteBatch spriteBatch, float cameraPosition, float cameraPositionY)
        {
            int segmentWidth = Textures[0].Width;
            int segmentHeight = Textures[0].Height;

            int segmentWidthDiff = segmentWidth - 1280;
            int segmentHeightDiff = segmentHeight - 720;

            //hardcode for now

            float ratioX = cameraPosition / (float)tileWidth;
            float ratioY = cameraPositionY / (float)tileHeight;

            //spriteBatch.Draw(Textures[0], new Vector2(0, 0), Color.White);
            spriteBatch.Draw(Textures[0], new Vector2(ratioX * -segmentWidthDiff, ratioY * -segmentHeightDiff), Color.White);
        }

        private void DrawFour(SpriteBatch spriteBatch, float cameraPosition, float cameraPositionY)
        {
            //Assume all textures the same size
            int segmentWidth = Textures[0].Width;
            int segmentHeight = Textures[0].Height;

            float ratioX = cameraPosition / (float)tileWidth;
            float ratioY = cameraPositionY / (float)tileHeight;

            spriteBatch.Draw(Textures[0], new Vector2(ratioX * -segmentWidth, ratioY * -segmentHeight), Color.White);
            spriteBatch.Draw(Textures[1], new Vector2(ratioX * -segmentWidth + segmentWidth, ratioY * -segmentHeight), Color.White);
            spriteBatch.Draw(Textures[2], new Vector2(ratioX * -segmentWidth, ratioY * -segmentHeight + segmentHeight), Color.White);
            spriteBatch.Draw(Textures[3], new Vector2(ratioX * -segmentWidth + segmentWidth, ratioY * -segmentHeight + segmentHeight), Color.White);

        }
         * */

    }
}
