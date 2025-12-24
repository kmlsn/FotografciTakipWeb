using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class GonderilenEmaillerMap : EntityTypeConfiguration<GonderilenEmailler>
    {
        public GonderilenEmaillerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MailKonu)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.AliciEposta)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MailIcerik)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("GonderilenEmailler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.MailKonu).HasColumnName("MailKonu");
            this.Property(t => t.AliciEposta).HasColumnName("AliciEposta");
            this.Property(t => t.MailIcerik).HasColumnName("MailIcerik");
            this.Property(t => t.TemaYol).HasColumnName("TemaYol");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.GonderilenEmaillers)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.GonderilenEmaillers)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
