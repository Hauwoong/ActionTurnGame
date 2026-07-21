
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

    public int Range(int min, int max) // max Æ÷ÇÔ 
    {
        return random.Next(min, max + 1);
    }
}