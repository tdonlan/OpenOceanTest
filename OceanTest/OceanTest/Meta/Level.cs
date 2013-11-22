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
    public class Level
    {
        //Game
        public Game1 game;

        //Tiles
        public Tile[,] tileArray;
        public int Width
        {
            get { return tileArray.GetLength(0); }
        }

        public int Height
        {
            get { return tileArray.GetLength(1); }
        }

        //Background
        public Color oceanColor;
        public Rectangle BGRectangle;

        //Game Objects
        public Player player;
        public List<Fish> fish = new List<Fish>();
        public List<FlockFish> flockFish = new List<FlockFish>();
        public List<Seaweed> seaweed = new List<Seaweed>();


        public Floor oceanFloor;
        public Wave oceanWave;
        public TimeSpan waveTimer;

        //Camera
        private float cameraPosition;
        private float cameraPositionYAxis;
        private int viewHeight;
        private int viewWidth;

        //Texting 
        public string testOutput;

        public Level(Game1 game)
        {
            this.game = game;
            player = new Player(this,new Vector2(1000,1000));
            SpawnFish(500);
            SpawnFlockFish(100);
            SpawnShark(10);
            

            //oceanFloor = new Floor(this, Floor.GetSampleFloor(game.r, -5000, 5000, 10000));
            //oceanWave = new Wave(this, 0);

            //SpawnSeaweed(20);

            LoadMazeLevel();

        }

        private void LoadLevel()
        {
            tileArray = new Tile[50, 50];
            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    bool isSolid = false;
                    if (i == 0 || i == tileArray.GetLength(0) - 1 || j == 0 || j == tileArray.GetLength(1) - 1)
                        isSolid = true;

                    if (i % 4 == 0 && j % 4 == 0)
                        isSolid = true;

                    tileArray[i, j] = new Tile(isSolid, TileType.Stone);
                }
            }
        }

        private void LoadMazeLevel()
        {
            int height = 100;
            int width = 100;
            CellAutoCave CACave = new CellAutoCave(width, height);

            tileArray = new Tile[width, height];

            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    bool isSolid = false;
                    if (i == 0 || i == tileArray.GetLength(0) - 1 || j == tileArray.GetLength(1) - 1)
                    {
                        isSolid = true;
                    }
                    else
                    {

                        if (CACave.grid[i, j] == 1)
                        {
                            isSolid = true;
                        }

                    }


                    tileArray[i, j] = new Tile(isSolid, TileType.Stone);
                }
            }

        }


        

        public void Update(GameTime gameTime)
        {
            waveTimer += gameTime.ElapsedGameTime;

            player.Update(gameTime);

            UpdateOceanColor();
            UpdateFish(gameTime);
            UpdateFlockFish(gameTime);
            UpdateSeaweed(gameTime);
            //UpdateWave(gameTime);
            

            testOutput = player.Velocity + " , " + player.rotation;
        }

        private void UpdateFish(GameTime gameTime)
        {
            for (int i = 0; i < fish.Count; i++)
            {
                if (isActive(fish[i].Position))
                {
                    fish[i].Update(gameTime);
                }
            }
        }

        private void UpdateFlockFish(GameTime gameTime)
        {
            for (int i = 0; i < flockFish.Count; i++)
            {
                if (isActive(flockFish[i].Position))
                {
                    flockFish[i].Update(gameTime);
                }
            }
        }

        private void UpdateSeaweed(GameTime gameTime)
        {
            for (int i = 0; i < seaweed.Count; i++)
            {
                if (isActive(seaweed[i].rootPosition))
                {
                    seaweed[i].Update(gameTime);
                }
            }
        }

        private void UpdateWave(GameTime gameTime)
        {
            if (isActive(new Vector2(player.Position.X, 0)))
            {
                oceanWave.Update(gameTime);
            }
        }

        #region Spawn



        public void SpawnFish(int n)
        {
            for (int i = 0; i < n; i++)
            {
                fish.Add(new Fish(this, new Vector2(game.r.Next(-2000, 2000), game.r.Next(100, 5000)),FishType.Normal));
            }

        }

        public void SpawnShark(int n)
        {
            for (int i = 0; i < n; i++)
            {
                fish.Add(new Fish(this, new Vector2(game.r.Next(-2000, 2000), game.r.Next(2000, 5000)), FishType.Shark));
            }
        }

        public void SpawnSeaweed(int n)
        {
            for (int i = 0; i < n; i++)
            {
                //seaweed.Add(new Seaweed(this, new Vector2(0,1000), 10));
                Vector2 temp = new Vector2(game.r.Next(-2000, 2000),-100);

                seaweed.Add(new Seaweed(this, oceanFloor.GetFloorAtPos(temp), game.r.Next(10, 20)));
            }
        }

        public void SpawnFlockFish(int n)
        {
            for (int i = 0; i < n; i++)
            {
                flockFish.Add(new FlockFish(this, new Vector2(game.r.Next(-2000,2000), game.r.Next(100, 1000))));
            }
        }

        #endregion

        private void UpdateOceanColor()
        {
            oceanColor = Color.Lerp(Color.LightBlue, Color.Black, (player.Position.Y / (float)GameConstants.OceanDepth));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition, -cameraPositionYAxis, 0.0f);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);


            //draw ocean
            spriteBatch.Draw(game.textureLoader.BGWater, new Vector2(cameraPosition, cameraPositionYAxis), oceanColor);
            //spriteBatch.Draw(game.textureLoader.WhitePixel, new Vector2(cameraPosition, cameraPositionYAxis), oceanColor);

            //draw Sky
            spriteBatch.Draw(game.textureLoader.BGSky, new Vector2(cameraPosition, -720), Color.White);
            //spriteBatch.Draw(game.textureLoader.WhitePixel, new Vector2(cameraPosition, -720), Color.White);


            DrawTiles(spriteBatch);
        
            //surface
            DrawPrimitives.drawOffsetSinCurve(new Vector2(-5000, 0), new Vector2(10000, 0), (float)waveTimer.TotalSeconds * 5, 100, 4, game.textureLoader.WhitePixel, Color.White,oceanColor, spriteBatch);

            //DrawPrimitives.drawLine(new Vector2(-5000, 0), new Vector2(10000, 0), 5, game.textureLoader.WhitePixel, Color.Black, spriteBatch);


            foreach (Fish f in fish)
                f.Draw(gameTime, spriteBatch);

            foreach (FlockFish f in flockFish)
                f.Draw(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);

            foreach (Seaweed s in seaweed)
                s.Draw(gameTime, spriteBatch);


           //oceanFloor.Draw(gameTime, spriteBatch);
           //oceanWave.Draw(gameTime, spriteBatch);

            DrawHUD(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private void DrawTiles(SpriteBatch spriteBatch)
        {

            // Calculate the visible range of tiles.
            int left = (int)Math.Floor(cameraPosition / GameConstants.TileWidth);
            int right = left + (spriteBatch.GraphicsDevice.Viewport.Width / GameConstants.TileWidth) + 1;
            right = Math.Min(right, Width - 1);

            int top = (int)Math.Floor(cameraPositionYAxis / GameConstants.TileHeight);
            int bottom = top + (spriteBatch.GraphicsDevice.Viewport.Height / GameConstants.TileHeight) + 1;
            //bottom = Math.Min(bottom, Height + 1);

            // For each tile position
            for (int y = top; y <= bottom; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    if (x > 0 && x < Width && y > 0 && y < Height)
                    {
                        DrawTile(spriteBatch, tileArray[x, y], x, y);
                    }

                }
            }
        }


        private void DrawTile(SpriteBatch spriteBatch, Tile t, int x, int y)
        {
            Rectangle rec = TileHelper.GetTileRectangle(x, y);
            if (t.isSolid)
            {
                spriteBatch.Draw(game.textureLoader.wallTexture, rec, Color.White);
            }
            else
            {
               // spriteBatch.Draw(game.textureLoader.floorTexture, rec, Color.White);
            }


        }


        private void DrawHUD(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(game.textureLoader.font, testOutput, new Vector2(cameraPosition, cameraPositionYAxis), Color.Red);
        }

        public bool isActive(Vector2 pos)
        {
            Rectangle activeRec = new Rectangle((int)cameraPosition - viewWidth, (int)cameraPositionYAxis - viewHeight, viewWidth * 4, viewHeight * 4);
            if (activeRec.Contains((int)pos.X, (int)pos.Y))
                return true;
            else
                return false;
        }

        public bool isOnScreen(Rectangle rec)
        {
            int left = (int)Math.Floor(cameraPosition);
            int right = left + viewWidth;

            int top = (int)Math.Floor(cameraPositionYAxis);
            int bottom = top + viewHeight;

            if (((rec.Left) >= left && (rec.Right) <= right) &&
                ((rec.Top) >= top && (rec.Bottom) <= bottom))
                return true;
            else
                return false;
        }

        private void ScrollCamera(Viewport viewport)
        {

            const float ViewMargin = 0.4f;

            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = cameraPosition + marginWidth;
            float marginRight = cameraPosition + viewport.Width - marginWidth;

            //const float TopMargin = 0.5f;
            //const float BottomMargin = 0.1f;

            const float TopMargin = 0.4f;
            const float BottomMargin = 0.4f;
            float marginTop = cameraPositionYAxis + viewport.Height * TopMargin;
            float marginBottom = cameraPositionYAxis + viewport.Height - viewport.Height * BottomMargin;


            viewHeight = viewport.Height;
            viewWidth = viewport.Width;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (player.Position.X < marginLeft)
                cameraMovement = player.Position.X - marginLeft;
            else if (player.Position.X > marginRight)
                cameraMovement = player.Position.X - marginRight;

            // Calculate how far to vertically scroll when the player is near the top or bottom of the screen.  
            float cameraMovementY = 0.0f;
            if (player.Position.Y < marginTop) //above the top margin  
                cameraMovementY = player.Position.Y - marginTop;
            else if (player.Position.Y > marginBottom) //below the bottom margin  
                cameraMovementY = player.Position.Y - marginBottom;

            // Update the camera position, but prevent scrolling off the ends of the level.
            /*
            float maxCameraPosition = Tile.Width * Width - viewport.Width;
            float maxCameraPositionYOffset = Tile.Height * Height - viewport.Height;
            
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, 5000);
            cameraPositionYAxis = MathHelper.Clamp(cameraPositionYAxis + cameraMovementY, 0.0f, 5000);
             * */

            cameraPosition = cameraPosition + cameraMovement;

            cameraPositionYAxis = cameraPositionYAxis + cameraMovementY;
        }


    }
}
