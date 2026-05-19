using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private List<Character> _members = new();
    public IReadOnlyList<Character> Members => _members;
}