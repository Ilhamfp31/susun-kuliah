using System;
using System.Collections;
using System.Collections.Generic;

namespace susunkuliah
{
    public class Course
    {
        public string name;
        public List<string> prerequisites;
        public int semester;
        public int time_begin;
        public int time_finish;

        public Course(string _name)
        {
            name = _name;
            prerequisites = new List<string>();
            semester = 0;
        }

        public void AddPrerequisite(string preq)
        {
            prerequisites.Add(preq);
        }
    }

    class TopologicalSort
    {
        Dictionary<string, Course> courses;

        public TopologicalSort(Dictionary<string, Course> _courses)
        {
            courses = _courses;
        }

        public List<Tuple<string, int, int>> GenerateSolutionDFS()
        {
            Dictionary<string, List<string>> adj_list = GenerateAdjList();
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            List<string> start_courses = GetStartCourses();
            List<Tuple<string, int, int>> solution = new List<Tuple<string, int, int>>();
            int cur_timestamp = 0;

            void _GenerateSolutionDFS(string course_name)
            {
                cur_timestamp++;
                int time_start = cur_timestamp;
                visited[course_name] = true;
                foreach (string neighbour in adj_list[course_name])
                {
                    if (!visited.ContainsKey(neighbour))
                    {
                        _GenerateSolutionDFS(neighbour);
                    }
                }
                cur_timestamp++;
                int time_finish = cur_timestamp;
                solution.Add(new Tuple<string, int, int>(course_name, time_start, time_finish));
            }

            foreach (string course_name in start_courses)
            {
                _GenerateSolutionDFS(course_name);
            }

            solution.Reverse();

            foreach (var sol in solution)
            {
                string course_name = sol.Item1;
                int highest_semester = GetHighestPrerequisiteSemester(course_name);
                courses[course_name].semester = highest_semester + 1;
                courses[course_name].time_begin = sol.Item2;
                courses[course_name].time_finish = sol.Item3;
                Console.Write(course_name);
                Console.Write(" => Semester ");
                Console.WriteLine(courses[course_name].semester);
            }

            return solution;
        }

        public List<List <string> > GenerateSolutionBFS() {
            Dictionary<string, List<string>> adj = GenerateAdjList();
            Dictionary<string, int> counter = new Dictionary<string, int>();
            List<string> deliminator = new List<string>();
            List<List <string> > solution = new List<List <string> >();
            bool end = false;
            bool continuity = true;

            counter = fillCounter(adj);

            while (!end && continuity) {
                end = true;
                continuity = false;
                foreach (var item in counter) {
                    deliminator.Clear();
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

                    solution.Add(new List<string> (deliminator));
                }
            }
            return solution;
        }

        public Dictionary<string, int> fillCounter(Dictionary<string, List<string>> adj) {
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

        public Dictionary<string, List<string>> GenerateAdjList()
        {
            Dictionary<string, List<string>> adj_list = new Dictionary<string, List<string>>();

            foreach (string course_name in courses.Keys)
            {
                if (!adj_list.ContainsKey(course_name))
                {
                    adj_list[course_name] = new List<string>();
                }
                foreach (var preq in courses[course_name].prerequisites)
                {
                    if (adj_list.ContainsKey(preq))
                    {
                        adj_list[preq].Add(courses[course_name].name);
                    }
                    else
                    {
                        adj_list.Add(preq, new List<string> { courses[course_name].name });
                    }
                }
            }

            return adj_list;
        }

        public List<string> GetStartCourses()
        {
            List<string> start_courses = new List<string>();

            foreach (string course_name in courses.Keys)
            {
                if (courses[course_name].prerequisites.Count == 0)
                {
                    start_courses.Add(courses[course_name].name);
                }
            }

            return start_courses;
        }

        public int GetHighestPrerequisiteSemester(string course_name) {
            int mx = 0;

            foreach (var preq in courses[course_name].prerequisites)
            {
                if (courses[preq].semester > mx)
                {
                    mx = courses[preq].semester;
                }
            }

            return mx;
        }
    }

    public class GraphNew {
        Dictionary<string, Course> courses;

        public GraphNew() {
            courses = new Dictionary<string, Course>();

            // Hard code for testing purposes
            Course C1 = new Course("C1");
            C1.AddPrerequisite("C3");
            courses["C1"] = C1;
            //courses.Add(C1);

            Course C2 = new Course("C2");
            C2.AddPrerequisite("C1");
            C2.AddPrerequisite("C4");
            courses["C2"] = C2;
            //courses.Add(C2);

            Course C3 = new Course("C3");
            courses["C3"] = C3;
            //courses.Add(C3);

            Course C4 = new Course("C4");
            C4.AddPrerequisite("C1");
            C4.AddPrerequisite("C3");
            courses["C4"] = C4;
            //courses.Add(C4);

            Course C5 = new Course("C5");
            C5.AddPrerequisite("C2");
            C5.AddPrerequisite("C4");
            courses["C5"] = C5;
            //courses.Add(C5);
        }

        public void Run() {
            TopologicalSort youngG = new TopologicalSort(courses);
            List<Tuple<string, int, int>> solution = youngG.GenerateSolutionDFS();

            foreach (var x in solution)
            {
                Console.WriteLine(x.ToString());
            }
        }

    }

    public class Graph
    {
        Dictionary<string, List<string>> adj = new Dictionary<string, List<string>>();
        Dictionary<string, bool> visited = new Dictionary<string, bool>();
        List<string> start_vertices = new List<string>();
        List<string> solution = new List<string>();
        int cur_timestamp = 0;

        //Dictionary<string, List<string>> preq;

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
