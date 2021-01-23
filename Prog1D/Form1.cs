using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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
    public partial class Form1 : Form
    {
        List<KontrolList> kontrolLists;
        List<int> kaynakBelirtimleri;
        public string path;

        IFirebaseConfig Config = new FirebaseConfig()
        {
            AuthSecret = "XdYfk7aaJzV9iru1CT5E7BEOlwfrfBEoH3uV1y1I",
            BasePath = "https://fir-for-1137f.firebaseio.com/"
        };
        IFirebaseClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            

            client = new FireSharp.FirebaseClient(Config);
            //9–12
            //29 - 33
            //@"C:\Users\enesbicen\Desktop\abcd.docx"
            string strDoc = @path;



            WordprocessingDocument wordprocessingDocument =
                WordprocessingDocument.Open(strDoc, true);
            BookmarkStart currentBM = null;
            var paragraphs = wordprocessingDocument.MainDocumentPart.Document.Descendants<Paragraph>();

            kontrolLists = new List<KontrolList>();
            List<string> icindekilerOncesi = new List<string>();
            kaynakBelirtimleri = new List<int>();
            int nerede = 0;//ilk iki paragrafı almamak için bu değişken(tabloların başındaki gereksiz ilk iki paragraf)
            int hangiTabloda = 5;
            int kaynak = 0; string text = "";
            foreach (var paragraph in paragraphs)
            {
                var t = paragraph.NextSibling<BookmarkStart>();
                var s = paragraph.ParagraphProperties;

                //if (t != null)
                //{
                //    if (currentBM == null)
                //    {
                //        currentBM = t;
                //        k++;
                //    }
                //    else
                //    {
                //        if (!currentBM.Name.Equals(t.Name))
                //        {
                //           // text += k + "\n";
                //            k++;
                //            currentBM = t;
                //        }
                //    }

                //}


                if (paragraph.InnerText.Length > 1)
                {
                    string temp = paragraph.InnerText.ToString();
                    if (temp.Equals("ÖNSÖZ") || temp.Equals("ABSTRACT"))
                    { icindekilerOncesi.Add(temp); }

                    if (paragraph.InnerText.ToString().ToLower().Equals("içindekiler"))
                    { nerede = 0; hangiTabloda = 1; icindekilerOncesi.Add("İÇİNDEKİLER"); }

                    if (paragraph.InnerText.ToString().ToLower().Equals("şekiller listesi"))
                    { nerede = 0; hangiTabloda = 2; icindekilerOncesi.Add("ŞEKİLLER LİSTESİ"); }
                    if (paragraph.InnerText.ToString().ToLower().Equals("tablolar listesi"))
                    { nerede = 0; hangiTabloda = 3; icindekilerOncesi.Add("TABLOLAR LİSTESİ"); }
                    if (paragraph.InnerText.ToString().ToLower().Equals("ekler listesi"))
                    { nerede = 0; hangiTabloda = 4; icindekilerOncesi.Add("EKLER LİSTESİ"); }

                    if (paragraph.InnerText.ToString().ToLower().Equals("giriş"))
                    { hangiTabloda = 6; }

                    if (paragraph.InnerText.ToString().ToLower().Equals("özet"))
                    { hangiTabloda = 5; icindekilerOncesi.Add("ÖZET"); }
                    if (paragraph.InnerText.ToString().ToLower().Equals("simgeler ve kisaltmalar"))
                    { hangiTabloda = 5; icindekilerOncesi.Add("SİMGELER VE KISALTMALAR"); }


                    //içindekiler-şekiller-tablolar-ekler tablolarında bulunan değerlerin depolanması
                    if (nerede > 2 && hangiTabloda <= 4 && paragraph.InnerText.Length > 4)
                    {
                        kontrolLists.Add(nesneGonder(paragraph.InnerText.ToString(), hangiTabloda));
                    }

                    //GİRİŞ başlığından sonra dokuman kısmına geçtik, bu scopta dokuman incelenir.
                    if (hangiTabloda == 6)
                    {
                        //içerik listemizde mevcut sayfayı içeren girdileri verir 
                        //List<KontrolList> tum = kontrolLists.FindAll(x => x.sayfa.ToString().Equals(k.ToString()));

                        KontrolList tempItem = null;
                        foreach (var item in kontrolLists)
                        {
                            if (paragraph.InnerText.ToString().Contains(item.icerik))
                            {
                                tempItem = item;
                            }
                        }

                        if (tempItem != null)
                        {
                            if (!paragraph.InnerText.ToString().Contains(tempItem.numara))
                            {
                                if (!tempItem.numara.Equals("hata"))
                                {
                                    text += tempItem.hangiBolum + " tablosunda belirtilen içerikte " + tempItem.numara + "  numarası bulunamadı. Paragrafı kontrol edin : " + paragraph.InnerText.ToString() + "\n\n";
                                }
                                else
                                {
                                    text += tempItem.hangiBolum + " tablosunda belirtilen içerik " + "  işlenemedi. Lütfen Paragrafı manuel olarak kontrol edin : " + paragraph.InnerText.ToString() + "\n\n";
                                }

                            }
                            kontrolLists.Remove(tempItem);
                        }

                        if (paragraph.InnerText.ToString().Contains("["))
                        {

                            string sonuc = kaynakBelirtimKontrol(paragraph.InnerText.ToString());
                            //text += sonuc + "\n";
                            if (sonuc.Contains("hata"))
                            {
                                text += "\n\n Toplu Kaynak belirtimlerinde maksimum 3 kaynak belirtebilirsiniz. Şu paragrafta kaynak belirtiminde hata tespit edlmiştir: \t" + paragraph.InnerText.ToString() + "\n\n";
                            }
                        }

                    }

                    //Başlıklardan sonra 2 tane paragraf işimize yaramadığı için nerede değişkeni sayesinde onlardan kurtuluyoruz
                    nerede++;
                }

            }


            //Burada içindekiler tablosundan önce gelen başlıkların kontrolü yapılır.
            for (int i = 0; i < icindekilerOncesi.Count; i++)
            {
                var temp = kontrolLists.Find(x => x.icerik.Equals(icindekilerOncesi[i]));
                if (temp != null)
                {
                    kontrolLists.Remove(temp);
                }
            }

            text += "\n\n\n";

            //tablolarda belirtilen ancak ya içerik olarak yada numara olrak dokümanda bulunmayan başlıkları verir
            foreach (var item in kontrolLists)
            {
                if (!item.numara.Equals("hata"))
                {
                    text += item.hangiBolum + " tablosunda " + item.numara + " numaralı " + item.sayfa + ". sayfada bulunması gereken \"" + item.icerik + "\" içeriği bulunamadı. bir kontrol edin\n\n";
                }
                else
                {
                    text += item.hangiBolum + " tablosunda " + item.numara + " numaralı " + item.sayfa + ". sayfada bulunması gereken \"" + item.icerik + "\" içeriği bulunamadı. bir kontrol edin\n\n";
                }

            }

            text += "\n\n\n";
            //Kaynak belirtiminde numara atlaması olmuş mu onu kontrol eder.
            //örneğin  24 numaralı kaynak belirtimi olmasına rağmen 23 numaralı kaynak belirtiminin olmaması.
            text += belirtilmeyenKaynaklariBul();

            //yazılım herhangi bir hata bulamazsa verilecek çıktı
            if (text.Equals("\n\n\n\n\n\n"))
            {
                text = "herhangi bir hata bulunamadi.";
            }
            else//sonuçlar ile ilgili uyarı
            {
                text = "Değerli kullanıcımız aşağıda verilen bazı hatalar dokumanda karakter olarak var olmayıp" +
                    " word numaralandırması olarak var olan bazı değerleri verebilir. Verilen hataları kontrol edin. Eğer siz " +
                    "kontrol ettğinizde bu başlıklar doğru görünüyorsa sıkıntı yoktur.\n\n" + text;
            }

            richTextBox1.Text = text;
            MessageBox.Show("okuma İşlemi Tamamlandı!", "Bilgilendirme Penceresi");
            wordprocessingDocument.Close();

            var datam1 = new Data
            {

                dosyam = text
            };

            SetResponse response = await client.SetTaskAsync("DosyaHatalar/" + "hatam1", datam1);
            Data result = response.ResultAs<Data>();
           
        }


        public string belirtilmeyenKaynaklariBul()
        {
            String sonuc = "";
            if (kaynakBelirtimleri != null && kaynakBelirtimleri.Count > 0)
            {


                int enBuyuk = kaynakBelirtimleri[0], enKucuk = kaynakBelirtimleri[0];
                foreach (var item in kaynakBelirtimleri)
                {
                    if (item > enBuyuk)
                        enBuyuk = item;

                    if (item < enKucuk)
                        enKucuk = item;
                }

                int temp = enKucuk;
                for (int i = enKucuk + 1; i < enBuyuk; i++)
                {
                    if (temp != i - 1)
                        sonuc += temp + ", ";

                    temp = i;
                }

                if (!sonuc.Equals(""))
                {
                    sonuc = "Kaynak belirtilirken atlanılan sayılar : " + sonuc;
                }
            }
            return sonuc;
        }
        private KontrolList nesneGonder(string paragraf, int hangiTabloda)
        {
            //sayfa numarası için son dört karakterin alınması
            String sayi = paragraf.Substring(paragraf.Length - 4);

            string romenNumber = "";//romen rakamalrının alınması için kullanılan değişken
            foreach (char item in sayi)
            {
                try
                {
                    if (item.Equals('i') || item.Equals('v') || item.Equals('x'))
                    {

                        romenNumber = romenNumber + item;
                    }
                    int a = Convert.ToInt32(item.ToString());
                }
                catch (Exception)
                {
                    sayi = sayi.Substring(1);
                    //throw;
                }
            }



            KontrolList kontrolList;

            String icerik = paragraf.Substring(0, paragraf.Length - sayi.Length);

            int sayfa;

            try
            {
                sayfa = Convert.ToInt32(sayi);
            }
            catch (Exception)
            {
                if (!romenNumber.Equals(""))
                {
                    sayfa = sayfabul(romenNumber);
                }
                else
                    sayfa = -9999;
                //burada hata verdi
                // throw;
            }


            // romen rakamalrın icerikten silinmesi
            if (!romenNumber.Equals("") && sayi.Equals(""))
            {
                icerik = icerik.Substring(0, icerik.Length - romenNumber.Length);
            }

            //bir paragrafa sığmayan şekil ve tablo içeriklerinin bir önceki paragrafa eklenmesi
            if ((hangiTabloda == 2 || hangiTabloda == 3) && !(paragraf.Contains("Şekil") || paragraf.Contains("Tablo")))
            {
                kontrolList = kontrolLists[kontrolLists.Count - 1];
                kontrolList.icerik = kontrolList.icerik + " " + icerik;
                kontrolList.sayfa = sayfa;
                kontrolLists.RemoveAt(kontrolLists.Count - 1);
            }
            else
            {
                //icerik nesnelerinin oluşturulması

                switch (hangiTabloda)
                {

                    case 1:

                        if (!icindekilerIcerikDuzenle(icerik).Equals("hata"))
                        {
                            string[] dizi = icindekilerIcerikDuzenle(icerik).Split('$');
                            kontrolList = new KontrolList("icindekiler", dizi[1], sayfa, dizi[0]);
                        }
                        else
                            kontrolList = new KontrolList("icindekiler", icerik, sayfa, "hata");
                        break;

                    case 2:
                        if (!sekillerIcerikDuzenle(icerik).Equals("hata"))
                        {
                            string[] dizi1 = sekillerIcerikDuzenle(icerik).Split('$');
                            kontrolList = new KontrolList("sekiller", dizi1[1], sayfa, dizi1[0]);
                        }
                        else
                            kontrolList = new KontrolList("sekiller", icerik, sayfa, "hata");

                        break;

                    case 3:
                        if (!tabloIcerikDuzenle(icerik).Equals("hata"))
                        {
                            string[] dizi2 = tabloIcerikDuzenle(icerik).Split('$');
                            kontrolList = new KontrolList("tablolar", dizi2[1], sayfa, dizi2[0]);
                        }
                        else
                            kontrolList = new KontrolList("tablolar", icerik, sayfa, "hata");
                        break;

                    case 4:
                        if (!ekIcerikDuzenle(icerik).Equals("hata"))
                        {
                            string[] dizi3 = ekIcerikDuzenle(icerik).Split('$');
                            kontrolList = new KontrolList("ekler", dizi3[1], sayfa, dizi3[0]);
                        }
                        else
                            kontrolList = new KontrolList("ekler", icerik, sayfa, "hata");
                        break;
                    default:
                        kontrolList = new KontrolList("x(bilinmeyen)", icerik, sayfa, "hata");
                        break;
                }
            }

            return kontrolList;
        }
        public string sekillerIcerikDuzenle(string icerik)
        {
            while (icerik.Substring(icerik.Length - 1).Equals("."))
            {
                icerik = icerik.Substring(0, icerik.Length - 1);
            }
            try
            {
                int index = icerik.Substring(9, 1).Equals(".") ? 9 : 10;
                string[] dizi = icerik.Substring(0, index).Split(' ');
                string numara = "x (numara alinamadi)";

                numara = dizi[1];
                icerik = icerik.Substring(index + 1);
                return numara + "$" + icerik;
            }
            catch (Exception)
            {
                return "hata";
            }

        }
        public string tabloIcerikDuzenle(string icerik)
        {
            while (icerik.Substring(icerik.Length - 1).Equals("."))
            {
                icerik = icerik.Substring(0, icerik.Length - 1);
            }
            try
            {
                int index = icerik.Substring(9, 1).Equals(".") ? 9 : 10;
                string[] dizi = icerik.Substring(0, index).Split(' ');
                string numara = "numara alinamadi";

                numara = dizi[1];

                icerik = icerik.Substring(index + 1);
                return numara + "$" + icerik;

            }
            catch (Exception)
            {
                return "hata";
            }
        }
        public string icindekilerIcerikDuzenle(string icerik)
        {
            while (icerik.Substring(icerik.Length - 1).Equals("."))
            {
                icerik = icerik.Substring(0, icerik.Length - 1);
            }
            try
            {

                int index = icerik.LastIndexOf('.');
                string numara;

                numara = icerik.Substring(0, index).Trim();

                icerik = icerik.Substring(index + 1);
                icerik = icerik.Trim();
                return numara + "$" + icerik;
            }
            catch (Exception)
            {
                return "hata";
            }


        }
        public string ekIcerikDuzenle(string icerik)
        {
            while (icerik.Substring(icerik.Length - 1).Equals("."))
            {
                icerik = icerik.Substring(0, icerik.Length - 1);
            }

            try
            {
                int index = icerik.IndexOf(':');
                string[] dizi = icerik.Substring(0, index).Split(' ');
                string numara = "numara alinamadi";

                numara = dizi[1];

                icerik = icerik.Substring(index + 1);
                return numara + "$" + icerik;
            }
            catch (Exception)
            {
                return "hata";
            }


        }
        public int sayfabul(string metin)
        {
            int sayi = 0;

            int i_sayisi = 0;
            foreach (char item in metin)
            {
                if (item.Equals('i'))
                    i_sayisi++;

            }

            if (metin.Substring(0, 1).Equals("x"))
                sayi = 10 + i_sayisi;
            if (metin.Substring(0, 1).Equals("v"))
                sayi = 5 + i_sayisi;
            if (metin.Substring(metin.Length - 1).Equals("x"))
                sayi = 10 - i_sayisi;
            if (metin.Substring(metin.Length - 1).Equals("v"))
                sayi = 5 - i_sayisi;

            return sayi;
        }
        public string kaynakBelirtimKontrol(string paragraf)
        {
            string sonuc = "";
            int first = paragraf.IndexOf("[");
            int last = paragraf.IndexOf("]");
            if (last < 0)
            {
                sonuc = "parantez kapatma yanlisi";
            }
            else
            {

                string kaynak = "";
                for (int i = first + 1; i < last; i++)
                {
                    kaynak += paragraf[i];
                }
                // sonuc = kaynak;
                if ((kaynak.Contains("-") || kaynak.Contains("–")) && !kaynak.Contains(","))
                {
                    char ayrac = (kaynak.Contains("-")) ? '-' : '–';

                    string[] dizi = kaynak.Trim().Split(ayrac);
                    try
                    {
                        int baslangic = Convert.ToInt32(dizi[0].Trim());
                        int son = Convert.ToInt32(dizi[1].Trim());

                        if (son - baslangic > 2)
                        {
                            sonuc += "hata";

                        }
                        int j = baslangic;
                        while (j < son + 1)
                        {
                            kaynakBelirtimleri.Add(j);

                            j++;
                        }

                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                }
                else if (kaynak.Contains(",") && !kaynak.Contains("-") && !kaynak.Contains("–"))
                {
                    sonuc += "dogru";
                    string[] arr = kaynak.Split(',');

                    foreach (var item in arr)
                    {
                        try
                        {
                            int sayi = Convert.ToInt32(item.Trim());

                            kaynakBelirtimleri.Add(sayi);
                        }
                        catch (Exception)
                        {

                            //throw;
                        }

                    }
                }
                else if (!kaynak.Contains(",") && !kaynak.Contains("-") && !kaynak.Contains("–"))
                {
                    sonuc += "dogru";

                    try
                    {
                        int sayi = Convert.ToInt32(kaynak.Trim());

                        kaynakBelirtimleri.Add(sayi);
                    }
                    catch (Exception)
                    {

                        //throw;
                    }



                }
                else if (kaynak.Contains(",") && (kaynak.Contains("-") || kaynak.Contains("–")))
                {
                    string[] dizi = kaynak.Split(',');

                    foreach (var item in dizi)
                    {
                        if (!item.Contains("-") && !item.Contains("–"))
                        {
                            try
                            {
                                int sayi = Convert.ToInt32(item.Trim());

                                kaynakBelirtimleri.Add(sayi);
                            }
                            catch (Exception)
                            {

                                //throw;
                            }


                        }
                        else
                        {
                            //kaynak=kaynak.Trim();
                            char ayrac = (item.Contains("-")) ? '-' : '–';
                            string[] array = item.Trim().Split(ayrac);
                            try
                            {
                                int baslangic = Convert.ToInt32(array[0].Trim());
                                int son = Convert.ToInt32(array[1].Trim());

                                if (son - baslangic > 2)
                                {
                                    sonuc += "hata";

                                }
                                int l = baslangic;
                                while (l < son + 1)
                                {
                                    kaynakBelirtimleri.Add(l);
                                    l++;
                                }

                            }
                            catch (Exception)
                            {

                                // throw;
                            }
                        }
                    }
                }
                else
                {
                    sonuc = "gecbunu" + kaynak;
                }

            }
            return sonuc;
        }




        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
        }
    }
}
