namespace VerifyTests;

internal class PathNode
{
    public required string Name { get; set; }
    public List<PathNode> Children { get; set; } = [];
}