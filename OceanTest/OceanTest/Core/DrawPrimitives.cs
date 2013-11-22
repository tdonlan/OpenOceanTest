using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


    //Class to draw shapes: squares, rectangles, lines, circles, curves
    class DrawPrimitives
    {

        public static void DrawRectangle(Rectangle rec, Texture2D tex, Color col, SpriteBatch spriteBatch, bool solid, int thickness)
        {
            if (!solid)
            {

                Vector2 Position = new Vector2(rec.X, rec.Y);
                int border = thickness;

                int borderWidth = (int)(rec.Width) + (border * 2);
                int borderHeight = (int)(rec.Height) + (border);

                drawStraightLine(new Vector2((int)rec.X, (int)rec.Y), new Vector2((int)rec.X + rec.Width, (int)rec.Y), tex, col, spriteBatch, thickness); //top bar
                drawStraightLine(new Vector2((int)rec.X, (int)rec.Y + rec.Height), new Vector2((int)rec.X + rec.Width, (int)rec.Y + rec.Height), tex, col, spriteBatch, thickness); //bottom bar
                drawStraightLine(new Vector2((int)rec.X, (int)rec.Y), new Vector2((int)rec.X, (int)rec.Y + rec.Height), tex, col, spriteBatch, thickness); //left bar
                drawStraightLine(new Vector2((int)rec.X + rec.Width, (int)rec.Y), new Vector2((int)rec.X + rec.Width, (int)rec.Y + rec.Height + thickness), tex, col, spriteBatch, thickness); //right bar
            }
            else
            {
                spriteBatch.Draw(tex, new Vector2((float)rec.X, (float)rec.Y), rec, col, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            }

        }

        //draws a line (rectangle of thickness) from A to B.  A and B have make either horiz or vert line.
        public static void drawStraightLine(Vector2 A, Vector2 B, Texture2D tex, Color col, SpriteBatch spriteBatch, int thickness)
        {
            Rectangle rec;
            if (A.X < B.X) // horiz line
            {
                rec = new Rectangle((int)A.X, (int)A.Y, (int)(B.X - A.X), thickness);
            }
            else //vert line
            {
                rec = new Rectangle((int)A.X, (int)A.Y, thickness, (int)(B.Y - A.Y));
            }

            spriteBatch.Draw(tex, rec, col);

            //spriteBatch.Draw(tex, new Vector2((float)rec.X, (float)rec.Y), rec, col, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

        }



        //may want to change content model
        public static void drawLine(Vector2 A, Vector2 B, int width, Texture2D pixel, Color col, SpriteBatch spriteBatch)
        {
            try
            {
                int dist = Convert.ToInt32(Math.Round(getDistance(A, B)));

                int recX = Convert.ToInt32(Math.Round(A.X));
                int recY = Convert.ToInt32(Math.Round(A.Y));

                float angle = (float)Math.Atan2((double)(B.Y - A.Y), (double)(B.X - A.X));


                spriteBatch.Draw(pixel, new Rectangle(recX, recY, dist, width), null, col, angle, new Vector2(0, 0), SpriteEffects.None, 0f);
            }
            catch (Exception ex)
            {
            }
        }

        public static void drawOffsetSinCurve(Vector2 A, Vector2 B, float seconds, int units, int width, Texture2D tex, Color col1, Color col2, SpriteBatch spriteBatch)
        {
            float dif = Math.Abs(B.X - A.X);
            float unitDist = dif / (float)units;

            Vector2 cur = A;

            for (int i = 0; i < units; i++)
            {


                Vector2 temp = new Vector2(cur.X, cur.Y + (float)Math.Sin((double)(i + seconds)) * 10);
                Vector2 temp2 = new Vector2(cur.X + unitDist, cur.Y + (float)Math.Sin((double)(i + 1 + seconds)) * 10);


                drawLine(temp + new Vector2(0, -20), temp2 + new Vector2(0, -20), 20, tex, col1, spriteBatch);
                drawLine(temp, temp2, width, tex, col1, spriteBatch);
                drawLine(temp + new Vector2(0, 1), temp2 + new Vector2(0, 1), 30, tex, col2, spriteBatch);


                cur.X += unitDist-2f;
            }

        }


        public static float getDistance(Vector2 vec1, Vector2 vec2)
        {
            return (float)Math.Sqrt(Math.Pow((vec2.X - vec1.X), 2) + Math.Pow((vec2.Y - vec1.Y), 2));
        }

        public static void DrawCircle(Vector2 origin, float radius, Texture2D tex, Color col, SpriteBatch spriteBatch)
        {


            Vector2 tempVec;
            //iterate through 2Pi radians to get the full circle
            //might  need to increase the iterations if the radius is espcially large
            for (float i = 0; i < (float)Math.PI * 2; i += (float)Math.PI / 180)
            {
                tempVec = new Vector2((float)(origin.X + radius * Math.Cos(i)), (float)(origin.Y + radius * Math.Sin(i) * -1));

                spriteBatch.Draw(tex, tempVec, col);
            }
        }

        public static void DrawCircle(Circle c, Texture2D tex, Color col, SpriteBatch spriteBatch)
        {
            DrawCircle(c.Center, c.Radius, tex, col, spriteBatch);
        }


        //Currently now showing stats.
        public static void DrawHealthBar(SpriteBatch spriteBatch, Texture2D whitePixel, Rectangle rec, Color col, bool showStats, bool border, int val, int totalVal)
        {
            //Draw the Health Bar
            float ratio = (float)val / (float)totalVal;

            int healthWidth = (int)Math.Round(rec.Width * ratio);

            Rectangle drawRec = rec;
            drawRec.Width = healthWidth;

            if (border)
            {
                Rectangle borderRec = new Rectangle(rec.X - 1, rec.Y - 1, rec.Width + 2, rec.Height + 2);
                DrawPrimitives.DrawRectangle(borderRec, whitePixel, Color.White, spriteBatch, false, 2);

            }

            DrawPrimitives.DrawRectangle(drawRec, whitePixel, col, spriteBatch, true, 2);
        }

        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color frontColor, Color backColor, Vector2 origin, float scale)
        {

            value = DrawPrimitives.CleanString(value);

            spriteBatch.DrawString(font, value, position + new Vector2(2.0f, 2.0f), backColor, 0f, origin, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, value, position, frontColor, 0f, origin, scale, SpriteEffects.None, 0f);
        }




        //Calls the wordWrap Helper
        public static void DrawStringWordWrap(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, int width)
        {
            StringBuilder SB = new StringBuilder(DrawPrimitives.CleanString(text));
            StringBuilder wrappedSB = new StringBuilder();
            Rectangle rec = new Rectangle((int)position.X, (int)position.Y, width, 999);
            WordWrapper.WrapWord(SB, wrappedSB, font, rec, 1f);

            spriteBatch.DrawString(font, wrappedSB.ToString(), position, Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
        }

        //Remove Non-Alphanumeric values from string.  Include some punctuation.
        public static string CleanString(string val)
        {
            if (val != "" && val != null)
            {
                return Regex.Replace(val, @"[^a-zA-Z0-9 :*/%.'?!,+-]", "");
            }
            else
            {
                return "";
            }
        }

    }

