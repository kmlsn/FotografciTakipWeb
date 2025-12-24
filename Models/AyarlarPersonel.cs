using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class AyarlarPersonel
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public short BaslangicIzinSuresi { get; set; }
        public short BirUcYilIzin { get; set; }
        public short UcBesYilIzin { get; set; }
        public short BesOnYilIzin { get; set; }
        public short OnOnbesYilIzin { get; set; }
        public short OnbesYirmiYilIzin { get; set; }
        public short YillikMaasArtisOrani { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
    }
}
