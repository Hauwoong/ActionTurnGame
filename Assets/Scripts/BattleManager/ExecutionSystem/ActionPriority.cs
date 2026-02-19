using System;


public struct ActionPriority : IComparable<ActionPriority>
{
    public int Speed;
    public int RegisterOrder;

    public int CompareTo(ActionPriority other)
    {
        int speedCompare = Speed.CompareTo(other.Speed);

        if (speedCompare != 0) return speedCompare;

        return RegisterOrder.CompareTo(other.RegisterOrder);
    }
}
