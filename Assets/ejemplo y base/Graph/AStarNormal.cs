using System;
using System.Collections.Generic;
using System.Linq;
using U = Utility;

public class AStarNormal<Node> where Node : class
{
    public class Arc
    {
        public Node endpoint;
        public float cost;
        public Arc(Node ep, float c)
        {
            endpoint = ep;
            cost = c;
        }
    }

    public static IEnumerable<Node> Run
    (
        Node from,
        Node to,
        Func<Node, Node, float> h,
        Func<Node, bool> satisfies,
        Func<Node, IEnumerable<Arc>> expand
    )
    {
        var initialState = new AStarState<Node>();
        initialState.open.Add(from);
        initialState.gs[from] = 0;
        initialState.fs[from] = h(from, to);
        initialState.previous[from] = null;
        initialState.current = from;

        var state = initialState;
        while (state.open.Count > 0 && !state.finished)
        {
            state = state.Clone();

            var candidate = state.open.OrderBy(x => state.fs[x]).First();
            state.current = candidate;

            if (satisfies(candidate))
            {
                U.Log("SATISFIED");
                state.finished = true;
            }
            else
            {
                state.open.Remove(candidate);
                state.closed.Add(candidate);
                var neighbours = expand(candidate);
                if (neighbours == null || !neighbours.Any())
                    continue;

                var gCandidate = state.gs[candidate];

                foreach (var ne in neighbours)
                {
                    if (ne.endpoint.In(state.closed))
                        continue;

                    var gNeighbour = gCandidate + ne.cost;
                    state.open.Add(ne.endpoint);

                    if (gNeighbour > state.gs.DefaultGet(ne.endpoint, () => gNeighbour))
                        continue;

                    state.previous[ne.endpoint] = candidate;
                    state.gs[ne.endpoint] = gNeighbour;
                    state.fs[ne.endpoint] = gNeighbour + h(ne.endpoint, to);
                }
            }
        }


        //if (!state.finished)
        //    return null;

        var seq =
            U.Generate(state.current, n => state.previous[n])
            .TakeWhile(n => n != null)
            .Reverse();

        return seq;
    }
}