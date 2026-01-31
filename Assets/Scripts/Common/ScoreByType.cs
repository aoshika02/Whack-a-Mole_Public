using System.Collections.Generic;

public static class ScoreByType
{
    private static readonly Dictionary<AnimalType, int> scoreTypeDict = new Dictionary<AnimalType, int>()
    {
        {  AnimalType.Crocodile, 100 },
        { AnimalType.Dog, -50 },
        { AnimalType.Turtle, -75 },
    };

    public static int GetScore(AnimalType type)
    {
        if (scoreTypeDict.TryGetValue(type, out int score))
        {
            return score;
        }
        return 0;
    }
}
