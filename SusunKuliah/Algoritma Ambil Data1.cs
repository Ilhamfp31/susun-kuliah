 using (StreamReader sr = new StreamReader(TextBox1.Text))
                {
                    string kodeKuliah = "";
                    while ((kodeKuliah = sr.ReadLine()) != null)
                    {
                        string kodeKuliahMasukan = "";
                        string kodeKuliahDitunjuk = "";
                        bool sudahDapatAwal = false;
                        for (int i = 0; i < kodeKuliah.Length; ++i)
                        {
                            if (!sudahDapatAwal)
                            {
                                if (kodeKuliah[i] == ',')
                                {
                                    kodeKuliahDitunjuk = kodeKuliahMasukan;
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

                                    if (mapKuliah.ContainsKey(kodeKuliahMasukan))
                                    {
                                        mapKuliah[kodeKuliahMasukan].Add(kodeKuliahDitunjuk);
                                    }
                                    else
                                    {
                                        mapKuliah.Add(kodeKuliahMasukan, new List<string> { kodeKuliahDitunjuk });
                                    }
                                }
                                else
                                {
                                    kodeKuliahMasukan += kodeKuliah[i];
                                }
                            }
                        }
                    }
                }