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
    public class Collision
    {
        //Need to make sure the calculated bounds are within the array bounds
        public static void HandleCollisions(Level level, Rectangle BoundingRectangle, ref Vector2 Position)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / GameConstants.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / GameConstants.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / GameConstants.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / GameConstants.TileHeight)) - 1;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = rightTile; x >= leftTile; --x)
                {
                    if (x > 0 && y > 0 && x < level.Width && y < level.Height)
                    {
                        // If this tile is collidable,
                        if (level.tileArray[x, y].isSolid)
                        {
                            Rectangle tileBounds = TileHelper.GetTileRectangle(x, y);
                            // Determine collision depth (with direction) and magnitude.
                            bounds = HandleCollision(BoundingRectangle, bounds, true, tileBounds, ref Position);
                        }
                    }
                }
            }


        }

        private static Rectangle HandleCollision(Rectangle BoundingRectangle, Rectangle bounds, bool isSolid, Rectangle tileBounds, ref Vector2 Position)
        {
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                // Resolve the collision along the shallow axis.  
                if (absDepthY < absDepthX)
                {
                    // Ignore platforms, unless we are on the ground.  
                    if (isSolid)
                    {
                        // Resolve the collision along the Y axis.  
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                        // Perform further collisions with the new bounds.  Need to include change in position
                        bounds = new Rectangle(BoundingRectangle.X, (int)Math.Round(BoundingRectangle.Y + depth.Y), BoundingRectangle.Width, BoundingRectangle.Height);
                    }
                }
                else if (isSolid) // Ignore platforms.  
                {
                    // Resolve the collision along the X axis.  
                    Position = new Vector2(Position.X + depth.X, Position.Y);

                    // Perform further collisions with the new bounds.  
                    bounds = new Rectangle((int)Math.Round(BoundingRectangle.X + depth.X), BoundingRectangle.Y, BoundingRectangle.Width, BoundingRectangle.Height);
                    //bounds = BoundingRectangle;
                }
            }
            return bounds;
        }

    }
}
