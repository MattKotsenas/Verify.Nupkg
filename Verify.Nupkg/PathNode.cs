namespace VerifyTests;

internal class PathNode
{
    public required string Name { get; set; }
    public ICollection<PathNode> Children { get; set; } = [];
}