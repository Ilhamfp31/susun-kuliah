using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SusunKuliah
{

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
                                if (kodeKuliah[i] == ',')
                                {
                                    kodeKuliahDitunjuk = Regex.Replace(kodeKuliahMasukan, @"\s+", String.Empty);
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
                    for(int i=0; i< entry.Value.Count; ++i)
                    {
                        graph.AddEdge(entry.Value[i], entry.Key);
                        //Console.WriteLine("Key = {0}, Value = {1}", entry.Key, entry.Value[i]);
                    }

                    //Gambar graph awal step by step
                    Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                    renderer.CalculateLayout();
                    int width = 250;
                    Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), PixelFormat.Format32bppPArgb);
                    renderer.Render(bitmap);
                    //Masukan gambar ke dalam list gambar
                    listGambar.Add(BitmapToImageSource(bitmap));

                }

                //Tampilkan gambar pertama
                ImageBox1.Source = listGambar[counterGambarTampil];
                
                //Cek pilihan sort
                if (rdBtnDFS.IsChecked == true)
                {
                    //Mulai DFS
                    Stack<string> s = new Stack<string>();



                    while(s.Count != 0)
                    {
                        //Lakukan apa gitu kek DFS
                    }


                }
                else if (rdBtnBFS.IsChecked == true)
                {
                    //Mulai BFS
                    
                    Queue < string > q = new Queue<string>();

                    foreach (KeyValuePair<string, List<string>> entry in mapKuliah)
                    {
                        // Warnai node yang dikunjungi dengan
                        // graph.FindNode("A").Attr.FillColor =Microsoft.Msagl.Drawing.Color.Red;

                        while (q.Count != 0)
                        {
                            //Lakukan apa gitu kek BFS
                        }

                        //Gambar graph hasil BFS step by step
                        //Gambar graph awal step by step
                        Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
                        renderer.CalculateLayout();
                        int width = 250;
                        Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)), PixelFormat.Format32bppPArgb);
                        renderer.Render(bitmap);
                        //Masukan gambar ke dalam list gambar
                        listGambar.Add(BitmapToImageSource(bitmap));

                        // Warnai kembali node yang dikunjungi tadi ke warna awalnya
                        // graph.FindNode("A").Attr.FillColor =Microsoft.Msagl.Drawing.Color.White;
                    }

                }
                else
                {
                    //Jika tipe sort belum dipilih munculkan pesan error.
                    System.Windows.MessageBox.Show("Mohon pilih salah satu tipe topological sort (DFS/BFS).");
                }

            } else
            {
                //Jika file belum didapat munculkan pesan error.
                System.Windows.MessageBox.Show("Mohon masukkan file input.");
            }


        }

        //Button tunjukan selanjutnya
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int temp = counterGambarTampil + 1;
            counterGambarTampil = Math.Min(listGambar.Count - 1 , temp);
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
