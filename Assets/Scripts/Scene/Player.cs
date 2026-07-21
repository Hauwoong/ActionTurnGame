using System;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Character> _roster = new();
    [SerializeField] private List<Character> _selectedParty = new();

    public IReadOnlyList<Character> Roster => _roster;
    public IReadOnlyList<Character> SelectedParty => _selectedParty;
}