using System.Collections;

namespace VerifyTests;

internal class NuspecScrubbers : ICollection<INuspecScrubber>, IReadOnlyCollection<INuspecScrubber>
{
    private readonly ICollection<INuspecScrubber> _scrubbers = new List<INuspecScrubber>();

    public static string ContextKey { get; } = "Nupkg:NuspecScrubbers";

    #region ICollection<INuspecScrubber> implementation
    public int Count => _scrubbers.Count;

    public bool IsReadOnly => _scrubbers.IsReadOnly;

    public void Add(INuspecScrubber item)
    {
        _scrubbers.Add(item);
    }

    public void Clear()
    {
        _scrubbers.Clear();
    }

    public bool Contains(INuspecScrubber item)
    {
        return _scrubbers.Contains(item);
    }

    public void CopyTo(INuspecScrubber[] array, int arrayIndex)
    {
        _scrubbers.CopyTo(array, arrayIndex);
    }

    public IEnumerator<INuspecScrubber> GetEnumerator()
    {
        return _scrubbers.GetEnumerator();
    }

    public bool Remove(INuspecScrubber item)
    {
        return _scrubbers.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_scrubbers).GetEnumerator();
    }
    #endregion
}
