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
    public class FloorRock
    {
        public int rockIndex;

        public Vector2 position;
        public float rotation;
        public float scale;

        public FloorRock(Vector2 position, int index, Random r)
        {
            this.position = position;
            this.rockIndex = index;
            this.rotation = r.Next(-1, 2) * (float)r.NextDouble();
            this.scale = (float)r.NextDouble() + .5f;

        }
    }

    public class Floor
    {
        public Level level;

        public List<Vector2> pointList;
        public List<FloorRock> rocks = new List<FloorRock>();


        public float StartX;
        public float EndX;

        public Floor(Level level, List<Vector2> pointList)
        {
            this.level = level;
            this.pointList = pointList;

            this.StartX = pointList[0].X;
            this.EndX = pointList[pointList.Count - 1].X;

            rocks = getRocks();

        }



        //Checks collision and returns the new position
        public Vector2 GetCollision(Vector2 position)
        {
            Vector2 floorPos = GetFloorAtPos(position);
            
            if (floorPos.Y < position.Y)
                position.Y = floorPos.Y;

           

            if (position.X < StartX)
                position.X = StartX;

            if (position.X > EndX)
                position.X = EndX;
            return position;
        }

      
        public Vector2 GetFloorAtPos(Vector2 pos)
        {
            //iterate till we find the right points
            int index = -1;
            for (int i = 0; i < pointList.Count; i++)
            {
                if (pointList[i].X > pos.X)
                {
                    index = i - 1;
                    break;
                }
            }

            if (index >= 0)
            {
                float dif = pointList[index + 1].X - pointList[index].X;
                float posX = pos.X - pointList[index].X;

               
                return new Vector2(pos.X, MathHelper.Lerp(pointList[index].Y, pointList[index + 1].Y, (float)posX / dif));
            }
            else
            {
                return new Vector2(pos.X, 10000);
            }
        }

        //distributes rocks along the ocean floor, to draw later
        public List<FloorRock> getRocks()
        {
            List<FloorRock> retval = new List<FloorRock>();


            int rockSize = 25;


            for (int i = 1; i < pointList.Count; i++)
            {
                //get distance
                float dist = DrawPrimitives.getDistance(pointList[i], pointList[i - 1]);
    
                Vector2 dir = pointList[i - 1] - pointList[i];
                dir = Helper.Normalize(dir);

                int rockCount = (int)Math.Floor( dist / (float)rockSize);

                for (int j = 0; j < rockCount; j++) //average size of 50 px
                {
                    Vector2 cur = pointList[i - 1] + (dir * j * rockSize);
                    retval.Add(new FloorRock(cur, level.game.r.Next(4),level.game.r));
                }
            }
            return retval;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //iterate through the point list and draw pixels?
            for (int i = 1; i < pointList.Count; i++)
            {
                DrawPrimitives.drawLine(pointList[i - 1], pointList[i], 5, level.game.textureLoader.WhitePixel, Color.Red, spriteBatch);
             //   DrawPointSegment(pointList[i - 1], pointList[i], gameTime, spriteBatch);
            }

            DrawRocks(gameTime, spriteBatch);
        }

        private void DrawRocks(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (FloorRock r in rocks)
            {
                Vector2 origin = new Vector2(level.game.textureLoader.Rocks[r.rockIndex].Width/2,level.game.textureLoader.Rocks[r.rockIndex].Height/2);
                spriteBatch.Draw(level.game.textureLoader.Rocks[r.rockIndex], r.position,null, Color.White,r.rotation,origin,r.scale,SpriteEffects.None,0);
            }
        }


        private void DrawPointSegment(Vector2 A, Vector2 B, GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 point = A;

            int dif = (int)(Math.Round(B.X) - Math.Round(A.X));
            
            for(int i=0;i<dif;i+=50)
            {
                DrawPrimitives.drawLine(point, new Vector2(point.X, point.Y + 50), 50, level.game.textureLoader.WhitePixel, Color.Red, spriteBatch);
                point.X = A.X + i;
                point.Y = MathHelper.Lerp(A.Y,B.Y,(float)i/(float)dif);
            }
        }


        public static List<Vector2> GetSampleFloor(Random r, int start, int depth, int width)
        {
            Curve oceanCurve = new Curve();
            oceanCurve.Keys.Add(new CurveKey(0, 0));
            oceanCurve.Keys.Add(new CurveKey(width * .25f, depth));
            oceanCurve.Keys.Add(new CurveKey(width * .75f, depth));
            oceanCurve.Keys.Add(new CurveKey(width, 0));

            List<Vector2> retval = new List<Vector2>();

            int num = 100;
            int unit = width/num;
            for (int i = 0; i < num; i++)
            {
                retval.Add(new Vector2(start + unit * i, oceanCurve.Evaluate(unit * i) + r.Next(-20,20))); //smooth
            }

            return retval;
        }
    }
}
