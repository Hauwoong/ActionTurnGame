using UnityEngine;

public enum BoutType
{
    direct,
    Intercept,
    Unopposed
}

public class Bout
{
    public ActionInstance A;
    public ActionInstance B; // null °¡´É
    public BoutType Type;
    public int SpeedPriority;

    public Bout(ActionInstance a, ActionInstance b, BoutType type)
    {
        A = a;
        B = b;
        Type = type;
        SpeedPriority = Mathf.Max(a.Speed, b?.Speed ?? a.Speed);
    }
}
