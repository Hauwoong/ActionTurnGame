using UnityEngine;

public class ResolutionSystem
{
    public void Resolve(BattleState state)
    {
        ResolveBouts(state);
        ResolveUnopposed(state);
    }

    void ResolveBouts(BattleState state)
    {
        foreach (var bout in state.ActiveBouts)
        {
            ClashResolver.Resolve(bout);
        }
    }
}
