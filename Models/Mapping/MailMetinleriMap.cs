using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class MailMetinleriMap : EntityTypeConfiguration<MailMetinleri>
    {
        public MailMetinleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MetinAdi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.MailBaslik)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.MailKonu)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.MailMetni)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("MailMetinleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.MetinAdi).HasColumnName("MetinAdi");
            this.Property(t => t.MailBaslik).HasColumnName("MailBaslik");
            this.Property(t => t.MailKonu).HasColumnName("MailKonu");
            this.Property(t => t.MailMetni).HasColumnName("MailMetni");
            this.Property(t => t.IcerikResim).HasColumnName("IcerikResim");
            this.Property(t => t.TemaYol).HasColumnName("TemaYol");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.MailMetinleris)
                .HasForeignKey(d => d.FirmaId);

        }
    }
}
