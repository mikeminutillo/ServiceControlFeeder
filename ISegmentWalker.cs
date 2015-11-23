using System.Collections.Generic;

namespace ConsoleApplication1
{
    public interface ISegmentWalker
    {
        void Walk(IEnumerable<Segment> segments);
    }
}