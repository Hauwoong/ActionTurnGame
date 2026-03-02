using UnityEditor.Build;

public interface IRng
{
    int Range(int min, int max);
    //float Value();
    //bool Chance(float p);
}

public class DeterministicRng : IRng
{
    private System.Random random;

    public DeterministicRng(int seed)
    {
        random = new System.Random(seed);
    }

    public int Range(int min, int max)
    {
        return random.Next(min, max + 1);
    }
}