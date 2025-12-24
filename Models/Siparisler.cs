using System;
using System.Collections.Generic;

namespace FotografciTakipWeb.Models
{
    public partial class Siparisler
    {
        public long Id { get; set; }
        public long FirmaId { get; set; }
        public Nullable<long> SatisFiyatId { get; set; }
        public long SiparisNo { get; set; }
        public string Paket { get; set; }
        public string PaketDetay { get; set; }
        public decimal PaketTutar { get; set; }
        public short LisansSuresi { get; set; }
        public bool Odendi { get; set; }
        public short Durum { get; set; }
        public System.DateTime Tarih { get; set; }
        public bool Iptal { get; set; }
        public Nullable<bool> OdemeBildirim { get; set; }
        public string Dosya { get; set; }
        public string OdemeBildirimAciklama { get; set; }
        public string OdemeHata { get; set; }
        public string OdemeTuru { get; set; }
        public long OlusturanKullaniciId { get; set; }
        public System.DateTime OlusturmaTarih { get; set; }
        public long DegistirenKullaniciId { get; set; }
        public System.DateTime DegistirmeTarih { get; set; }
        public bool Aktif { get; set; }
        public bool Sil { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual SatisFiyatlari SatisFiyatlari { get; set; }
    }
}
