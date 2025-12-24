using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class GonderilenSmslerMap : EntityTypeConfiguration<GonderilenSmsler>
    {
        public GonderilenSmslerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SmsMetni)
                .IsRequired();

            this.Property(t => t.Telefon)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.SmsGorevId)
                .HasMaxLength(50);

            this.Property(t => t.Durum)
                .HasMaxLength(3);

            this.Property(t => t.HataKodu)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("GonderilenSmsler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.GonderimTarihi).HasColumnName("GonderimTarihi");
            this.Property(t => t.SmsMetni).HasColumnName("SmsMetni");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.SmsGorevId).HasColumnName("SmsGorevId");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.HataKodu).HasColumnName("HataKodu");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.GonderilenSmslers)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.GonderilenSmslers)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
