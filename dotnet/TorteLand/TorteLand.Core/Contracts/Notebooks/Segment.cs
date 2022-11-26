namespace TorteLand.Core.Contracts.Notebooks;

public record Segment(
    int Begin,
    int Border,
    int End)
{
    public override string ToString()
        => $"[{Begin};{Border};{End}]";
}