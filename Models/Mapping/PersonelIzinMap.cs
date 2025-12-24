using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class PersonelIzinMap : EntityTypeConfiguration<PersonelIzin>
    {
        public PersonelIzinMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Aciklama)
                .HasMaxLength(250);

            this.Property(t => t.GorevliSubeler)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PersonelIzin");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.PersonelId).HasColumnName("PersonelId");
            this.Property(t => t.IzinBaslama).HasColumnName("IzinBaslama");
            this.Property(t => t.IzinBitis).HasColumnName("IzinBitis");
            this.Property(t => t.IseBaslama).HasColumnName("IseBaslama");
            this.Property(t => t.KullanilanIzinGun).HasColumnName("KullanilanIzinGun");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");
            this.Property(t => t.GorevliSubeler).HasColumnName("GorevliSubeler");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.PersonelIzins)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Personel)
                .WithMany(t => t.PersonelIzins)
                .HasForeignKey(d => d.PersonelId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.PersonelIzins)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
