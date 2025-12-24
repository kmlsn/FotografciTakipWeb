using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class SiparislerMap : EntityTypeConfiguration<Siparisler>
    {
        public SiparislerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Paket)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PaketDetay)
                .IsRequired();

            this.Property(t => t.OdemeBildirimAciklama)
                .HasMaxLength(250);

            this.Property(t => t.OdemeHata)
                .HasMaxLength(250);

            this.Property(t => t.OdemeTuru)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("Siparisler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SatisFiyatId).HasColumnName("SatisFiyatId");
            this.Property(t => t.SiparisNo).HasColumnName("SiparisNo");
            this.Property(t => t.Paket).HasColumnName("Paket");
            this.Property(t => t.PaketDetay).HasColumnName("PaketDetay");
            this.Property(t => t.PaketTutar).HasColumnName("PaketTutar");
            this.Property(t => t.LisansSuresi).HasColumnName("LisansSuresi");
            this.Property(t => t.Odendi).HasColumnName("Odendi");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.Tarih).HasColumnName("Tarih");
            this.Property(t => t.Iptal).HasColumnName("Iptal");
            this.Property(t => t.OdemeBildirim).HasColumnName("OdemeBildirim");
            this.Property(t => t.Dosya).HasColumnName("Dosya");
            this.Property(t => t.OdemeBildirimAciklama).HasColumnName("OdemeBildirimAciklama");
            this.Property(t => t.OdemeHata).HasColumnName("OdemeHata");
            this.Property(t => t.OdemeTuru).HasColumnName("OdemeTuru");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.Siparislers)
                .HasForeignKey(d => d.FirmaId);
            this.HasOptional(t => t.SatisFiyatlari)
                .WithMany(t => t.Siparislers)
                .HasForeignKey(d => d.SatisFiyatId);

        }
    }
}
