using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
namespace Prog1D
{
    public partial class Form2 : Form
    {
        Form1 frm1 = new Form1();
        IFirebaseConfig Config = new FirebaseConfig()
        {
            AuthSecret = "XdYfk7aaJzV9iru1CT5E7BEOlwfrfBEoH3uV1y1I",
            BasePath = "https://fir-for-1137f.firebaseio.com/"
        };
        IFirebaseClient client;
        public Form2()
        {
            InitializeComponent();
           
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        string dosya_adı = "";
        bool timeCalis = false;
        private async void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("İnterneti açınız", "uyarı", MessageBoxButtons.OK);
           
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            client = new FireSharp.FirebaseClient(Config);
            dosya_adı = openFile.FileName;
          
            

            if (dosya_adı != "")
            {
                var datam = new dataHatırla
                {

                    hatırla = dosya_adı
                };

                SetResponse response = await client.SetTaskAsync("dosyamAdı/" + "dosyam1", datam);
                dataHatırla result = response.ResultAs<dataHatırla>();

                label4.Text = dosya_adı;
                label2.Text = "Yüklenen dosya: " + dosya_adı;
                label3.Text = "DOSYANIZ YÜKLENİYOR...";
                timeCalis = true;
                this.progressBar1.Value = 100;
                this.timer1.Start();
                MessageBox.Show("DOSYA DOĞRULANDI", "DOSYA DOĞRULAMA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
            else
            {
                MessageBox.Show("DOSYA BULUNAMADI...","DOSYA DOĞRULAMA",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timeCalis==true)
            {
                this.progressBar1.Increment(1);
                if (progressBar1.Value >= 100)
                {
                    this.timer1.Stop();
                    
                }
            }
          
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            label4.Text = "İNCELENİYOR...";
            if (!dosya_adı.Contains("docx"))
            {
                MessageBox.Show("Hatalı dosya .docx uzantılı bir dosya seçmelisiniz", "Bilgilendirme Penceresi");
            }
            else
            {
                frm1 = new Form1();
                frm1.path = dosya_adı;
                frm1.Show();
            }
            
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 frm3 = new Form3();
            frm3.Show();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(Config);
            var rs = await client.GetTaskAsync("dosyamAdı/dosyam1");
            dataHatırla resulta = rs.ResultAs<dataHatırla>();
            label4.Text = "YÜKLENEN DOSYA: " + resulta.hatırla;
        }
    }
}
