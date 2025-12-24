using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class GunlukIsler
    {
        public GunlukIsler()
        {
            this.Odemelers = new List<Odemeler>();
        }

        public long Id { get; set; }
        public long FirmaId { get; set; }
        public long SubeId { get; set; }
        public long TakipNo { get; set; }
        public System.DateTime Tarih { get; set; }
        public long KategoriId { get; set; }
        public Nullable<long> MusteriId { get; set; }
        public string AdSoyad { get; set; }
        public string TCKimlikNo { get; set; }
        public string SabitTel { get; set; }
        public string CepTel { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string OdemeTuru { get; set; }
        public int Adet { get; set; }
        public decimal BirimUcret { get; set; }
        public decimal Ucret { get; set; }
        public Nullable<decimal> Odenen { get; set; }
        public Nullable<decimal> KalanBakiye { get; set; }
        public string Notlar { get; set; }
        public bool Iptal { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual GunlukIsKategori GunlukIsKategori { get; set; }
        public virtual Sube Sube { get; set; }
        public virtual ICollection<Odemeler> Odemelers { get; set; }
    }
}
