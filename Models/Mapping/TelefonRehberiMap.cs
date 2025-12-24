using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FotografciTakipWeb.Models.Mapping
{
    public class TelefonRehberiMap : EntityTypeConfiguration<TelefonRehberi>
    {
        public TelefonRehberiMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.FirmaAdi)
                .HasMaxLength(50);

            this.Property(t => t.AdSoyad)
                .HasMaxLength(50);

            this.Property(t => t.SabitTel1)
                .HasMaxLength(11);

            this.Property(t => t.SabitTel2)
                .HasMaxLength(11);

            this.Property(t => t.CepTel1)
                .HasMaxLength(11);

            this.Property(t => t.CepTel2)
                .HasMaxLength(11);

            this.Property(t => t.Fax)
                .HasMaxLength(11);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.Notlar)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("TelefonRehberi");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FirmaId).HasColumnName("FirmaId");
            this.Property(t => t.RehberGrupId).HasColumnName("RehberGrupId");
            this.Property(t => t.SozlesmeId).HasColumnName("SozlesmeId");
            this.Property(t => t.MusteriId).HasColumnName("MusteriId");
            this.Property(t => t.CariId).HasColumnName("CariId");
            this.Property(t => t.PersonelId).HasColumnName("PersonelId");
            this.Property(t => t.FirmaAdi).HasColumnName("FirmaAdi");
            this.Property(t => t.AdSoyad).HasColumnName("AdSoyad");
            this.Property(t => t.SabitTel1).HasColumnName("SabitTel1");
            this.Property(t => t.SabitTel2).HasColumnName("SabitTel2");
            this.Property(t => t.CepTel1).HasColumnName("CepTel1");
            this.Property(t => t.CepTel2).HasColumnName("CepTel2");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.SmsKabul).HasColumnName("SmsKabul");
            this.Property(t => t.EmailKabul).HasColumnName("EmailKabul");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.KilitBit).HasColumnName("KilitBit");
            this.Property(t => t.OlusturanKullaniciId).HasColumnName("OlusturanKullaniciId");
            this.Property(t => t.OlusturmaTarih).HasColumnName("OlusturmaTarih");
            this.Property(t => t.DegistirenKullaniciId).HasColumnName("DegistirenKullaniciId");
            this.Property(t => t.DegistirmeTarih).HasColumnName("DegistirmeTarih");
            this.Property(t => t.Aktif).HasColumnName("Aktif");
            this.Property(t => t.Sil).HasColumnName("Sil");

            // Relationships
            this.HasRequired(t => t.Firma)
                .WithMany(t => t.TelefonRehberis)
                .HasForeignKey(d => d.FirmaId);
            this.HasRequired(t => t.RehberGrup)
                .WithMany(t => t.TelefonRehberis)
                .HasForeignKey(d => d.RehberGrupId);

        }
    }
}
