using System;
using System.Collections;
using System.Collections.Generic;

namespace susunkuliah
{
    public class Graph
    {
        Dictionary<string, List<string>> adj = new Dictionary<string, List<string>();
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

        public List<List <string> > TopologicalSortBFS() {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            List<string> deliminator = new List<string>();
            List<List <string> > output = new List<List <string> >();
            bool end = false;
            bool continuity = true;

            counter = fillCounter();

            while (!end) && continuity {
                end = true;
                continuity = false;
                foreach (var item in counter) {
                    if (item.Value == 0) {
                        continuity = true;
                        deliminator.Add(item.Key);
                    }
                    else {
                        end = false;
                    }
                }

                if (!continuity) && (!end) {
                    //ERROR
                }
                else {
                    foreach (var vertice in deliminator) {
                        if (counter.ContainsKey(vertice)) {
                            foreach (var target in adj[vertice]) {
                                counter[target]--;
                            }
                            counter.Remove(vertice);
                        }
                    }

                    output.Add(new List<string> (deliminator));
                }
            }
            return output;
        }

        private Dictionary<string, int> fillCounter() {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            foreach (var item in adj) {
                if (!counter.ContainsKey(item.Key)) {
                    counter.Add(item.key, 0);
                }
                for (int i = 0; i < item.Value.Count; i++) {
                    if counter.ContainsKey(item.Value[i]) {
                        counter[item.Value[i]]++;
                    }
                    else {
                        counter.Add(item.key, 1);
                    }
                }
            }
            return counter;          
        }

    }
}
