namespace EventLogAnalysis;

/// <summary>
/// I don't remember why I added this and probably should remove if not using.  Searching finds the possibly original source:
/// https://www.stevefenton.co.uk/blog/2021/10/introducing-an-async-pipeline-in-c/#the-pipeline-code
/// </summary>
public static class PipelineExtensions
{
    public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> fn)
    {
        return fn(input);
    }

    public static TOut Pipe<TIn, TParam1, TOut>(this TIn input, Func<TIn, TParam1, TOut> fn, TParam1 p1)
    {
        return fn(input, p1);
    }

    public static TOut Pipe<TIn, TParam1, TParam2, TOut>(this TIn input, Func<TIn, TParam1, TParam2, TOut> fn, TParam1 p1, TParam2 p2)
    {
        return fn(input, p1, p2);
    }
}