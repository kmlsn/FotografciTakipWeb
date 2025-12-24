using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class TempSmsGonderListeMap : EntityTypeConfiguration<TempSmsGonderListe>
    {
        public TempSmsGonderListeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.AdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.Telefon)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.TelefonFormatli)
                .HasMaxLength(15);

            this.Property(t => t.Mesaj)
                .HasMaxLength(255);

            this.Property(t => t.Durum)
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("TempSmsGonderListe");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.AdSoyad).HasColumnName("AdSoyad");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.TelefonFormatli).HasColumnName("TelefonFormatli");
            this.Property(t => t.Mesaj).HasColumnName("Mesaj");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");
        }
    }
}
