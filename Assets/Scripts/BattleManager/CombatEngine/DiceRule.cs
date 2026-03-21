
public class DiceRule
{
    public ClashResult Win;
    public ClashResult Lose;
    public ClashResult Draw;

    public ClashResult Resolve(int rollA, int rollB)
    {
        if (rollA > rollB) return Win;
        if (rollA < rollB) return Lose;
        return Draw;
    }
}