using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelObstacle
{
    Texture2D texture;

    int width = 5;
    int length = 5;

    public int Width
    {
        get { return width; }
        set
        {
            int[,] auxGrid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    auxGrid[i, j] = grid[i, j];
                }
            }
            width = value > 0 ? value : 1;
            grid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < auxGrid.GetLength(0) && j < auxGrid.GetLength(1)) grid[i, j] = auxGrid[i, j];
                }
            }
            RedrawTexture();
        }
    }
    public int Length
    {
        get { return length; }
        set
        {
            int[,] auxGrid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    auxGrid[i, j] = grid[i, j];
                }
            }
            length = value > 0 ? value : 1;
            grid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < auxGrid.GetLength(0) && j < auxGrid.GetLength(1)) grid[i, j] = auxGrid[i, j];
                }
            }
            RedrawTexture();
        }
    }

    public int lineThickness = 1;
    public int blockThickness = 5;

    int[,] grid = new int[5, 5];
    int[,] GetGrid()
    {
        return grid;
    }

    public int numberOfBlockTypes = 3;
    public int numberOfBlockTypes_ = 5;

    public void RedrawTexture()
    {
        texture = new Texture2D(blockThickness * width + lineThickness * (width + 1), blockThickness * length + lineThickness * (length + 1));

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //grid[i, j] = 0;
                Color[] colors = new Color[(blockThickness + lineThickness) * lineThickness];
                for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                {
                    colors[colorIndex] = Color.black;
                }
                texture.SetPixels(j * (blockThickness + lineThickness), (length - i) * (blockThickness + lineThickness), (blockThickness + lineThickness), lineThickness, colors);
                texture.SetPixels(j * (blockThickness + lineThickness), (length - i - 1) * (blockThickness + lineThickness) + lineThickness, lineThickness, (blockThickness + lineThickness), colors);
                if (j == width - 1)
                {
                    texture.SetPixels((j + 1) * (blockThickness + lineThickness), (length - i - 1) * (blockThickness + lineThickness) + lineThickness, lineThickness, (blockThickness + lineThickness), colors);
                }
                if (i == length - 1)
                {
                    texture.SetPixels(j * (blockThickness + lineThickness), 0, (blockThickness + lineThickness), lineThickness, colors);
                }
                if (j == width - 1 && i == length - 1)
                {
                    colors = colors = new Color[lineThickness * lineThickness];
                    for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                    {
                        colors[colorIndex] = Color.black;
                    }
                    texture.SetPixels((j + 1) * (blockThickness + lineThickness), 0, lineThickness, lineThickness, colors);
                }

                colors = colors = new Color[blockThickness * blockThickness];
                for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                {
                    colors[colorIndex] = Color.Lerp(Color.white, Color.red, ((float)grid[i, j]) / numberOfBlockTypes_);
                }
                texture.SetPixels((j) * (blockThickness + lineThickness) + lineThickness, (length - i - 1) * (blockThickness + lineThickness) + lineThickness, blockThickness, blockThickness, colors);
            }
        }
        texture.Apply();
    }
    public Texture2D GenerateTexture(int posI = -1, int posJ = -1)
    {
        if (texture == null || grid == null)
        {
            grid = new int[length, width];
            texture = new Texture2D(blockThickness * width + lineThickness * (width + 1), blockThickness * length + lineThickness * (length + 1));

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grid[i, j] = 0;
                    Color[] colors = new Color[(blockThickness + lineThickness) * lineThickness];
                    for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                    {
                        colors[colorIndex] = Color.black;
                    }
                    texture.SetPixels(j * (blockThickness + lineThickness), (length - i) * (blockThickness + lineThickness), (blockThickness + lineThickness), lineThickness, colors);
                    texture.SetPixels(j * (blockThickness + lineThickness), (length - i - 1) * (blockThickness + lineThickness) + lineThickness, lineThickness, (blockThickness + lineThickness), colors);
                    if (j == width - 1)
                    {
                        texture.SetPixels((j + 1) * (blockThickness + lineThickness), (length - i - 1) * (blockThickness + lineThickness) + lineThickness, lineThickness, (blockThickness + lineThickness), colors);
                    }
                    if (i == length - 1)
                    {
                        texture.SetPixels(j * (blockThickness + lineThickness), 0, (blockThickness + lineThickness), lineThickness, colors);
                    }

                    if (j == width - 1 && i == length - 1)
                    {
                        colors = colors = new Color[lineThickness * lineThickness];
                        for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                        {
                            colors[colorIndex] = Color.black;
                        }
                        texture.SetPixels((j + 1) * (blockThickness + lineThickness), 0, lineThickness, lineThickness, colors);
                    }

                    colors = colors = new Color[blockThickness * blockThickness];
                    for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                    {
                        colors[colorIndex] = Color.white;
                    }
                    texture.SetPixels((j) * (blockThickness + lineThickness) + lineThickness, (length - i - 1) * (blockThickness + lineThickness) + lineThickness, blockThickness, blockThickness, colors);
                }
            }
            texture.Apply();
        }

        //Debug.Log("posI: " + posI + ", posJ: " + posJ);
        if (posI < 0 || posI >= length || posJ < 0 || posJ >= width)
        {
            //Debug.Log("returning without modifications");
            return texture;
        }

        //Debug.Log("returning with modifications");

        //Debug.Log("before: " + posI + ", " + posJ);
        //Debug.Log("before: grid[" + posI + ", " + posJ + "] = " + grid[posI, posJ]);

        grid[posI, posJ] = (grid[posI, posJ] + 1) % numberOfBlockTypes_;

        //Debug.Log("grid[" + posI + ", " + posJ + "] = " + grid[posI, posJ]);
        //Debug.Log("numberOfBlockTypes_: " + numberOfBlockTypes_);
        //Debug.Log("(grid[posX, posY] + 1) % numberOfBlockTypes_: " + ((grid[posI, posJ] + 1) % numberOfBlockTypes_));


        Color[] newColors = new Color[blockThickness * blockThickness];
        Color colorToSet = Color.Lerp(Color.white, Color.red, ((float)grid[posI, posJ]) / numberOfBlockTypes_);
        for (int colorIndex = 0; colorIndex < newColors.Length; colorIndex++)
        {
            newColors[colorIndex] = colorToSet;
        }

        texture.SetPixels((posJ) * (blockThickness + lineThickness) + lineThickness, (length - 1 - posI) * (blockThickness + lineThickness) + lineThickness, blockThickness, blockThickness, newColors);
        //texture.SetPixels(0, 0, blockThickness, blockThickness, newColors);
        texture.Apply();


        //Debug.Log("returning");
        return texture;

    }
}
