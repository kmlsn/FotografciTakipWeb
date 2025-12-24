using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class PersonelOdemeMap : EntityTypeConfiguration<PersonelOdeme>
    {
        public PersonelOdemeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.OdemeTuru)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OdemeSekli)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Aciklama)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("PersonelOdeme");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.SubeId).HasColumnName("SubeId");
            this.Property(t => t.PersonelId).HasColumnName("PersonelId");
            this.Property(t => t.OdemeTarihi).HasColumnName("OdemeTarihi");
            this.Property(t => t.OdemeTuru).HasColumnName("OdemeTuru");
            this.Property(t => t.OdemeSekli).HasColumnName("OdemeSekli");
            this.Property(t => t.Tutar).HasColumnName("Tutar");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.PersonelOdemes)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Personel)
                .WithMany(t => t.PersonelOdemes)
                .HasForeignKey(d => d.PersonelId);
            this.HasRequired(t => t.Sube)
                .WithMany(t => t.PersonelOdemes)
                .HasForeignKey(d => d.SubeId);

        }
    }
}
