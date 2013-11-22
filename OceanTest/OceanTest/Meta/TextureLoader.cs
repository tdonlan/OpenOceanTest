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
    public class TextureLoader
    {

        public Texture2D WhitePixel;
        public Texture2D Sub;
        public Texture2D Prop;
        public Texture2D Fish;
        public Texture2D SmallFish;
        public Texture2D Shark;

        public Texture2D Wave;

        public Texture2D wallTexture;
        public Texture2D floorTexture;

        

        public Texture2D SeaWeed;

        public List<Texture2D> Rocks = new List<Texture2D>();

        public Texture2D BGWater;
        public Texture2D BGSky;

        public SpriteFont font;

        private ContentManager content;

        public TextureLoader(ContentManager content)
        {
            this.content = content;
            LoadTextures();
        }

        private void LoadTextures()
        {
            WhitePixel = content.Load<Texture2D>("Test/WhitePixel");
            Sub = content.Load<Texture2D>("Sprite/Sub1");
            Prop = content.Load<Texture2D>("Sprite/Prop");
            Fish = content.Load<Texture2D>("Sprite/Fish1");
            SmallFish = content.Load<Texture2D>("Sprite/SmallFish");
            Shark = content.Load<Texture2D>("Sprite/Shark1");

            SeaWeed = content.Load<Texture2D>("Sprite/Seaweed");

            Wave = content.Load<Texture2D>("Sprite/Wave");

            for (int i = 1; i <= 4; i++)
            {
                Rocks.Add(content.Load<Texture2D>("Sprite/Rock" + i));
            }

            BGWater = content.Load<Texture2D>("Background/BGWater1");
            BGSky = content.Load<Texture2D>("Background/BGSky");

            font = content.Load<SpriteFont>("Font/Font1");

            wallTexture = content.Load<Texture2D>("Sprite/wallTile");
            floorTexture = content.Load<Texture2D>("Sprite/floorTile");

        }
    }
}
