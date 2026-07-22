using System.Collections.Generic;

public class CardResolver
{
    public List<DiceEntry> BuildDiceEntries(CardModel card, ActionInstance action,
        int characterId, ref int nextDiceId)
    {
        var entries = new List<DiceEntry>();

        foreach (var diceData in card.Dices)
        {
            int id = nextDiceId++;
            var handle = new DiceHandle(new CharacterHandle(characterId), id);
            var dice = new DiceRuntime(diceData, action);
            entries.Add(new DiceEntry(dice, handle));
        }

        return entries;
    }

    public bool CanUse(CardModel card, CharacterRuntime user)
    {
        return user.CurrentEnergy >= card.Cost;
    }

    public List<ICombatEvent> BuildCardEffects(CardModel card, CharacterRuntime user)
    {
        var events = new List<ICombatEvent>();
        // TODO: 나중에 카드 효과를 구현할 때, card.description을 파싱해서 이벤트를 생성하는 로직이 필요할 것이다.
        return events;
    }
}