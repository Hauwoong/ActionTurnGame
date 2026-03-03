using UnityEngine;

public readonly struct CharacterHandle
{
    private readonly int CharacterId;
    private readonly string OwnerTag;

    public CharacterHandle(int characterId, string ownerTag)
    {
        CharacterId = characterId;
        OwnerTag = ownerTag;
    }
}
