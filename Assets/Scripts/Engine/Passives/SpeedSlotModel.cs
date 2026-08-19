
public class SpeedSlotModel : PassiveModel
{
    public override PassiveEffect CreateEffect(CharacterRuntime owner)
        => new SpeedSlotPassive(owner);
}
