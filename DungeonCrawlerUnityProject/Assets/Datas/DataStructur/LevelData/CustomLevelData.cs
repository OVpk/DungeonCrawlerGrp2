using UnityEngine;

public class CustomLevelData : AbstractLevelData
{
    [SerializeField] private AreaData[] customLevel;
    public AreaData[,] customLevel2d => ConvertTo2dArray();

    public void Init(AreaData[,] levelToSave)
    {
        height = levelToSave.GetLength(0);
        width = levelToSave.GetLength(1);
        customLevel = new AreaData[height * width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                customLevel[i * width + j] = levelToSave[i, j];
            }
        }
    }

    private AreaData[,] ConvertTo2dArray()
    {
        AreaData[,] result = new AreaData[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                result[i, j] = customLevel[i * width + j];
            }
        }
        return result;
    }
}
