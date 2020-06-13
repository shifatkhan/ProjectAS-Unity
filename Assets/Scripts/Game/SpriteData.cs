using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** This class takes care of converting a char sprite sheet with all the letters
 *  into a Dictionary of letters that can be used for normal text.
 *  
 *  P.S.: Make sure that the char sheet has characters that are evenly spaced and
 *  each char sprite is square size & that wrap mode is enabled.
 *  
 *  @author ShifatKhan
 */
public class SpriteData : MonoBehaviour
{
    // TODO: Remove special characters for multiplatform support.
    private char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?,.;:'\"`/|\\()[]{}<>≪≫@&#$£¥€+-×÷=*%~_∞♥♂♀".ToCharArray();

    public string charSheetFilename;        // Sprite char font sheet (in Resources folder)
    private Sprite[] charSprites;   // Sprites of each chars.
    public int spriteSize;

    public Dictionary<char, CharData> charData;

    void Start()
    {
        if(charSheetFilename == null)
        {
            // Default to m5x7 char sheet.
            charSheetFilename = "font_m5x7";
        }

        GetSubsprites();
        GetSpritesWidth();
    }

    /** Loads all the char sprites from the Resources folder.
     */
    public void GetSubsprites()
    {
        Sprite[] subsprites = Resources.LoadAll<Sprite>(charSheetFilename);
        charSprites = subsprites;

        Debug.Log("Height: " + charSprites[0].texture.height);
        Debug.Log("Width: " + charSprites[0].texture.width);
    }

    /** Iterate through each char sprite and find their width.
     */
    public void GetSpritesWidth()
    {
        int height = charSprites[0].texture.height;
        int width = charSprites[0].texture.width;

        int charIndex = 0;

        charData = new Dictionary<char, CharData>();

        for(int spriteY = 0; spriteY < height; spriteY += spriteSize)
        {
            for(int spriteX = 0; spriteX < width; spriteX += spriteSize)
            {
                int pixelX = 0;
                int pixelY = 0;

                int min = 0;
                int max = spriteSize;

                // Get min width
                while (min == 0 && pixelX < spriteSize)
                {
                    for (pixelY = 0; pixelY > -spriteSize; pixelY--)
                    {
                        // TODO: Change this so it works without wrap-mode enabled.
                        if (charSprites[charIndex].texture.GetPixel(spriteX + pixelX, spriteY-1 - pixelY).a != 0)
                        {
                            min = pixelX;
                        }
                    }
                    pixelX++;
                }

                pixelX = spriteSize;

                // Get max width
                while (max == spriteSize && pixelX > 0)
                {
                    for (pixelY = 0; pixelY < spriteSize; pixelY++)
                    {
                        if (charSprites[charIndex].texture.GetPixel(spriteX + pixelX, spriteY - pixelY).a != 0)
                        {
                            max = pixelX;
                        }
                    }
                    pixelX--;
                }

                // Calculate final width
                int charWidth = max - min + 1;
                // Add to dictionary
                if (charWidth <= spriteSize && charIndex < chars.Length)
                {
                    char c = chars[charIndex];
                    Sprite charSprite = charSprites[charIndex];

                    charData.Add(c, new CharData(charWidth, charSprite));

                    charIndex++;
                }
            }
        }
    }

    /** Holds information about a char sprite.
     */
    public struct CharData
    {
        public int width;
        public Sprite sprite;

        public CharData(int width, Sprite sprite)
        {
            this.width = width;
            this.sprite = sprite;
        }
    }
}
