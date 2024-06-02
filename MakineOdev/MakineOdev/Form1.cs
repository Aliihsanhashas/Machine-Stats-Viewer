using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MakineOdev
{
    public partial class Form1 : Form
    {
        List<Makine> makineler = new List<Makine>();

        public Form1()
        {
            InitializeComponent();
            DosyadanOku();
            ComboBoxDoldur();
            button1.Click += button1_Click;
        }

        private void DosyadanOku()
        {
            string DosyaYolu = "C:\\Users\\aliih\\OneDrive\\Masaüstü\\OopLecture\\Makineler.txt";
            if (!File.Exists(DosyaYolu))
            {
                MessageBox.Show("Txt dosyası Bulamadı.");
                return;
            }

            string[] lines = File.ReadAllLines(DosyaYolu);
            Console.WriteLine($"Okunan satır sayısı: {lines.Length}");

            foreach (string line in lines.Skip(1)) // Baslıgı es geçiyoruz.
            {
                string[] bolum = line.Split(',');

                if (bolum.Length != 4)
                {
                    // 4 kısımdan olusmayan veriler varsa hata vericek.
                    Console.WriteLine($"Invalid line format: {line}");
                    continue;  // Veride Hata olmasına rağmen çalışmaya devam etmesi icin kullandık.
                }

                string makineNo = bolum[0];
                DateTime tarih;
                int hedefMiktar;
                int uretilenMiktar;

                //Gelen verilerin dogrulugunu kontrol ediyor.
                bool tarihD = DateTime.TryParseExact(bolum[1], "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih);
                bool hedefMiktariD = int.TryParse(bolum[2], out hedefMiktar);
                bool uretilenMiktarD = int.TryParse(bolum[3], out uretilenMiktar);
                //Eger dogruysa listenin içine atıcak.
                if (tarihD && hedefMiktariD && uretilenMiktarD)
                {
                    Makine veri = new Makine
                    {
                        makineNo = makineNo,
                        Tarih = tarih,
                        HedefMiktar = hedefMiktar,
                        UretilenMiktar = uretilenMiktar
                    };
                    makineler.Add(veri);
                }
                else
                {
                    Console.WriteLine($"Geçersiz Veri: {line}");
                }
            }

            Console.WriteLine($"Geçerli Veri: {makineler.Count}");
        }

        private void ComboBoxDoldur()
        {
            //ComboBox a makineleri ekleyen kısım
            List<string> makine = makineler.Select(m => m.makineNo).Distinct().ToList();
            if (makine.Count == 0)
            {
                MessageBox.Show("Geçerli veri bulunamadı");
                return;
            }

            comboBox1.Items.AddRange(makine.ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string secilenMakine = comboBox1.SelectedItem.ToString();
            DateTime baslangiTarihi = dateTimePicker1.Value.Date;
            DateTime bitisTarihi = dateTimePicker2.Value.Date;
            List<Makine> IstenilenVeri = makineler
                .Where(d => d.makineNo == secilenMakine && d.Tarih >= baslangiTarihi && d.Tarih <= bitisTarihi)
                .ToList();
            //Listeden verileri alıp gridview e ekleyen kısım.
            dataGridView1.DataSource = IstenilenVeri;
            chart1.Series.Clear();

            Series Hedeflenen = new Series("Hedef Miktar") { ChartType = SeriesChartType.Column };
            Series Uretilen = new Series("Uretilen Miktar") { ChartType = SeriesChartType.Column };

            //chartları ayarlayan kısım.
            foreach (Makine veri in IstenilenVeri)
            {
                Hedeflenen.Points.AddXY(veri.Tarih, veri.HedefMiktar);
                Uretilen.Points.AddXY(veri.Tarih, veri.UretilenMiktar);
            }

            chart1.Series.Add(Hedeflenen);
            chart1.Series.Add(Uretilen);
        }
    }
}
