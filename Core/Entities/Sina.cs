using System.Collections.Generic;
using System;

namespace Core.Entities;

public class Sina : BaseEntity
{
    public string IlAdi { get; set; }

    public string UstKurumAdi { get; set; }
    public string KurumAdi { get; set; }
    public string KurumKodu { get; set; }
    public string FakulteTipi { get; set; }
    public string FakulteAdi { get; set; }
    public string FakulteKodu { get; set; }
    public string EgitimVerilenKurumAdi { get; set; }
    public string EgitimVerilenKurumKodu { get; set; }

    public string BirlikteKullanimYapilanKurum { get; set; }
    public string BirlikteKullanimYapilanKurumKodu { get; set; }
    public string BirlikteKullanimYapilanFakulte { get; set; }
    public string BirlikteKullanimYapilanFakulteKodu { get; set; }

    public string UzmanlikDali { get; set; }
    public string UzmanlikDaliKodu { get; set; }
    public bool? UzmanlikDaliAnaDalMi { get; set; }

    public string YetkiKategorisi { get; set; }
    public int EgiticiSayisi { get; set; }
    public List<DateTime?> OgrenciMezuniyetTarihiListesi { get; set; }
}