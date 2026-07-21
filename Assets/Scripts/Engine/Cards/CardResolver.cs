using System.Collections.Generic;

public class CardResolver
{
    public List<DiceEntry> BuildDiceEntries(CardData card, ActionInstance action,
        int characterId, ref int nextDiceId)
    {
        var entries = new List<DiceEntry>();

        foreach (var diceData in card.dices)
        {
            int id = nextDiceId++;
            var handle = new DiceHandle(new CharacterHandle(characterId), id);
            var dice = new DiceRuntime(diceData, action);
            entries.Add(new DiceEntry(dice, handle));
        }

        return entries;
    }

    public bool CanUse(CardData card, CharacterRuntime user)
    {
        return user.CurrentEnergy >= card.cost;
    }

    public List<ICombatEvent> BuildCardEffects(CardData card, CharacterRuntime user)
    {
        var events = new List<ICombatEvent>();
        // TODO: 나중에 카드 효과를 구현할 때, card.description을 파싱해서 이벤트를 생성하는 로직이 필요할 것이다.
        return events;
    }
}