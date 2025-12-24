using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Musteri
    {
        public Musteri()
        {
            this.MusteriFotografs = new List<MusteriFotograf>();
            this.MusteriMesajs = new List<MusteriMesaj>();
            this.MusteriMesajlaris = new List<MusteriMesajlari>();
            this.Odemelers = new List<Odemeler>();
            this.Sozlesmes = new List<Sozlesme>();
        }

        public long Id { get; set; }
        public long SubeId { get; set; }
        public long FirmaId { get; set; }
        public long MusteriKodu { get; set; }
        public string Sifre { get; set; }
        public string AdiSoyadi { get; set; }
        public string TCKimlikNo { get; set; }
        public string SabitTel { get; set; }
        public string CepTel { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public string Notlar { get; set; }
        public bool SMSKabul { get; set; }
        public bool EmailKabul { get; set; }
        public bool KilitBit { get; set; }
        public Nullable<bool> MusteriPanelGirisYetki { get; set; }
        public string FotografSecimDurum { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Sube Sube { get; set; }
        public virtual ICollection<MusteriFotograf> MusteriFotografs { get; set; }
        public virtual ICollection<MusteriMesaj> MusteriMesajs { get; set; }
        public virtual ICollection<MusteriMesajlari> MusteriMesajlaris { get; set; }
        public virtual ICollection<Odemeler> Odemelers { get; set; }
        public virtual ICollection<Sozlesme> Sozlesmes { get; set; }
    }
}
