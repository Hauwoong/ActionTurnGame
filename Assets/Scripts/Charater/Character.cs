using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    [SerializeField] private  CharacterData _characterData;
    public CharacterData CharacterData => _characterData;
}
