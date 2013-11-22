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
    public class Tile
    {
        public bool isSolid;
        public TileType type;

        public Tile(bool isSolid, TileType type)
        {
            this.isSolid = isSolid;
            this.type = type;
        }


        
    }

    public class TileHelper
    {
        //get the world position (center) of the tile
        public static Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(x * GameConstants.TileWidth + 25, y * GameConstants.TileHeight + 25);
        }

        //get the tile position that overlaps the world position
        public static Vector2 GetTilePosition(Vector2 position)
        {
            int x = (int)Math.Round(position.X / GameConstants.TileWidth);
            int y = (int)Math.Round(position.Y / GameConstants.TileHeight);
            return new Vector2(x, y);
        }

        public static Rectangle GetTileRectangle(int x, int y)
        {
            return new Rectangle(x * GameConstants.TileWidth, y * GameConstants.TileHeight, GameConstants.TileWidth, GameConstants.TileHeight);
        }


        public static bool CheckCollision(Rectangle boundingRec, Tile[,] tileArray, Direction dir)
        {
            bool retval = false;

            int leftTile = (int)Math.Floor((float)boundingRec.Left / GameConstants.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)boundingRec.Right / GameConstants.TileWidth)) -1;
            int topTile = (int)Math.Floor((float)boundingRec.Top / GameConstants.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)boundingRec.Bottom  / GameConstants.TileHeight)) ;

            switch (dir)
            {
                case Direction.Left:
                    for (int i = topTile; i <= bottomTile; i++)
                    {
                        if (tileArray[leftTile, i].isSolid) retval = true;
                    }
                    break;
                case Direction.Right:
                    for (int i = topTile; i <= bottomTile; i++)
                    {
                        if (tileArray[rightTile, i].isSolid) retval = true;
                    }
                    break;
                case Direction.Up:
                    for (int i = leftTile; i <= rightTile; i++)
                    {
                        if (tileArray[i, topTile].isSolid) retval = true;
                    }

                    break;
                case Direction.Down:
                    for (int i = leftTile; i <= rightTile; i++)
                    {
                        if (tileArray[i, bottomTile].isSolid) retval = true;
                    }
                    break;
                default:
                    break;
            }

            return retval;

        }
    }

    //put a level factory here?
}
