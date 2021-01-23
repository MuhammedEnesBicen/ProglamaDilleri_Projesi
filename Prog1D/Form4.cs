using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
namespace Prog1D
{
    public partial class Form4 : Form
    {
        IFirebaseConfig Config = new FirebaseConfig()
        {
            AuthSecret = "XdYfk7aaJzV9iru1CT5E7BEOlwfrfBEoH3uV1y1I",
            BasePath = "https://fir-for-1137f.firebaseio.com/"
        };
        IFirebaseClient client;
        public Form4()
        {
            InitializeComponent();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 frm3 = new Form3();
            frm3.ShowDialog();
            Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(Config);
            MailMessage mesaj = new MailMessage();
            SmtpClient istemci = new SmtpClient();
            istemci.Credentials = new System.Net.NetworkCredential("furkan.demirel.056@gmail.com", "Şifre");
            //github da paylaşılacağı için şifre yeri yazılmamıştır
            istemci.Port = 587;
            istemci.Host = "smtp.gmail.com";
            istemci.EnableSsl = true;
            mesaj.To.Add(label7.Text);
            mesaj.From = new MailAddress(textBox1.Text);
            mesaj.Subject = textBox2.Text;
            mesaj.Body = textBox3.Text;

            var geri = new GeriDonus
            {

                kullanıcıMail = textBox1.Text,
                kullanıcıBaslik = textBox2.Text,
                kullanıcıkonu = textBox3.Text
            };

            SetResponse response = await client.SetTaskAsync("KullanıcıDonus/" + "Donus", geri);
            GeriDonus result = response.ResultAs<GeriDonus>();

            istemci.Send(mesaj);
           
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

       
    }
}
