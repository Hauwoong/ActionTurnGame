
public interface IClashContext
{
    CharacterRuntime Attacker { get; }
    CharacterRuntime Defender { get; }
    bool IsCancelled { get; }
}