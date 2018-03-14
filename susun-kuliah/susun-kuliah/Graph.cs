using System;
using System.Collections;
using System.Collections.Generic;

namespace susunkuliah
{
    public class Graph
    {
        Dictionary<string, List<string>> adj = new Dictionary<string, List<string>();
        List<>
        int cur_timestamp = 0;

        public Graph()
        {
            addEdge("C1", "C2");
            addEdge("C2", "C3");
            addEdge("C3", "C1");
        }

        public void addEdge(string key, string value)
        {
            if (adj.ContainsKey(key)) {
                adj[key].Add(value);
            } else {
                adj.Add(key, new List<string> { value });
            }
        }

        public TopologicalSortDFS() {
            cur_timestamp = 0;

        }
    }
}
