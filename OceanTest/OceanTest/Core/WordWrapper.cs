using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// From http://forums.create.msdn.com/forums/t/37808.aspx
/// </summary>

public static class WordWrapper
{
    static StringBuilder builder = new StringBuilder(" ");
    public static char[] NewLine = { '\r', '\n' };
    static Vector2 MeasureCharacter(this SpriteFont font, char character)
    {
        builder[0] = character;
        return font.MeasureString(builder);
    }

    //Helper to simply return the wrapped string
    public static string WrapWord(string text, SpriteFont font, Rectangle rectangle, float scale)
    {
        StringBuilder SB = new StringBuilder(text);
        StringBuilder wrappedSB = new StringBuilder();
        WordWrapper.WrapWord(SB, wrappedSB, font, rectangle, scale);
        return wrappedSB.ToString();
    }

    public static void WrapWord(StringBuilder original, StringBuilder target, SpriteFont font, Rectangle bounds, float scale)
    {
        int lastWhiteSpace = 0;
        float currentLength = 0;
        float lengthSinceLastWhiteSpace = 0;
        float characterWidth = 0;
        for (int i = 0; i < original.Length; i++)
        {
            //get the character 
            char character = original[i];
            //measure the length of the current line 
            characterWidth = font.MeasureCharacter(character).X * scale;
            currentLength += characterWidth;
            //find the length since last white space 
            lengthSinceLastWhiteSpace += characterWidth;
            //are we at a new line? 
            if ((character != '\r') && (character != '\n'))
            {
                //time for a new line? 
                if (currentLength > bounds.Width)
                {
                    //if so are we at white space? 
                    if (char.IsWhiteSpace(character))
                    {
                        //if so insert newline here 
                        target.Insert(i, NewLine);
                        //reset lengths 
                        currentLength = 0;
                        lengthSinceLastWhiteSpace = 0;
                        // return to the top of the loop as to not append white space 
                        continue;
                    }
                    else
                    {
                        //not at white space so we insert a new line at the previous recorded white space 
                        target.Insert(lastWhiteSpace, NewLine);
                        //remove the white space 
                        target.Remove(lastWhiteSpace + NewLine.Length, 1);
                        //make sure the the characters at the line break are accounted for 
                        currentLength = lengthSinceLastWhiteSpace;
                        lengthSinceLastWhiteSpace = 0;
                    }
                }
                else
                {
                    //not time for a line break? are we at white space? 
                    if (char.IsWhiteSpace(character))
                    {
                        //record it's location 
                        lastWhiteSpace = target.Length;
                        lengthSinceLastWhiteSpace = 0;
                    }
                }
            }
            else
            {
                lengthSinceLastWhiteSpace = 0;
                currentLength = 0;
            }
            //always append  
            target.Append(character);
        }
    }
}