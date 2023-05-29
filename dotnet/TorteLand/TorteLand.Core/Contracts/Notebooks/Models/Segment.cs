namespace TorteLand.Core.Contracts.Notebooks.Models;

public record Segment(
    int Begin,
    int Border,
    int End)
{
    public Segment(int begin, int end)
        : this(begin, (begin + end) / 2, end)
    {}

    public override string ToString()
        => $"[{Begin};{Border};{End}]";
}