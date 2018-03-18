using System;
using System.Collections;
using System.Collections.Generic;

namespace susunkuliah
{
    public class Graph
    {
        Dictionary<string, List<string>> adj = new Dictionary<string, List<string>>();
        Dictionary<string, bool> visited = new Dictionary<string, bool>();
        List<string> start_vertices = new List<string>();
        List<string> solution = new List<string>();
        int cur_timestamp = 0;

        Dictionary<string, List<string>> preq;

        public Graph()
        {
            adj["C1"] = new List<string>();
            adj["C2"] = new List<string>();
            adj["C3"] = new List<string>();
            adj["C4"] = new List<string>();
            adj["C5"] = new List<string>();

            addEdge("C1", "C2");
            addEdge("C4", "C2");

            addEdge("C1", "C4");
            addEdge("C3", "C4");

            addEdge("C2", "C5");
            addEdge("C4", "C5");

            // addEdge("C2", "C3");
            // addEdge("C3", "C4");
            // adj["C4"] = new List<string>();

            start_vertices.Add("C3");
            start_vertices.Add("C1");

            // DFS
            TopologicalSortDFS();

            //Console.WriteLine("\n");

            // BFS
            //List<List<String>> ans = TopologicalSortBFS();

            //foreach (var x in ans) {
            //    foreach (var y in x) {
            //        Console.Write(y);
            //        Console.Write(" ");
            //    }
            //    Console.WriteLine("");
            //}
        }

        public void addEdge(string key, string value)
        {
            if (adj.ContainsKey(key)) {
                adj[key].Add(value);
            } else {
                adj.Add(key, new List<string> { value });
            }
        }

        public void _TopologicalSortDFS(string vertice) {
            cur_timestamp++;
            visited[vertice] = true;
            foreach (string neighbour in adj[vertice]) {
                if (!visited.ContainsKey(neighbour)) {
                    _TopologicalSortDFS(neighbour);
                }
            }
            cur_timestamp++;
            Console.Write(vertice);
            Console.Write(" ");
            Console.WriteLine(cur_timestamp);
            solution.Add(vertice);
        }

        public List<string> TopologicalSortDFS()
        {
            cur_timestamp = 0;
            solution = new List<string>();
            visited = new Dictionary<string, bool>();
            foreach (string vertice in start_vertices)
            {
                _TopologicalSortDFS(vertice);
            }
            int len = solution.Count;
            for (int i = 0; i < len/2; i++)
            {
                string temp = solution[i];
                solution[i] = solution[len - i - 1];
                solution[len - i - 1] = temp;
            }
            return solution;
        }

        public List<List <string> > TopologicalSortBFS() {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            List<string> deliminator = new List<string>();
            List<List <string> > output = new List<List <string> >();
            bool end = false;
            bool continuity = true;

            counter = fillCounter();

            while (!end && continuity) {
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

                if (!continuity && !end) {
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

        public Dictionary<string, int> fillCounter() {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            foreach (var item in adj) {
                if (!counter.ContainsKey(item.Key)) {
                    counter.Add(item.Key, 0);
                }
                for (int i = 0; i < item.Value.Count; i++) {
                    if (counter.ContainsKey(item.Value[i])) {
                        counter[item.Value[i]]++;
                    }
                    else {
                        counter.Add(item.Value[i], 1);
                    }
                }
            }
            return counter;          
        }

    }
}
