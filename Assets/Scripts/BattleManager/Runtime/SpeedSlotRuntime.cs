
public class SpeedSlotRuntime  
{
    public SpeedSlot Slot { get; }

    public int MinSpeed { get; }
    public int MaxSpeed { get; }

    public int Speed { get; private set; }
    
    public bool Used { get; private set; }

    public SpeedSlotRuntime(SpeedSlot slot, int minSpeed, int maxSpeed)
    {
        Slot = slot;
        MinSpeed = minSpeed;
        MaxSpeed = maxSpeed;
    }

    public void Roll(IRng rng)
    {
        Speed = rng.Range(MinSpeed, MaxSpeed);
        Used = false;
    }

    public void MarkUse()
    {
        Used = true;
    }
}
