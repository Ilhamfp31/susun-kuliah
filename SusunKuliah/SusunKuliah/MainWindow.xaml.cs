using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace SusunKuliah
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
            List<Tuple<string, int, int>> semester = new List<Tuple<string, int, int>>();
            int cur_timestamp = 0;

            void _GenerateSolutionDFS(string course_name)
            {
                cur_timestamp++;
                int time_start = cur_timestamp;
                solution.Add(new Tuple<string, int, int>(course_name, time_start, -1));
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
                semester.Add(new Tuple<string, int, int>(course_name, time_start, time_finish));
                solution.Add(new Tuple<string, int, int>(course_name, time_start, time_finish));
            }

            foreach (string course_name in start_courses)
            {
                _GenerateSolutionDFS(course_name);
            }

            semester.Reverse();

            foreach (var sol in semester)
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

        public int GetHighestPrerequisiteSemester(string course_name)
        {
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

    public class GraphNew
    {
        Dictionary<string, Course> courses;

        public GraphNew()
        {
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

        public void Run()
        {
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
            if (adj.ContainsKey(key))
            {
                adj[key].Add(value);
            }
            else
            {
                adj.Add(key, new List<string> { value });
            }
        }

        public void _TopologicalSortDFS(string vertice)
        {
            cur_timestamp++;
            visited[vertice] = true;
            foreach (string neighbour in adj[vertice])
            {
                if (!visited.ContainsKey(neighbour))
                {
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
            for (int i = 0; i < len / 2; i++)
            {
                string temp = solution[i];
                solution[i] = solution[len - i - 1];
                solution[len - i - 1] = temp;
            }
            return solution;
        }

        public List<List<string>> TopologicalSortBFS()
        {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            List<string> deliminator = new List<string>();
            List<List<string>> output = new List<List<string>>();
            bool end = false;
            bool continuity = true;

            counter = fillCounter();

            while (!end && continuity)
            {
                end = true;
                continuity = false;
                foreach (var item in counter)
                {
                    if (item.Value == 0)
                    {
                        continuity = true;
                        deliminator.Add(item.Key);
                    }
                    else
                    {
                        end = false;
                    }
                }

                if (!continuity && !end)
                {
                    //ERROR
                }
                else
                {
                    foreach (var vertice in deliminator)
                    {
                        if (counter.ContainsKey(vertice))
                        {
                            foreach (var target in adj[vertice])
                            {
                                counter[target]--;
                            }
                            counter.Remove(vertice);
                        }
                    }

                    output.Add(new List<string>(deliminator));
                }
            }
            return output;
        }

        public Dictionary<string, int> fillCounter()
        {
            Dictionary<string, int> counter = new Dictionary<string, int>();
            foreach (var item in adj)
            {
                if (!counter.ContainsKey(item.Key))
                {
                    counter.Add(item.Key, 0);
                }
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (counter.ContainsKey(item.Value[i]))
                    {
                        counter[item.Value[i]]++;
                    }
                    else
                    {
                        counter.Add(item.Value[i], 1);
                    }
                }
            }
            return counter;
        }

    }

    public partial class MainWindow : Window
    {
        // Buat OpenFileDialog
        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

        //Check file sudah dipilih atau belum
        Nullable<bool> result;

        //Counter gambar
        int counterGambarTampil = 0;

        //List Gambar
        List<BitmapImage> listGambar = new List<BitmapImage>();

        public MainWindow()
        {
            InitializeComponent();
        }

        //Button untuk ambil file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Set filter untuk file extension dan default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files (*.txt)|*.txt";


            // Munculkan OpenFileDialog dengan memanggil method ShowDialog  
            result = dlg.ShowDialog();

            // Ambil nama file yang dipilih dan display di TextBox1
            if (result == true)
            {
                string filename = dlg.FileName;
                TextBox1.Text = filename;
            }
        }

        //Button untuk susun kuliah
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Inisialisasi map kuliah
            Dictionary<string, List<string>> mapKuliah = new Dictionary<string, List<string>>();
            Dictionary<string, Course> courses = new Dictionary<string, Course>();

            //Cek file sudah didapat atau belum
            if (result == true)
            {
                //Jika file sudah didapat, baca.
                using (StreamReader sr = new StreamReader(TextBox1.Text))
                {
                    string kodeKuliah = string.Empty;
                    while ((kodeKuliah = sr.ReadLine()) != null)
                    {
                        string kodeKuliahMasukan = string.Empty;
                        string kodeKuliahDitunjuk = string.Empty;
                        bool sudahDapatAwal = false;
                        for (int i = 0; i < kodeKuliah.Length; ++i)
                        {
                            if (!sudahDapatAwal)
                            {
                                if (kodeKuliah[i] == ',' || kodeKuliah[i] == '.')
                                {
                                    kodeKuliahDitunjuk = Regex.Replace(kodeKuliahMasukan, @"\s+", String.Empty);
                                    courses[kodeKuliahDitunjuk] = new Course(kodeKuliahDitunjuk);
                                    kodeKuliahMasukan = string.Empty;
                                    sudahDapatAwal = true;
                                }
                                else
                                {
                                    kodeKuliahMasukan += kodeKuliah[i];
                                }
                            }
                            else
                            {
                                if (kodeKuliah[i] == ',' || kodeKuliah[i] == '.')
                                {
                                    kodeKuliahMasukan = Regex.Replace(kodeKuliahMasukan, @"\s+", String.Empty);
                                    courses[kodeKuliahDitunjuk].AddPrerequisite(kodeKuliahMasukan);
                                    if (mapKuliah.ContainsKey(kodeKuliahDitunjuk))
                                    {
                                        mapKuliah[kodeKuliahDitunjuk].Add(kodeKuliahMasukan);
                                    }
                                    else
                                    {
                                        mapKuliah.Add(kodeKuliahDitunjuk, new List<string> { kodeKuliahMasukan });
                                    }
                                    kodeKuliahMasukan = string.Empty;
                                }
                                else
                                {
                                    kodeKuliahMasukan += kodeKuliah[i];
                                }
                            }
                        }
                    }
                }

                //Graph untuk digambar.
                Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("");
                //Masukan graph yang sudah dibaca.
                foreach (KeyValuePair<string, List<string>> entry in mapKuliah)
                {
                    for (int i = 0; i < entry.Value.Count; ++i)
                    {
                        graph.AddEdge(entry.Value[i], entry.Key);
                        //Console.WriteLine("Key = {0}, Value = {1}", entry.Key, entry.Value[i]);
                    }
                }
                //Gambar graph awal step by step
                Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                renderer.CalculateLayout();
                int width = 250;
                Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), PixelFormat.Format32bppPArgb);
                renderer.Render(bitmap);
                //Masukan gambar ke dalam list gambar
                listGambar.Add(BitmapToImageSource(bitmap));

                //Tampilkan gambar pertama
                ImageBox1.Source = listGambar[counterGambarTampil];

                //Cek pilihan sort
                if (rdBtnDFS.IsChecked == true)
                {
                    TopologicalSort topo = new TopologicalSort(courses);
                    List<Tuple<string, int, int>> dfs = topo.GenerateSolutionDFS();

                    foreach (var x in dfs)
                    {
                        Console.WriteLine(x.ToString());
                    }

                    // dfs = dfs.OrderByDescending(t => t.Item2).ToList();


                    //Graph untuk digambar.
                    //Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("");
                    //Masukan graph yang sudah dibaca.

                    foreach (Tuple<string, int, int> entry in dfs)
                    {
                        if (entry.Item3 == -1)
                        {
                            graph.FindNode(entry.Item1).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                            graph.FindNode(entry.Item1).LabelText = graph.FindNode(entry.Item1).LabelText + " " + entry.Item2 + "/";
                            //Gambar graph awal step by step
                            renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                            renderer.CalculateLayout();
                            width = 250;
                            bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), PixelFormat.Format32bppPArgb);
                            renderer.Render(bitmap);
                            //Masukan gambar ke dalam list gambar
                            listGambar.Add(BitmapToImageSource(bitmap));
                        }
                        else
                        {
                            graph.FindNode(entry.Item1).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
                            graph.FindNode(entry.Item1).LabelText = graph.FindNode(entry.Item1).LabelText + entry.Item3;
                            //Gambar graph awal step by step
                            renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                            renderer.CalculateLayout();
                            width = 250;
                            bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), PixelFormat.Format32bppPArgb);
                            renderer.Render(bitmap);
                            //Masukan gambar ke dalam list gambar
                            listGambar.Add(BitmapToImageSource(bitmap));
                        }


                    }




                    ////Mulai DFS
                    //Stack<string> s = new Stack<string>();



                    //while (s.Count != 0)
                    //{
                    //    //Lakukan apa gitu kek DFS
                    //}


                }
                else if (rdBtnBFS.IsChecked == true)
                {
                    //Mulai BFS


                }
                else
                {
                    //Jika tipe sort belum dipilih munculkan pesan error.
                    System.Windows.MessageBox.Show("Mohon pilih salah satu tipe topological sort (DFS/BFS).");
                }

            }
            else
            {
                //Jika file belum didapat munculkan pesan error.
                System.Windows.MessageBox.Show("Mohon masukkan file input.");
            }


        }

        //Button tunjukan selanjutnya
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int temp = counterGambarTampil + 1;
            counterGambarTampil = Math.Min(listGambar.Count - 1, temp);
            ImageBox1.Source = (listGambar[counterGambarTampil]);
        }

        //Ubah bitmap ke bitmapimage agar dapat ditampilkan
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }

}
