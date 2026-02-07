
using UnityEngine;
using System.Collections.Generic;

public class ClashGroup : MonoBehaviour
{
    public List<ActionSlot> slots = new();

    public void Add(ActionSlot slot)
    {
        slots.Add(slot);
    }

    public void ResolveWithIntercept()
    {

    }
}
