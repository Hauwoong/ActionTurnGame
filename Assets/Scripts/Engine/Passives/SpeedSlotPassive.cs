public class SpeedSlotPassive : PassiveEffect
{
    public SpeedSlotPassive(CharacterRuntime owner)
         : base(owner, PassiveType.SpeedSlot) { }
    public override void OnTurnStart(TurnStartContext ctx)
    {
        int targetCount = Owner.BaseSpeedSlotCount + ctx.EmotionLevel;
        Owner.SetSpeedSlotCount(targetCount);
    }
}