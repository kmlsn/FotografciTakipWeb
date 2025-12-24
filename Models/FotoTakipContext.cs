using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FotografciTakipWeb.Models.Mapping;

namespace FotografciTakipWeb.Models
{
    public partial class FotoTakipContext : DbContext
    {
        static FotoTakipContext()
        {
            Database.SetInitializer<FotoTakipContext>(null);
        }

        public FotoTakipContext()
            : base("Name=FotoTakipContext")
        {
        }

        public DbSet<AyarlarFiltre> AyarlarFiltres { get; set; }
        public DbSet<AyarlarGenel> AyarlarGenels { get; set; }
        public DbSet<AyarlarMailGonderim> AyarlarMailGonderims { get; set; }
        public DbSet<AyarlarMailHesap> AyarlarMailHesaps { get; set; }
        public DbSet<AyarlarMusteri> AyarlarMusteris { get; set; }
        public DbSet<AyarlarPersonel> AyarlarPersonels { get; set; }
        public DbSet<AyarlarRezervasyon> AyarlarRezervasyons { get; set; }
        public DbSet<AyarlarSmsGonderim> AyarlarSmsGonderims { get; set; }
        public DbSet<AyarlarSozlesmeCikti> AyarlarSozlesmeCiktis { get; set; }
        public DbSet<Cari> Caris { get; set; }
        public DbSet<CariHareket> CariHarekets { get; set; }
        public DbSet<CekimPaketleri> CekimPaketleris { get; set; }
        public DbSet<DestekTalepleri> DestekTalepleris { get; set; }
        public DbSet<DestekTalepleriDetay> DestekTalepleriDetays { get; set; }
        public DbSet<Firma> Firmas { get; set; }
        public DbSet<GelirGider> GelirGiders { get; set; }
        public DbSet<GelirGiderTurleri> GelirGiderTurleris { get; set; }
        public DbSet<GirisLog> GirisLogs { get; set; }
        public DbSet<GonderilenEmailler> GonderilenEmaillers { get; set; }
        public DbSet<GonderilenSmsler> GonderilenSmslers { get; set; }
        public DbSet<GunlukIsKategori> GunlukIsKategoris { get; set; }
        public DbSet<GunlukIsler> GunlukIslers { get; set; }
        public DbSet<HataLoglari> HataLoglaris { get; set; }
        public DbSet<Il> Ils { get; set; }
        public DbSet<Ilce> Ilces { get; set; }
        public DbSet<Kullanici> Kullanicis { get; set; }
        public DbSet<KullaniciYetki> KullaniciYetkis { get; set; }
        public DbSet<Lisanslar> Lisanslars { get; set; }
        public DbSet<MailMetinleri> MailMetinleris { get; set; }
        public DbSet<ModulSayfa> ModulSayfas { get; set; }
        public DbSet<Musteri> Musteris { get; set; }
        public DbSet<MusteriFotograf> MusteriFotografs { get; set; }
        public DbSet<MusteriMesaj> MusteriMesajs { get; set; }
        public DbSet<MusteriMesajlari> MusteriMesajlaris { get; set; }
        public DbSet<MusteriMesajlariDetay> MusteriMesajlariDetays { get; set; }
        public DbSet<Odemeler> Odemelers { get; set; }
        public DbSet<OtomatikSmsListesi> OtomatikSmsListesis { get; set; }
        public DbSet<Personel> Personels { get; set; }
        public DbSet<PersonelGorevleri> PersonelGorevleris { get; set; }
        public DbSet<PersonelIzin> PersonelIzins { get; set; }
        public DbSet<PersonelOdeme> PersonelOdemes { get; set; }
        public DbSet<Randevu> Randevus { get; set; }
        public DbSet<RandevuGorunum> RandevuGorunums { get; set; }
        public DbSet<RandevuToPersonel> RandevuToPersonels { get; set; }
        public DbSet<RehberGrup> RehberGrups { get; set; }
        public DbSet<Resim> Resims { get; set; }
        public DbSet<RezervasyonEkHizmet> RezervasyonEkHizmets { get; set; }
        public DbSet<RezervasyonTurleri> RezervasyonTurleris { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<RolYetki> RolYetkis { get; set; }
        public DbSet<SatisFiyatlari> SatisFiyatlaris { get; set; }
        public DbSet<Siparisler> Siparislers { get; set; }
        public DbSet<SmsBakiye> SmsBakiyes { get; set; }
        public DbSet<SmsHareket> SmsHarekets { get; set; }
        public DbSet<SmsMetinleri> SmsMetinleris { get; set; }
        public DbSet<Sozlesme> Sozlesmes { get; set; }
        public DbSet<SozlesmeSartlari> SozlesmeSartlaris { get; set; }
        public DbSet<Sube> Subes { get; set; }
        public DbSet<SubeToKullanici> SubeToKullanicis { get; set; }
        public DbSet<SubeToPersonel> SubeToPersonels { get; set; }
        public DbSet<Surecler> Sureclers { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<TatilGunleri> TatilGunleris { get; set; }
        public DbSet<TelefonRehberi> TelefonRehberis { get; set; }
        public DbSet<TempSmsGonderListe> TempSmsGonderListes { get; set; }
        public DbSet<Unvanlar> Unvanlars { get; set; }
        public DbSet<ZamanDilimleri> ZamanDilimleris { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AyarlarFiltreMap());
            modelBuilder.Configurations.Add(new AyarlarGenelMap());
            modelBuilder.Configurations.Add(new AyarlarMailGonderimMap());
            modelBuilder.Configurations.Add(new AyarlarMailHesapMap());
            modelBuilder.Configurations.Add(new AyarlarMusteriMap());
            modelBuilder.Configurations.Add(new AyarlarPersonelMap());
            modelBuilder.Configurations.Add(new AyarlarRezervasyonMap());
            modelBuilder.Configurations.Add(new AyarlarSmsGonderimMap());
            modelBuilder.Configurations.Add(new AyarlarSozlesmeCiktiMap());
            modelBuilder.Configurations.Add(new CariMap());
            modelBuilder.Configurations.Add(new CariHareketMap());
            modelBuilder.Configurations.Add(new CekimPaketleriMap());
            modelBuilder.Configurations.Add(new DestekTalepleriMap());
            modelBuilder.Configurations.Add(new DestekTalepleriDetayMap());
            modelBuilder.Configurations.Add(new FirmaMap());
            modelBuilder.Configurations.Add(new GelirGiderMap());
            modelBuilder.Configurations.Add(new GelirGiderTurleriMap());
            modelBuilder.Configurations.Add(new GirisLogMap());
            modelBuilder.Configurations.Add(new GonderilenEmaillerMap());
            modelBuilder.Configurations.Add(new GonderilenSmslerMap());
            modelBuilder.Configurations.Add(new GunlukIsKategoriMap());
            modelBuilder.Configurations.Add(new GunlukIslerMap());
            modelBuilder.Configurations.Add(new HataLoglariMap());
            modelBuilder.Configurations.Add(new IlMap());
            modelBuilder.Configurations.Add(new IlceMap());
            modelBuilder.Configurations.Add(new KullaniciMap());
            modelBuilder.Configurations.Add(new KullaniciYetkiMap());
            modelBuilder.Configurations.Add(new LisanslarMap());
            modelBuilder.Configurations.Add(new MailMetinleriMap());
            modelBuilder.Configurations.Add(new ModulSayfaMap());
            modelBuilder.Configurations.Add(new MusteriMap());
            modelBuilder.Configurations.Add(new MusteriFotografMap());
            modelBuilder.Configurations.Add(new MusteriMesajMap());
            modelBuilder.Configurations.Add(new MusteriMesajlariMap());
            modelBuilder.Configurations.Add(new MusteriMesajlariDetayMap());
            modelBuilder.Configurations.Add(new OdemelerMap());
            modelBuilder.Configurations.Add(new OtomatikSmsListesiMap());
            modelBuilder.Configurations.Add(new PersonelMap());
            modelBuilder.Configurations.Add(new PersonelGorevleriMap());
            modelBuilder.Configurations.Add(new PersonelIzinMap());
            modelBuilder.Configurations.Add(new PersonelOdemeMap());
            modelBuilder.Configurations.Add(new RandevuMap());
            modelBuilder.Configurations.Add(new RandevuGorunumMap());
            modelBuilder.Configurations.Add(new RandevuToPersonelMap());
            modelBuilder.Configurations.Add(new RehberGrupMap());
            modelBuilder.Configurations.Add(new ResimMap());
            modelBuilder.Configurations.Add(new RezervasyonEkHizmetMap());
            modelBuilder.Configurations.Add(new RezervasyonTurleriMap());
            modelBuilder.Configurations.Add(new RolMap());
            modelBuilder.Configurations.Add(new RolYetkiMap());
            modelBuilder.Configurations.Add(new SatisFiyatlariMap());
            modelBuilder.Configurations.Add(new SiparislerMap());
            modelBuilder.Configurations.Add(new SmsBakiyeMap());
            modelBuilder.Configurations.Add(new SmsHareketMap());
            modelBuilder.Configurations.Add(new SmsMetinleriMap());
            modelBuilder.Configurations.Add(new SozlesmeMap());
            modelBuilder.Configurations.Add(new SozlesmeSartlariMap());
            modelBuilder.Configurations.Add(new SubeMap());
            modelBuilder.Configurations.Add(new SubeToKullaniciMap());
            modelBuilder.Configurations.Add(new SubeToPersonelMap());
            modelBuilder.Configurations.Add(new SureclerMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new TatilGunleriMap());
            modelBuilder.Configurations.Add(new TelefonRehberiMap());
            modelBuilder.Configurations.Add(new TempSmsGonderListeMap());
            modelBuilder.Configurations.Add(new UnvanlarMap());
            modelBuilder.Configurations.Add(new ZamanDilimleriMap());
        }
    }
}
