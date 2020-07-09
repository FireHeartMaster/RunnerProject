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
            int[,] auxGrabbableGrid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    auxGrid[i, j] = grid[i, j];
                    auxGrabbableGrid[i, j] = grabbableGrid[i, j];
                }
            }
            width = value > 0 ? value : 1;
            grid = new int[length, width];
            grabbableGrid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < auxGrid.GetLength(0) && j < auxGrid.GetLength(1))
                    {
                        grid[i, j] = auxGrid[i, j];
                        grabbableGrid[i, j] = auxGrabbableGrid[i, j];
                    }
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
            int[,] auxGrabbableGrid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    auxGrid[i, j] = grid[i, j];
                    auxGrabbableGrid[i, j] = grabbableGrid[i, j];
                }
            }
            length = value > 0 ? value : 1;
            grid = new int[length, width];
            grabbableGrid = new int[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < auxGrid.GetLength(0) && j < auxGrid.GetLength(1))
                    {
                        grid[i, j] = auxGrid[i, j];
                        grabbableGrid[i, j] = auxGrabbableGrid[i, j];
                    }
                }
            }
            RedrawTexture();
        }
    }

    public int lineThickness = 1;
    public int blockThickness = 5;

    int[,] grid = new int[5, 5];
    int[,] grabbableGrid = new int[5, 5];
    public int[,] GetGrid()
    {
        return grid;
    }
    public int[,] GetGrabbableGrid()
    {
        return grabbableGrid;
    }


    int numberOfBlockTypes = 5;
    int numberOfGrabbableTypes = 5;

    public GridScriptableObject gridData;

    public int NumberOfBlockTypes
    {
        get { return numberOfBlockTypes; }
        set 
        { 
            numberOfBlockTypes = value > 0 ? value : 1;           
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grid[i, j] = grid[i, j] >= numberOfBlockTypes ? 0 : grid[i, j];
                }
            }
            RedrawTexture();
        }
    }

    public int NumberOfGrabbableTypes
    {
        get { return numberOfGrabbableTypes; }
        set
        {
            numberOfGrabbableTypes = value > 0 ? value : 1;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {                    
                    grabbableGrid[i, j] = grabbableGrid[i, j] >= numberOfGrabbableTypes ? 0 : grabbableGrid[i, j];
                }
            }
            RedrawTexture();
        }
    }

    void DrawGrabbable(int posI, int posJ)
    {
        Color colorToSet;
        if (grabbableGrid[posI, posJ] != 0) colorToSet = Color.Lerp(Color.white, Color.blue, ((float)grabbableGrid[posI, posJ]) / (numberOfGrabbableTypes > 1 ? numberOfGrabbableTypes - 1 : 1));
        else return;

        int windowSize = Mathf.FloorToInt(0.5f * blockThickness);
        if(blockThickness % 2 != 0 || windowSize % 2 == 0)
        {
            windowSize++;
        }
        Color[] colors = new Color[windowSize * windowSize];
        
        for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
        {
            colors[colorIndex] = colorToSet;
        }
        texture.SetPixels((posJ) * (blockThickness + lineThickness) + lineThickness + ((int)(0.5f * (blockThickness - windowSize))), (length - posI - 1) * (blockThickness + lineThickness) + lineThickness + ((int)(0.5f * (blockThickness - windowSize))), windowSize, windowSize, colors);
    }
    public void RedrawTexture()
    {
        texture = new Texture2D(blockThickness * width + lineThickness * (width + 1), blockThickness * length + lineThickness * (length + 1));

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
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

                colors = new Color[blockThickness * blockThickness];
                Color colorToSet = Color.Lerp(Color.white, Color.red, ((float)grid[i, j]) / (numberOfBlockTypes > 1 ? numberOfBlockTypes - 1 : 1));
                for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                {
                    colors[colorIndex] = colorToSet;
                }
                texture.SetPixels((j) * (blockThickness + lineThickness) + lineThickness, (length - i - 1) * (blockThickness + lineThickness) + lineThickness, blockThickness, blockThickness, colors);

                DrawGrabbable(i, j);
            }
        }
        texture.Apply();
    }

    public void LoadGrid(int[] auxLoadedGrid, int[] auxLoadedGrabbableGrid, int loadedLength, int loadedWidth, int numberOfBlockTypes, int numberOfGrabbableTypes)
    {
        length = loadedLength;
        width = loadedWidth;
        this.numberOfBlockTypes = numberOfBlockTypes;
        this.numberOfGrabbableTypes = numberOfGrabbableTypes;

        grid = new int[length, width];
        grabbableGrid = new int[length, width];
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = auxLoadedGrid[i * width + j];
                grabbableGrid[i, j] = auxLoadedGrabbableGrid[i * width + j];
            }
        }

        RedrawTexture();
    }

    public void SaveGrid()
    {
        gridData.numberOfBlockTypes = numberOfBlockTypes;
        gridData.obstacleGrid = new int[length, width];

        gridData.numberOfGrabbableTypes = numberOfGrabbableTypes;
        gridData.collectableGrid = new int[length, width];

        gridData.length = length;
        gridData.width = width;

        gridData.auxObstacleGrid = new int[length * width];
        gridData.auxCollectableGrid = new int[length * width];

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int a = gridData.obstacleGrid[i, j];
                int b = grid[i, j];
                gridData.obstacleGrid[i, j] = grid[i, j];

                gridData.collectableGrid[i, j] = grabbableGrid[i, j];

                gridData.auxObstacleGrid[i * width + j] = gridData.obstacleGrid[i, j];
                gridData.auxCollectableGrid[i * width + j] = gridData.collectableGrid[i, j];
            }
        }
    }

    public void ResetGrid(int blockIndex = 0, int grabbableIndex = 0)
    {
        grid = new int[length, width];
        grabbableGrid = new int[length, width];
        texture = new Texture2D(blockThickness * width + lineThickness * (width + 1), blockThickness * length + lineThickness * (length + 1));

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = blockIndex;
                grabbableGrid[i, j] = grabbableIndex;
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

                colors = new Color[blockThickness * blockThickness];
                Color colorToSet = Color.Lerp(Color.white, Color.red, ((float)grid[i, j]) / (numberOfBlockTypes > 1 ? numberOfBlockTypes - 1 : 1));
                for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
                {
                    colors[colorIndex] = colorToSet;
                }
                texture.SetPixels((j) * (blockThickness + lineThickness) + lineThickness, (length - i - 1) * (blockThickness + lineThickness) + lineThickness, blockThickness, blockThickness, colors);

                DrawGrabbable(i, j);
            }
        }
        texture.Apply();
    }

    public void ResetGridToBasicFloor()
    {
        if(numberOfBlockTypes >= 2)
        {
            ResetGrid(1, 0);
        }
    }

    public Texture2D GenerateTexture(int posI = -1, int posJ = -1, int clickLeft = -1)
    {
        if (texture == null || grid == null)
        {
            ResetGrid();
        }

        if (posI < 0 || posI >= length || posJ < 0 || posJ >= width)
        {
            return texture;
        }

        if (clickLeft == 0)
        {
            grid[posI, posJ] = (grid[posI, posJ] + 1) % numberOfBlockTypes;
        }else if(clickLeft == 1)
        {
            grabbableGrid[posI, posJ] = (grabbableGrid[posI, posJ] + 1) % numberOfGrabbableTypes;
        }

        Color[] newColors = new Color[blockThickness * blockThickness];
        Color colorToSet = Color.Lerp(Color.white, Color.red, ((float)grid[posI, posJ]) / (numberOfBlockTypes > 1 ? numberOfBlockTypes - 1 : 1));
        for (int colorIndex = 0; colorIndex < newColors.Length; colorIndex++)
        {
            newColors[colorIndex] = colorToSet;
        }

        texture.SetPixels((posJ) * (blockThickness + lineThickness) + lineThickness, (length - 1 - posI) * (blockThickness + lineThickness) + lineThickness, blockThickness, blockThickness, newColors);
        DrawGrabbable(posI, posJ);
        texture.Apply();

        return texture;

    }
}
