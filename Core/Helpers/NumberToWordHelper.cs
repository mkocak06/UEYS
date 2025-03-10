using System;
using System.IO;

namespace Core.Helper
{
    public static class NumberToWordHelper
    {
        public static string ConvertToWord(decimal tutar)
        {
            string sTutar = tutar.ToString("F2").Replace('.', ',');
            // Replace('.',',') ondalık ayracının . olma durumu için    

            string lira = sTutar.Substring(0, sTutar.IndexOf(','));
            //tutarın tam kısmı   

            string kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);
            string yazi = "";

            string[] birler = { "", "BİR ", "İKİ ", "ÜÇ ", "DÖRT ", "BEŞ ", "ALTI ", "YEDİ ", "SEKİZ ", "DOKUZ " };
            string[] onlar = { "", "ON ", "YİRMİ ", "OTUZ ", "KIRK ", "ELLİ ", "ALTMIŞ ", "YETMİŞ ", "SEKSEN ", "DOKSAN " };
            string[] binler = { "KATRİLYON ", "TRİLYON ", "MİLYAR ", "MİLYON ", "BİN ", "" };
            //KATRİLYON'un önüne ekleme yapılarak artırabilir.   

            int grupSayisi = 6;
            //sayıdaki 3'lü grup sayısı. katrilyon içi 6. (1.234,00 daki grup sayısı 2'dir.)                       
            //KATRİLYON'un başına ekleyeceğiniz her değer için grup sayısını artırınız.    

            lira = lira.PadLeft(grupSayisi * 3, '0');
            //sayının soluna '0' eklenerek sayı 'grup sayısı x 3' basakmaklı yapılıyor.   

            string grupDegeri;
            for (int i = 0; i < grupSayisi * 3; i += 3)            //sayı 3'erli gruplar halinde ele alınıyor. 
            {
                grupDegeri = "";
                string s = lira.Substring(i, 1);
                if (s != "0")
                    grupDegeri += birler[Convert.ToInt32(lira.Substring(i, 1))] + "YÜZ"; //yüzler            
                if (grupDegeri == "BİRYÜZ") //biryüz düzeltiliyor.         
                    grupDegeri = "YÜZ";
                grupDegeri += onlar[Convert.ToInt32(lira.Substring(i + 1, 1))]; //onlar  
                grupDegeri += birler[Convert.ToInt32(lira.Substring(i + 2, 1))]; //birler    
                if (grupDegeri != "") //binler    
                    grupDegeri += binler[i / 3];
                if (grupDegeri == "BİRBİN") //birbin düzeltiliyor.         
                    grupDegeri = "BİN";
                yazi += grupDegeri;
            }

            // VİRGÜLDEN SONRA İKİ BASAMAK
            if (kurus.Substring(0, 1) != "0") //kuruş onlar     
            {
                yazi += "VİRGÜL ";
                yazi += onlar[Convert.ToInt32(kurus.Substring(0, 1))];
            }
            if (kurus.Substring(1, 1) != "0") //kuruş birler   
            {
                yazi += birler[Convert.ToInt32(kurus.Substring(1, 1))];
            }

            yazi = yazi.Replace("YÜZ", "YÜZ ").Replace("BİN", "BİN ");
            return yazi;
        }
    }
}
