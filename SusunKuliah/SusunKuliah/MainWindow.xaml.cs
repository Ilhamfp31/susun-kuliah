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
using System.Windows.Media;


namespace SusunKuliah
{
    // Class course untuk merepresentasikan mata kuliah pada persoalan
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

    // Class TopologicalSort sebagai algoritma utama
    class TopologicalSort
    {
        // courses berisikan matakuliah serta prerequisitenya
        Dictionary<string, Course> courses;

        // Konstruktor class
        public TopologicalSort(Dictionary<string, Course> _courses)
        {
            courses = _courses;
        }

        // Pembuatan solusi menggunakan pendekatan DFS
        public List<Tuple<string, int, int>> GenerateSolutionDFS()
        {
            // Membuat representasi adjacency list dari courses
            Dictionary<string, List<string>> adj_list = GenerateAdjList();

            // Membuat dictionary untuk menandakan node yang sudah dikunjungi
            Dictionary<string, bool> visited = new Dictionary<string, bool>();

            // Membuat list course yang tidak punya prerequisite sebagai matakulaih awal
            List<string> start_courses = GetStartCourses();

            // Membuat list berisikan tahapan solusi
            List<Tuple<string, int, int>> solution = new List<Tuple<string, int, int>>();

            // Membuat list berisikan urutan dari topological sort (berdasarkan finish time)
            List<Tuple<string, int, int>> semester = new List<Tuple<string, int, int>>();

            // Inisialisasi timestamp
            int cur_timestamp = 0;

            // Fungsi helper
            void _GenerateSolutionDFS(string course_name)
            {
                cur_timestamp++;

                int time_start = cur_timestamp;
                solution.Add(new Tuple<string, int, int>(course_name, time_start, -1));

                visited[course_name] = true;

                // Mengunjungi seluruh neighbour dari course_name
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

            // Memanggil helper function untuk setiap course pada start_courses
            foreach (string course_name in start_courses)
            {
                _GenerateSolutionDFS(course_name);
            }

            // Membalik urutan list semester untuk mendapatkan list yang terurut menurun (sesuai finish time)
            semester.Reverse();

            // Menghitung kapan (dalam semester) setiap mata kuliah sebaiknya diambil
            foreach (var sol in semester)
            {
                string course_name = sol.Item1;
                // Mendapatkan semester tertinggi yang dimiliki seluruh prerequisite course_name
                int highest_semester = GetHighestPrerequisiteSemester(course_name);
                courses[course_name].semester = highest_semester + 1;
                courses[course_name].time_begin = sol.Item2;
                courses[course_name].time_finish = sol.Item3;
            }

            return solution;
        }

        // Pembuatan solusi menggunakan pendekatan BFS
        public List<List<string>> GenerateSolutionBFS()
        {
            // Membuat representasi adjacency list dari courses
            Dictionary<string, List<string>> adj = GenerateAdjList();

            // Membuat dictionary untuk merepresentasikan jumlah course
            // prerequisite yang belum diambil tiap mata kuliah
            Dictionary<string, int> counter = new Dictionary<string, int>();

            // Membuat list yang menampung mata kuliah yang diambil pada semester
            List<string> deliminator = new List<string>();

            // Membuat list menampung solusi
            List<List<string>> solution = new List<List<string>>();

            bool end = false;
            bool continuity = true;

            // Menghitung jumlah prerequisite tiap matakuliah
            counter = fillCounter(adj);

            // Melakukan looping hingga matakuliah habis diambil
            while (!end && continuity)
            {
                end = true;
                continuity = false;
                deliminator.Clear();

                // Melakukan traversal terhadap counter
                foreach (var item in counter)
                {
                    // Jika mata kuliah tidak memiliki prerequisite, maka akan
                    // ditambahkan ke deliminator
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
                    // ERROR
                }
                else
                {
                    // Mengurangi jumlah prerequisite seluruh matkul yang mempunyai
                    // prerequsite mata kulaih yang ada di deliminator
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

                    // Menambahkan deliminator ke solusi
                    solution.Add(new List<string>(deliminator));
                }
            }
            
            return solution;
        }

        // Menghasilkan dictionary berisikan jumlah prerequsite dari seluruh matkul
        public Dictionary<string, int> fillCounter(Dictionary<string, List<string>> adj)
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

        // Membuat representasi adjacency list dari courses
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

        // Membuat list course tanpa prerequisite
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

        // Menghasilkan semester tertinggi dari seluruh prerequisite sebuah matkul
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


    // Window utama visualizer
    public partial class MainWindow : Window
    {
        // Buat OpenFileDialog
        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

        // Check file sudah dipilih atau belum
        Nullable<bool> result;

        // Counter gambar
        int counterGambarTampil = 0;

        // Set PesanSemester
        string PesanSemester = "";

        // List Gambar
        List<BitmapImage> listGambar = new List<BitmapImage>();


        public MainWindow()
        {

            SplashScreen splashScreen = new SplashScreen("SusunKuliahIcon.ico");
            splashScreen.Show(true);
            System.Threading.Thread.Sleep(500);
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                Assembly thisAssembly = Assembly.GetEntryAssembly();
                String resourceName = string.Format("{0}.{1}.dll",thisAssembly.EntryPoint.DeclaringType.Namespace,new AssemblyName(args.Name).Name);

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {

                    Byte[] assemblyData = new Byte[stream.Length];

                    stream.Read(assemblyData, 0, assemblyData.Length);

                    return Assembly.Load(assemblyData);

                }

            };
            this.Background = new System.Windows.Media.ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/bg1.jpg")));

        }

        // Button untuk ambil file
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
            // Inisialisasi map kuliah
            Dictionary<string, List<string>> mapKuliah = new Dictionary<string, List<string>>();
            Dictionary<string, Course> courses = new Dictionary<string, Course>();

            // Cek file sudah didapat atau belum
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
                foreach (KeyValuePair<string, List<string>> entry in mapKuliah)
                {
                    for (int i = 0; i < entry.Value.Count; ++i)
                    {
                        graph.AddEdge(entry.Value[i], entry.Key);
                    }
                }
                Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                renderer.CalculateLayout();
                int width = 250;
                Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                renderer.Render(bitmap);

                //Masukan gambar graph awal ke dalam list gambar
                listGambar.Add(BitmapToImageSource(bitmap));

                //Tampilkan gambar pertama
                ImageBox1.Source = listGambar[counterGambarTampil];

                TopologicalSort topo = new TopologicalSort(courses);

                // Mengecek pilihan radio button (DFS / BFS)
                if (rdBtnDFS.IsChecked == true) // Saat DFS dipilih
                {
                    // Generate solusi menggunakan topological sort DFS
                    List<Tuple<string, int, int>> dfs = topo.GenerateSolutionDFS();

                    // Membuat step-by-step pemrosesan graph
                    foreach (Tuple<string, int, int> entry in dfs)
                    {
                        if (entry.Item3 == -1) // Saat node yang dikunjungi baru masuk/start
                        {
                            // Set warna node yang baru dikunjungi dengan warna merah serta membarikan timestamp start
                            graph.FindNode(entry.Item1).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                            graph.FindNode(entry.Item1).LabelText = graph.FindNode(entry.Item1).LabelText + " " + entry.Item2 + "/";
                        }
                        else // Saat node yang dikunjungi sudah selesai / finish
                        {
                            // Set warna node yang selesai dikunjungi dengan warna biru serta membarikan timestamp start dan finish
                            graph.FindNode(entry.Item1).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
                            graph.FindNode(entry.Item1).LabelText = graph.FindNode(entry.Item1).LabelText + entry.Item3;
                        }

                        // Membuat renderer untuk graph
                        renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                        renderer.CalculateLayout();
                        width = 250;
                        bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                        renderer.Render(bitmap);

                        // Memasukan rendered graph ke list graph untuk keperluan visualisasi step-by-step
                        listGambar.Add(BitmapToImageSource(bitmap));
                    }

                    // Generate pesan untuk visualisasi informasi matakuliah yang diambil per semester
                    int counterSemester = 0;
                    int maxSemester = 0;

                    // Mencari semester maksimum pada seluruh mata kuliah
                    foreach (var x in courses)
                    {
                        if (x.Value.semester > maxSemester)
                        {
                            maxSemester = x.Value.semester;
                        }
                    }

                    // Membuat dan mengisi list matakuliah berdasarkan semester
                    List<List<string>> semesterList = new List<List<string>>();
                    for (int i = 0; i < maxSemester; i++) {
                        semesterList.Add(new List<string>());
                    }

                    foreach (var x in courses)
                    {
                        semesterList[x.Value.semester - 1].Add(x.Key);
                    }

                    // Generate string informasi matakuliah per semester
                    foreach (var x in semesterList)
                    {
                        counterSemester++;
                        PesanSemester += "Semester " + counterSemester + ": ";
                        for (int i = 0; i < x.Count; ++i)
                        {
                            PesanSemester += x[i];
                            if (i != x.Count - 1)
                            {
                                PesanSemester += ", ";
                            }
                        }
                        PesanSemester += " \n";
                    }

                }
                else if (rdBtnBFS.IsChecked == true) // Saat BFS dipilih
                {
                    // Generate solusi menggunakan pendekatan topological sort BFS
                    List<List<string>> bfs = topo.GenerateSolutionBFS();

                    // Membuat visualisasi graph secara step-by-step
                    foreach (var x in bfs)
                    {
                        // Mewarnai node yang sudah dipilih dengan warna biru
                        foreach (var entry in x)
                        {
                            graph.FindNode(entry).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
                        }

                        // Membuat renderer untuk graph
                        renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                        renderer.CalculateLayout();
                        width = 250;
                        bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                        renderer.Render(bitmap);

                        // Memasukan gambar ke dalam list gambar
                        listGambar.Add(BitmapToImageSource(bitmap));
                    }

                    // Generate string berisi informasi matakuliah yang diambil tiap semester
                    int counterSemester = 0;
                    foreach (var x in bfs)
                    {
                        counterSemester++;
                        PesanSemester += "Semester " + counterSemester + ": ";
                        for (int i = 0; i < x.Count; ++i) {
                            PesanSemester += x[i];
                            if (i != x.Count - 1)
                            {
                                PesanSemester += ", ";
                            }
                        }
                        PesanSemester += " \n";
                    }

                }
                else
                {
                    // Jika tipe sort belum dipilih munculkan pesan error.
                    System.Windows.MessageBox.Show("Please choose one type of topological sort (DFS/BFS).");
                }

            }
            else
            {
                // Jika file belum didapat munculkan pesan error.
                System.Windows.MessageBox.Show("Please enter the input file.");
            }


        }

        //Button tunjukan selanjutnya
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (counterGambarTampil == listGambar.Count - 1) {
                ImageBox1.Height = 0;
                ImageBox1.Width = 0;
                TextBox2.Opacity = 100.0;
                TextBox2.Text = PesanSemester;
                TextBox2.FontSize = 20;
            } else
            {

                int temp = counterGambarTampil + 1;
                counterGambarTampil = Math.Min(listGambar.Count - 1, temp);
                ImageBox1.Source = (listGambar[counterGambarTampil]);

            }
        }


        // Ubah bitmap ke bitmapimage agar dapat ditampilkan
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


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("College courses scheduler app for IF2211 - Institut Teknologi Bandung \n\nby\nIlham Firdausi Putra 13516140\nYusuf Rahmat Pratama 13516062\nAhmad Izzan 13516116\n ");
        }
    }

}
