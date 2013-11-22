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
    public class Wave
    {
        public Level level;
        
        public float surfaceY;

        public Texture2D texture;
        public Vector2 origin;

        public TimeSpan WaveTimer;
        public float WaveTime;

        public float waveRange;

        public List<Vector2> wavePositions = new List<Vector2>();

        public Wave(Level level, float surfaceY)
        {
            this.level = level;
            this.surfaceY = surfaceY;

            this.WaveTimer = TimeSpan.FromSeconds(0);

            //this.waveRange = 50;

            LoadContent();
        }

        private void LoadContent()
        {
            texture = level.game.textureLoader.Wave;


            InitWave();
        }
        //init wave
        //go from -5000 - 5000
        //waves spaced 45 apart
        private void InitWave()
        {
            for (int i = -5000; i < 5000; i += 45)
            {
                wavePositions.Add(new Vector2(i, surfaceY-40));
            }
        }

        public void Update(GameTime gameTime)
        {
            WaveTimer += gameTime.ElapsedGameTime;
           

            //oscilate each wave
            for (int i = 0; i < wavePositions.Count; i++)
            {
                Vector2 tempVector;
                if (i % 2 == 0)
                {
                   // tempVector = wavePositions[i] + new Vector2((int)(Math.Sin(WaveTimer.TotalSeconds) * waveRange), (int)(Math.Cos(WaveTimer.TotalSeconds) * waveRange));
                    tempVector = wavePositions[i] + new Vector2((int)(Math.Sin(WaveTimer.TotalSeconds * 6) * 2), (int)(Math.Cos(WaveTimer.TotalSeconds * 6) * -2));
                }
                else
                {
                    //tempVector = wavePositions[i];
                    //tempVector = wavePositions[i] + new Vector2((int)(Math.Cos(WaveTimer.TotalSeconds) * waveRange), (int)(Math.Sin(WaveTimer.TotalSeconds) * waveRange));
                    tempVector = wavePositions[i] + new Vector2((int)(Math.Sin(WaveTimer.TotalSeconds * 12) * 2), (int)(Math.Cos(WaveTimer.TotalSeconds * 12) * -2));
                }
                wavePositions[i] = tempVector;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw all evens, then all odds
            for (int i = 0; i < wavePositions.Count; i++)
            {
                if (i % 2 == 0)
                {
                    spriteBatch.Draw(texture, wavePositions[i], Color.White);
                }
            }
            for (int i = 0; i < wavePositions.Count; i++)
            {
                 if (i % 2 != 0)
                {
                    spriteBatch.Draw(texture, wavePositions[i], Color.White);
                 }
            }


        }


    }
}
