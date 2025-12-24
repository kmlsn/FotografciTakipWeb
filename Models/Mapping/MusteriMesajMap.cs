using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class MusteriMesajMap : EntityTypeConfiguration<MusteriMesaj>
    {
        public MusteriMesajMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Konu)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Mesaj)
                .IsRequired();

            this.Property(t => t.Durum)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("MusteriMesaj");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.MesajId).HasColumnName("MesajId");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.Mesaj).HasColumnName("Mesaj");
            this.Property(t => t.MesajTarihi).HasColumnName("MesajTarihi");
            this.Property(t => t.CevapTarihi).HasColumnName("CevapTarihi");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.FirmaCevapBit).HasColumnName("FirmaCevapBit");
            this.Property(t => t.CevaplaBit).HasColumnName("CevaplaBit");
            this.Property(t => t.OkunduBit).HasColumnName("OkunduBit");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.MusteriMesajs)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.Musteri)
                .WithMany(t => t.MusteriMesajs)
                .HasForeignKey(d => d.MusteriId);
            this.HasOptional(t => t.MusteriMesaj2)
                .WithMany(t => t.MusteriMesaj1)
                .HasForeignKey(d => d.MesajId);

        }
    }
}
