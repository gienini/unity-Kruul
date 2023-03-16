using System.Collections.Generic;

public class SortedSetComparer : IEqualityComparer<SortedSet<int>>
{
    public bool Equals(SortedSet<int> x, SortedSet<int> y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.SetEquals(y);
    }

    public int GetHashCode(SortedSet<int> obj)
    {
        int hash = 0;
        foreach (int item in obj)
        {
            hash ^= item.GetHashCode();
        }

        return hash;
    }
}