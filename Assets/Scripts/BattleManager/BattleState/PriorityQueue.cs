using System;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private List<(TElement item, TPriority priority)> heap = new();
    public int Count => heap.Count;

    public void Enqueue(TElement item, TPriority priority)
    {
        heap.Add((item, priority));
        HeapifyUp(heap.Count - 1);
    }

    public TElement Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("PriorityQueue is empty.");

        var root = heap[0].item;

        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count - 1);

        if (heap.Count > 0)
        {
            HeapifyDown(0);
        }

        return root;
    }

    void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (heap[index].priority.CompareTo(heap[parent].priority) <= 0) break;

            (heap[index], heap[parent]) = (heap[parent], heap[index]);
            index = parent;
        }
    }

    void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left <= lastIndex && heap[left].priority.CompareTo(heap[smallest].priority) > 0) smallest = left;
            if (right <= lastIndex && heap[right].priority.CompareTo(heap[smallest].priority) > 0) smallest = right;
            if (smallest == index) break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }

    public void Clear()
    {
        while (heap.Count > 0)
        {
            heap.RemoveAt(heap.Count - 1);
        }
    }
}
