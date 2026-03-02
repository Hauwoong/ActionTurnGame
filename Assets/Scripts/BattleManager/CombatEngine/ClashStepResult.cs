using System;

public readonly struct ClashStepResult : IEquatable<ClashStepResult>
{
    public int RollA { get; }
    public int RollB { get; }
    public bool DestoryA { get; }
    public bool DestoryB { get; }

    public ClashStepResult(int rollA, int rollB, bool destoryA, bool destoryB)
    {
        RollA = rollA;
        RollB = rollB;
        DestoryA = destoryA;
        DestoryB = destoryB;
    }

    public bool Equals(ClashStepResult other)
    {
        return RollA == other.RollA &&
            RollB == other.RollB &&
            DestoryA == other.DestoryA &&
            DestoryB == other.DestoryB;
    }

    public override bool Equals(object obj)
    {
        return obj is ClashStepResult other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RollA, RollB, DestoryA, DestoryB);
    }

    public static bool operator == (ClashStepResult left, ClashStepResult right) => left.Equals(right);
    public static bool operator !=(ClashStepResult left, ClashStepResult right) => !(left == right);
}