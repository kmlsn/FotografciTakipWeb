using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Kullanici
    {
        public Kullanici()
        {
            this.DestekTalepleris = new List<DestekTalepleri>();
            this.DestekTalepleriDetays = new List<DestekTalepleriDetay>();
            this.GirisLogs = new List<GirisLog>();
            this.KullaniciYetkis = new List<KullaniciYetki>();
            this.SubeToKullanicis = new List<SubeToKullanici>();
        }

        public long Id { get; set; }
        public long RolId { get; set; }
        public long GorevId { get; set; }
        public long FirmaId { get; set; }
        public string Email { get; set; }
        public string CepTel { get; set; }
        public string SifreHash { get; set; }
        public string GeciciSifre { get; set; }
        public string AdSoyad { get; set; }
        public Nullable<long> ResimId { get; set; }
        public string Notlar { get; set; }
        public string YetkiliSubeler { get; set; }
        public bool DuyuruBit { get; set; }
        public bool KilitBit { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual ICollection<DestekTalepleri> DestekTalepleris { get; set; }
        public virtual ICollection<DestekTalepleriDetay> DestekTalepleriDetays { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual ICollection<GirisLog> GirisLogs { get; set; }
        public virtual PersonelGorevleri PersonelGorevleri { get; set; }
        public virtual Resim Resim { get; set; }
        public virtual Rol Rol { get; set; }
        public virtual ICollection<KullaniciYetki> KullaniciYetkis { get; set; }
        public virtual ICollection<SubeToKullanici> SubeToKullanicis { get; set; }
    }
}
