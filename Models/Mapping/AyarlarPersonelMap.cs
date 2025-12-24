using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class AyarlarPersonelMap : EntityTypeConfiguration<AyarlarPersonel>
    {
        public AyarlarPersonelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AyarlarPersonel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.BaslangicIzinSuresi).HasColumnName("BaslangicIzinSuresi");
            this.Property(t => t.BirUcYilIzin).HasColumnName("BirUcYilIzin");
            this.Property(t => t.UcBesYilIzin).HasColumnName("UcBesYilIzin");
            this.Property(t => t.BesOnYilIzin).HasColumnName("BesOnYilIzin");
            this.Property(t => t.OnOnbesYilIzin).HasColumnName("OnOnbesYilIzin");
            this.Property(t => t.OnbesYirmiYilIzin).HasColumnName("OnbesYirmiYilIzin");
            this.Property(t => t.YillikMaasArtisOrani).HasColumnName("YillikMaasArtisOrani");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");
        }
    }
}
