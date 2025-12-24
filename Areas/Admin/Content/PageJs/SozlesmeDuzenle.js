initApp.listFilter($('#paketler_liste'), $('#paketler_liste_arama'));
initApp.listFilter($('#hizmetler_liste'), $('#hizmetler_liste_arama'));
initApp.listFilter($('#surecler_liste'), $('#surecler_liste_arama'));
// Tab Menüleri Aktif Pasif Fonksiyonları
function RezervasyonBilgileriAktif() {
    $("#Tab-RezervasyonBilgileri").addClass("active");
    $("#Tab-MusteriBilgileri").removeClass("active");
    $("#Tab-DigerBilgiler").removeClass("active");
    $("#Tab-PaketVeEkhizmetler").removeClass("active");
    $("#Tab-Surecler").removeClass("active");
    $("#Tab-CekimRandevu").removeClass("active");
    $("#Tab-OdemeBilgileri").removeClass("active");
    $("#Tab-SozlesmeOnizleme").removeClass("active");

    //$("#Tab-RezervasyonBilgileri").removeClass("disabled");
    //$("#Tab-MusteriBilgileri").addClass("disabled");
    //$("#Tab-DigerBilgiler").addClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").addClass("disabled");
    //$("#Tab-Surecler").addClass("disabled");
    //$("#Tab-CekimRandevu").addClass("disabled");
    //$("#Tab-OdemeBilgileri").addClass("disabled");
    //$("#Tab-SozlesmeOnizleme").addClass("disabled");

    $("#TabPanel-RezervasyonBilgileri").addClass("active show");
    $("#TabPanel-MusteriBilgileri").removeClass("active show");
    $("#TabPanel-DigerBilgiler").removeClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").removeClass("active show");
    $("#TabPanel-Surecler").removeClass("active show");
    $("#TabPanel-CekimRandevu").removeClass("active show");
    $("#TabPanel-OdemeBilgileri").removeClass("active show");
    $("#TabPanel-SozlesmeOnizleme").removeClass("active show");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "true");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "false");
    $("#Tab-DigerBilgiler").attr("aria-selected", "false");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "false");
    $("#Tab-Surecler").attr("aria-selected", "false");
    $("#Tab-CekimRandevu").attr("aria-selected", "false");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "false");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "false");
};
function MusteriBilgileriAktif() {
    $("#Tab-RezervasyonBilgileri").removeClass("active");
    $("#Tab-MusteriBilgileri").addClass("active");
    $("#Tab-DigerBilgiler").removeClass("active");
    $("#Tab-PaketVeEkhizmetler").removeClass("active");
    $("#Tab-Surecler").removeClass("active");
    $("#Tab-CekimRandevu").removeClass("active");
    $("#Tab-OdemeBilgileri").removeClass("active");
    $("#Tab-SozlesmeOnizleme").removeClass("active");

    //$("#Tab-RezervasyonBilgileri").addClass("disabled");
    //$("#Tab-MusteriBilgileri").removeClass("disabled");
    //$("#Tab-DigerBilgiler").addClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").addClass("disabled");
    //$("#Tab-Surecler").addClass("disabled");
    //$("#Tab-CekimRandevu").addClass("disabled");
    //$("#Tab-OdemeBilgileri").addClass("disabled");
    //$("#Tab-SozlesmeOnizleme").addClass("disabled");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "false");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "true");
    $("#Tab-DigerBilgiler").attr("aria-selected", "false");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "false");
    $("#Tab-Surecler").attr("aria-selected", "false");
    $("#Tab-CekimRandevu").attr("aria-selected", "false");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "false");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "false");

    $("#TabPanel-RezervasyonBilgileri").removeClass("active show");
    $("#TabPanel-MusteriBilgileri").addClass("active show");
    $("#TabPanel-DigerBilgiler").removeClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").removeClass("active show");
    $("#TabPanel-Surecler").removeClass("active show");
    $("#TabPanel-CekimRandevu").removeClass("active show");
    $("#TabPanel-OdemeBilgileri").removeClass("active show");
    $("#TabPanel-SozlesmeOnizleme").removeClass("active show");
};
function DigerBilgilerAktif() {
    $("#Tab-RezervasyonBilgileri").removeClass("active");
    $("#Tab-MusteriBilgileri").removeClass("active");
    $("#Tab-DigerBilgiler").addClass("active");
    $("#Tab-PaketVeEkhizmetler").removeClass("active");
    $("#Tab-Surecler").removeClass("active");
    $("#Tab-CekimRandevu").removeClass("active");
    $("#Tab-OdemeBilgileri").removeClass("active");
    $("#Tab-SozlesmeOnizleme").removeClass("active");

    //$("#Tab-RezervasyonBilgileri").addClass("disabled");
    //$("#Tab-MusteriBilgileri").addClass("disabled");
    //$("#Tab-DigerBilgiler").removeClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").addClass("disabled");
    //$("#Tab-Surecler").addClass("disabled");
    //$("#Tab-CekimRandevu").addClass("disabled");
    //$("#Tab-OdemeBilgileri").addClass("disabled");
    //$("#Tab-SozlesmeOnizleme").addClass("disabled");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "false");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "false");
    $("#Tab-DigerBilgiler").attr("aria-selected", "true");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "false");
    $("#Tab-Surecler").attr("aria-selected", "false");
    $("#Tab-CekimRandevu").attr("aria-selected", "false");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "false");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "false");

    $("#TabPanel-RezervasyonBilgileri").removeClass("active show");
    $("#TabPanel-MusteriBilgileri").removeClass("active show");
    $("#TabPanel-DigerBilgiler").addClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").removeClass("active show");
    $("#TabPanel-Surecler").removeClass("active show");
    $("#TabPanel-CekimRandevu").removeClass("active show");
    $("#TabPanel-OdemeBilgileri").removeClass("active show");
    $("#TabPanel-SozlesmeOnizleme").removeClass("active show");
};
function PaketVeEkhizmetlerAktif() {
    $("#Tab-RezervasyonBilgileri").removeClass("active");
    $("#Tab-MusteriBilgileri").removeClass("active");
    $("#Tab-DigerBilgiler").removeClass("active");
    $("#Tab-PaketVeEkhizmetler").addClass("active");
    $("#Tab-Surecler").removeClass("active");
    $("#Tab-CekimRandevu").removeClass("active");
    $("#Tab-OdemeBilgileri").removeClass("active");
    $("#Tab-SozlesmeOnizleme").removeClass("active");

    //$("#Tab-RezervasyonBilgileri").addClass("disabled");
    //$("#Tab-MusteriBilgileri").addClass("disabled");
    //$("#Tab-DigerBilgiler").addClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").removeClass("disabled");
    //$("#Tab-Surecler").addClass("disabled");
    //$("#Tab-CekimRandevu").addClass("disabled");
    //$("#Tab-OdemeBilgileri").addClass("disabled");
    //$("#Tab-SozlesmeOnizleme").addClass("disabled");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "false");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "false");
    $("#Tab-DigerBilgiler").attr("aria-selected", "false");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "true");
    $("#Tab-Surecler").attr("aria-selected", "false");
    $("#Tab-CekimRandevu").attr("aria-selected", "false");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "false");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "false");

    $("#TabPanel-RezervasyonBilgileri").removeClass("active show");
    $("#TabPanel-MusteriBilgileri").removeClass("active show");
    $("#TabPanel-DigerBilgiler").removeClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").addClass("active show");
    $("#TabPanel-Surecler").removeClass("active show");
    $("#TabPanel-CekimRandevu").removeClass("active show");
    $("#TabPanel-OdemeBilgileri").removeClass("active show");
    $("#TabPanel-SozlesmeOnizleme").removeClass("active show");
};
function SureclerAktif() {
    $("#Tab-RezervasyonBilgileri").removeClass("active");
    $("#Tab-MusteriBilgileri").removeClass("active");
    $("#Tab-DigerBilgiler").removeClass("active");
    $("#Tab-PaketVeEkhizmetler").removeClass("active");
    $("#Tab-Surecler").addClass("active");
    $("#Tab-CekimRandevu").removeClass("active");
    $("#Tab-OdemeBilgileri").removeClass("active");
    $("#Tab-SozlesmeOnizleme").removeClass("active");

    //$("#Tab-RezervasyonBilgileri").addClass("disabled");
    //$("#Tab-MusteriBilgileri").addClass("disabled");
    //$("#Tab-DigerBilgiler").addClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").addClass("disabled");
    //$("#Tab-Surecler").removeClass("disabled");
    //$("#Tab-CekimRandevu").addClass("disabled");
    //$("#Tab-OdemeBilgileri").addClass("disabled");
    //$("#Tab-SozlesmeOnizleme").addClass("disabled");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "false");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "false");
    $("#Tab-DigerBilgiler").attr("aria-selected", "false");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "false");
    $("#Tab-Surecler").attr("aria-selected", "true");
    $("#Tab-CekimRandevu").attr("aria-selected", "false");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "false");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "false");

    $("#TabPanel-RezervasyonBilgileri").removeClass("active show");
    $("#TabPanel-MusteriBilgileri").removeClass("active show");
    $("#TabPanel-DigerBilgiler").removeClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").removeClass("active show");
    $("#TabPanel-Surecler").addClass("active show");
    $("#TabPanel-CekimRandevu").removeClass("active show");
    $("#TabPanel-OdemeBilgileri").removeClass("active show");
    $("#TabPanel-SozlesmeOnizleme").removeClass("active show");
};
function CekimRandevuAktif() {
    $("#Tab-RezervasyonBilgileri").removeClass("active");
    $("#Tab-MusteriBilgileri").removeClass("active");
    $("#Tab-DigerBilgiler").removeClass("active");
    $("#Tab-PaketVeEkhizmetler").removeClass("active");
    $("#Tab-Surecler").removeClass("active");
    $("#Tab-CekimRandevu").addClass("active");
    $("#Tab-OdemeBilgileri").removeClass("active");
    $("#Tab-SozlesmeOnizleme").removeClass("active");

    //$("#Tab-RezervasyonBilgileri").addClass("disabled");
    //$("#Tab-MusteriBilgileri").addClass("disabled");
    //$("#Tab-DigerBilgiler").addClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").addClass("disabled");
    //$("#Tab-Surecler").addClass("disabled");
    //$("#Tab-CekimRandevu").removeClass("disabled");
    //$("#Tab-OdemeBilgileri").addClass("disabled");
    //$("#Tab-SozlesmeOnizleme").addClass("disabled");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "false");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "false");
    $("#Tab-DigerBilgiler").attr("aria-selected", "false");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "false");
    $("#Tab-Surecler").attr("aria-selected", "false");
    $("#Tab-CekimRandevu").attr("aria-selected", "true");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "false");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "false");

    $("#TabPanel-RezervasyonBilgileri").removeClass("active show");
    $("#TabPanel-MusteriBilgileri").removeClass("active show");
    $("#TabPanel-DigerBilgiler").removeClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").removeClass("active show");
    $("#TabPanel-Surecler").removeClass("active show");
    $("#TabPanel-CekimRandevu").addClass("active show");
    $("#TabPanel-OdemeBilgileri").removeClass("active show");
    $("#TabPanel-SozlesmeOnizleme").removeClass("active show");
};
function OdemeBilgileriAktif() {
    $("#Tab-RezervasyonBilgileri").removeClass("active");
    $("#Tab-MusteriBilgileri").removeClass("active");
    $("#Tab-DigerBilgiler").removeClass("active");
    $("#Tab-PaketVeEkhizmetler").removeClass("active");
    $("#Tab-Surecler").removeClass("active");
    $("#Tab-CekimRandevu").removeClass("active");
    $("#Tab-OdemeBilgileri").addClass("active");
    $("#Tab-SozlesmeOnizleme").removeClass("active");

    //$("#Tab-RezervasyonBilgileri").addClass("disabled");
    //$("#Tab-MusteriBilgileri").addClass("disabled");
    //$("#Tab-DigerBilgiler").addClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").addClass("disabled");
    //$("#Tab-Surecler").addClass("disabled");
    //$("#Tab-CekimRandevu").addClass("disabled");
    //$("#Tab-OdemeBilgileri").removeClass("disabled");
    //$("#Tab-SozlesmeOnizleme").addClass("disabled");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "false");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "false");
    $("#Tab-DigerBilgiler").attr("aria-selected", "false");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "false");
    $("#Tab-Surecler").attr("aria-selected", "false");
    $("#Tab-CekimRandevu").attr("aria-selected", "false");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "true");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "false");

    $("#TabPanel-RezervasyonBilgileri").removeClass("active show");
    $("#TabPanel-MusteriBilgileri").removeClass("active show");
    $("#TabPanel-DigerBilgiler").removeClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").removeClass("active show");
    $("#TabPanel-Surecler").removeClass("active show");
    $("#TabPanel-CekimRandevu").removeClass("active show");
    $("#TabPanel-OdemeBilgileri").addClass("active show");
    $("#TabPanel-SozlesmeOnizleme").removeClass("active show");
};
function SozlesmeOnizlemeAktif() {
    $("#Tab-RezervasyonBilgileri").removeClass("active");
    $("#Tab-MusteriBilgileri").removeClass("active");
    $("#Tab-DigerBilgiler").removeClass("active");
    $("#Tab-PaketVeEkhizmetler").removeClass("active");
    $("#Tab-Surecler").removeClass("active");
    $("#Tab-CekimRandevu").removeClass("active");
    $("#Tab-OdemeBilgileri").removeClass("active");
    $("#Tab-SozlesmeOnizleme").addClass("active");

    //$("#Tab-RezervasyonBilgileri").addClass("disabled");
    //$("#Tab-MusteriBilgileri").addClass("disabled");
    //$("#Tab-DigerBilgiler").addClass("disabled");
    //$("#Tab-PaketVeEkhizmetler").addClass("disabled");
    //$("#Tab-Surecler").addClass("disabled");
    //$("#Tab-CekimRandevu").addClass("disabled");
    //$("#Tab-OdemeBilgileri").addClass("disabled");
    //$("#Tab-SozlesmeOnizleme").removeClass("disabled");

    $("#Tab-RezervasyonBilgileri").attr("aria-selected", "false");
    $("#Tab-MusteriBilgileri").attr("aria-selected", "false");
    $("#Tab-DigerBilgiler").attr("aria-selected", "false");
    $("#Tab-PaketVeEkhizmetler").attr("aria-selected", "false");
    $("#Tab-Surecler").attr("aria-selected", "false");
    $("#Tab-CekimRandevu").attr("aria-selected", "false");
    $("#Tab-OdemeBilgileri").attr("aria-selected", "false");
    $("#Tab-SozlesmeOnizleme").attr("aria-selected", "true");

    $("#TabPanel-RezervasyonBilgileri").removeClass("active show");
    $("#TabPanel-MusteriBilgileri").removeClass("active show");
    $("#TabPanel-DigerBilgiler").removeClass("active show");
    $("#TabPanel-PaketVeEkhizmetler").removeClass("active show");
    $("#TabPanel-Surecler").removeClass("active show");
    $("#TabPanel-CekimRandevu").removeClass("active show");
    $("#TabPanel-OdemeBilgileri").removeClass("active show");
    $("#TabPanel-SozlesmeOnizleme").addClass("active show");
};
function dateFormat(d) {
    //console.log(d);
    //console.log((d.getDate() + "").padStart(2, "0") + "." + ((d.getMonth() + 1) + "").padStart(2, "0") + "." + d.getFullYear());
    return (d.getDate() + "").padStart(2, "0") + "." + ((d.getMonth() + 1) + "").padStart(2, "0") + "." + d.getFullYear();
}
var controls = {
    leftArrow: '<i class="fal fa-angle-left" style="font-size: 1.25rem"></i>',
    rightArrow: '<i class="fal fa-angle-right" style="font-size: 1.25rem"></i>'
}
var runDatePicker = function () {
    $('#SozlesmeTarihi').datepicker(
       {
           todayBtn: "linked",
           todayHighlight: true,
           templates: controls,
           language: 'tr',
       }).datepicker("setDate", new Date());
    $('#OpsiyonTarihi').datepicker(
       {
           todayBtn: "linked",
           todayHighlight: true,
           templates: controls,
           language: 'tr'
       }).datepicker("setDate", new Date());
    $('#RezervasyonTarihi').datepicker(
       {
           todayBtn: "linked",
           todayHighlight: true,
           templates: controls,
           language: 'tr'
       }).datepicker("setDate", new Date());
    $('#OdemeTarihi').datepicker(
       {
           todayBtn: "linked",
           todayHighlight: true,
           templates: controls,
           language: 'tr'
       }).datepicker("setDate", new Date());
    $('#KalanOdemeTarihi').datepicker(
       {
           todayBtn: "linked",
           todayHighlight: true,
           templates: controls,
           language: 'tr'
       }).datepicker("setDate", new Date());
    $('#CekimTarihi').datepicker(
       {
           todayBtn: "linked",
           todayHighlight: true,
           templates: controls,
           language: 'tr'
       }).datepicker("setDate", new Date());
}
var sozlesmeId; var sozlesmeNo; var musteriid = 0; var randevuId;
$(":input").inputmask();

$('#slimtest1').slimScroll(
       {
           position: 'right',
           height: '250px',
           railVisible: true,
           alwaysVisible: true
       });
$('#slimtest2').slimScroll(
       {
           position: 'right',
           height: '250px',
           railVisible: true,
           alwaysVisible: true
       });
$('#slimtest3').slimScroll(
       {
           position: 'right',
           height: '250px',
           railVisible: true,
           alwaysVisible: true
       });
$('#slimtest4').slimScroll(
       {
           position: 'right',
           height: '250px',
           railVisible: true,
           alwaysVisible: true
       });
$('#slimtest5').slimScroll(
       {
           position: 'right',
           height: '350px',
           railVisible: true,
           alwaysVisible: true
       });
$("#PaketTutar").inputmask("currency",
        {
            placeholder: "0.00", // Varsayılan değer
            prefix: "", // öndeki değer
            suffix: " TL", // sondaki değer
            rightAlign: false, // sağa-sola yaslama
            radixPoint: ",", // kuruş ayıracı
            groupSeparator: "." // binlik ayıracı
        });
$("#Iskonto").inputmask("currency",
        {
            placeholder: "0.00", // Varsayılan değer
            prefix: "", // öndeki değer
            suffix: " TL", // sondaki değer
            rightAlign: false, // sağa-sola yaslama
            radixPoint: ",", // kuruş ayıracı
            groupSeparator: "." // binlik ayıracı
        });
$("#EkHizmetTutar").inputmask("currency",
        {
            placeholder: "0.00", // Varsayılan değer
            prefix: "", // öndeki değer
            suffix: " TL", // sondaki değer
            rightAlign: false, // sağa-sola yaslama
            radixPoint: ",", // kuruş ayıracı
            groupSeparator: "." // binlik ayıracı
        });
$("#ToplamTutar").inputmask("currency",
        {
            placeholder: "0.00", // Varsayılan değer
            prefix: "", // öndeki değer
            suffix: " TL", // sondaki değer
            rightAlign: false, // sağa-sola yaslama
            radixPoint: ",", // kuruş ayıracı
            groupSeparator: "." // binlik ayıracı
        });
$("#OdemeTutar").inputmask("currency",
        {
            placeholder: "0.00", // Varsayılan değer
            prefix: "", // öndeki değer
            suffix: " TL", // sondaki değer
            rightAlign: false, // sağa-sola yaslama
            radixPoint: ",", // kuruş ayıracı
            groupSeparator: "." // binlik ayıracı
        });
$("#KalanOdemeTutar").inputmask("currency",
       {
           placeholder: "0.00", // Varsayılan değer
           prefix: "", // öndeki değer
           suffix: " TL", // sondaki değer
           rightAlign: false, // sağa-sola yaslama
           radixPoint: ",", // kuruş ayıracı
           groupSeparator: "." // binlik ayıracı
       });
$(document).ready(function () {
    $(function () {
        $('#MusteriId').select2();
    });
    // Sayfadan ayrılmak istenince uyarı veriyor.
    //window.addEventListener('beforeunload', function (e) {
    //    e.preventDefault();
    //    e.returnValue = '';
    //});

    runDatePicker();
    RezervasyonBilgileriAktif();
    // ------  Personel İzin Kontrolü
    $("#PersonelId").change(function () {
        var PersonelId = $(this).find("option:selected").val();
        var Personel = $(this).find("option:selected").text();
        var PersonelData = new FormData();
        PersonelData.append("RezervasyonTarihi", $("#RezervasyonTarihi").val());
        PersonelData.append("PersonelId", PersonelId);
        $.ajax({
            type: "POST",
            datatype: 'json',
            data: PersonelData,
            contentType: false,
            processData: false,
            url: "/Otomasyon/RezervasyonIslemleri/PersonelIzinKontrol/",
            success: function (data) {
                if (data.Sonuc == false) {
                    Swal.fire(
                    {
                        title: "Dikkat",
                        html: "<p><h3 style='color:red'>&laquo; " + Personel + " &raquo;<h3/></p> <p><h3>" + data.Mesaj + "<h3/></p>",
                        type: "warning",
                        confirmButtonText: "Tamam"
                    });
                    $("#PersonelId").val("0");
                }
                //else if (data.Sonuc == true) {

                //}
            },
            error: function (err) {
                alert("HATA");
            }
        });
    });
    // ------  Personel İzin Kontrolü
    // ------  Rezervasyon Sekli seçimi
    $("#RezervasyonSekliId").change(function () {
        var rezervasyonsekli = $(this).find("option:selected").val();
        if (rezervasyonsekli == 1) {
            $("#OpsiyonTarihi").attr("disabled", "true");
        }
        else {
            $("#OpsiyonTarihi").removeAttr("disabled");
        }
    });
    // ------  Rezervasyon Sekli seçimi

    // ------  Zaman Dilimi seçimi
    $("#ZamanDilimiId").change(function () {
        var zamandilimi = $(this).find("option:selected").val();
        if (zamandilimi != 0) {
            $("#BaslangicZaman").attr("disabled", "true");
            $("#BitisZaman").attr("disabled", "true")
        }
        else {
            $("#BaslangicZaman").removeAttr("disabled");
            $("#BitisZaman").removeAttr("disabled");
        }
    });
    // ------  Zaman Dilimi seçimi

    // ------  Rezervasyon Türü seçimi
    $("#RezervasyonTurId").change(function () {
        if ($(this).find("option:selected").data("formalan") == "Gelin/Damat") {
            $(".Gelin").removeAttr("style", "display:none");
            $(".Damat").removeAttr("style", "display:none");
            $(".BebekCocuk").attr("style", "display:none");
            $(".UrunKatalog").attr("style", "display:none");
            $(".ModelManken").attr("style", "display:none");
            $(".Yetkili").attr("style", "display:none");
        }
        else if ($(this).find("option:selected").data("formalan") == "Bebek/Çocuk") {
            $(".Gelin").attr("style", "display:none");
            $(".Damat").attr("style", "display:none");
            $(".BebekCocuk").removeAttr("style", "display:none");
            $(".UrunKatalog").attr("style", "display:none");
            $(".ModelManken").attr("style", "display:none");
            $(".Yetkili").attr("style", "display:none");
        }
        else if ($(this).find("option:selected").data("formalan") == "Ürün/Katalog") {
            $(".Gelin").attr("style", "display:none");
            $(".Damat").attr("style", "display:none");
            $(".BebekCocuk").attr("style", "display:none");
            $(".UrunKatalog").removeAttr("style", "display:none");
            $(".ModelManken").attr("style", "display:none");
            $(".Yetkili").attr("style", "display:none");
        }
        else if ($(this).find("option:selected").data("formalan") == "Model/Manken") {
            $(".Gelin").attr("style", "display:none");
            $(".Damat").attr("style", "display:none");
            $(".BebekCocuk").attr("style", "display:none");
            $(".UrunKatalog").attr("style", "display:none");
            $(".ModelManken").removeAttr("style", "display:none");
            $(".Yetkili").attr("style", "display:none");
        }
        else if ($(this).find("option:selected").data("formalan") == "Yetkili") {
            $(".Gelin").attr("style", "display:none");
            $(".Damat").attr("style", "display:none");
            $(".BebekCocuk").attr("style", "display:none");
            $(".UrunKatalog").attr("style", "display:none");
            $(".ModelManken").attr("style", "display:none");
            $(".Yetkili").removeAttr("style", "display:none");
        }
        else {
            $(".Gelin").removeAttr("style", "display:none");
            $(".Damat").removeAttr("style", "display:none");
            $(".BebekCocuk").removeAttr("style", "display:none");
            $(".UrunKatalog").removeAttr("style", "display:none");
            $(".ModelManken").removeAttr("style", "display:none");
            $(".Yetkili").removeAttr("style", "display:none");
        }
    });
    // ------   Rezervasyon Türü seçimi

    // ------ Müşteri Listesinde Müşteri seçimi
    $("#MusteriId").change(function () {
        musteriid = $(this).find("option:selected").val();
        var musterikodu = $(this).find("option:selected").data("musterikodu");
        var adsoyad = $(this).find("option:selected").data("adsoyad");
        var tckimlik = $(this).find("option:selected").data("tckimlikno");
        var email = $(this).find("option:selected").data("email");
        var ceptel = $(this).find("option:selected").data("ceptel");
        var sabittel = $(this).find("option:selected").data("sabittel");
        var adres = $(this).find("option:selected").data("adres");
        var notlar = $(this).find("option:selected").data("notlar");
        if (musteriid != 0) {
            adres = adres.replace(/\<br \/\>/g, "\r\n");
            notlar = notlar.replace(/\<br \/\>/g, "\r\n");
            $("#MusteriKodu").val(musterikodu);
            $("#AdSoyad").val(adsoyad);
            $("#TCKimlikNo").val(tckimlik);
            $("#Email").val(email);
            $("#CepTel").val(ceptel);
            $("#SabitTel").val(sabittel);
            $("#Adres").val(adres);
            $("#Notlar").val(notlar);
        }
        else {
            $("#MusteriKodu").val(musterikodu);
            $("#AdSoyad").val("");
            $("#TCKimlikNo").val("");
            $("#Email").val("");
            $("#CepTel").val("");
            $("#SabitTel").val("");
            $("#Adres").val("");
            $("#Notlar").val("");
        }
    });
    // ------ Müşteri Listesinde Müşteri seçimi
    var OdemeAlDurum = false;
    // ------  Paket ve EkHizmetler Sekmesi hesaplama ve yazma işlemleri  ------ //
    var formattedOutput = new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY',
        minimumFractionDigits: 2,
    });
    var ToplamTutar = $("#ToplamTutarVT").val().replace(" TL", "");
    ToplamTutar = ToplamTutar.replace(".", "");
    ToplamTutar = ToplamTutar.replace(",", ".");
    var paketfiyat = 0;
    var toplampaketfiyat = $("#PaketTutarVT").val().replace(" TL", "");
    toplampaketfiyat = toplampaketfiyat.replace(".", "");
    toplampaketfiyat = toplampaketfiyat.replace(",", ".");
    var ekhizmetfiyat = 0;
    var toplamekhizmetfiyat = $("#EkHizmetTutarVT").val().replace(" TL", "");
    toplamekhizmetfiyat = toplamekhizmetfiyat.replace(".", "");
    toplamekhizmetfiyat = toplamekhizmetfiyat.replace(",", ".");
    var iskonto = $("#IskontoVT").val().replace(" TL", "");
    iskonto = iskonto.replace(".", "");
    iskonto = iskonto.replace(",", ".");
 
    var alacaktoplam = $("#AlacaklarToplam").val().replace(" TL", "");
    alacaktoplam = alacaktoplam.replace(".", "");
    alacaktoplam = alacaktoplam.replace(",", ".");

    var odemetoplam = $("#OdemelerToplam").val().replace(" TL", "");
    odemetoplam = odemetoplam.replace(".", "");
    odemetoplam = odemetoplam.replace(",", ".");

    var toplam = $("#ToplamTutarVT").val().replace(" TL", "");
    toplam = toplam.replace(".", "");
    toplam = toplam.replace(",", ".");
    $("#sozlesmetutari").html(accounting.formatMoney(toplam, "TL", 2, ".", ",", "%v %s"));
    $("#Odemetoplam").html(accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));

    var kalan = parseFloat(toplam) - (parseFloat(odemetoplam) + parseFloat(alacaktoplam));
    $("#sozlesmekalan").html(accounting.formatMoney(kalan, "TL", 2, ".", ",", "%v %s"));
    $("#kalanalacak").html(accounting.formatMoney(kalan, "TL", 2, ".", ",", "%v %s"));

    var secilenpaketID = [];
    var secilenekhizmetID = [];
    var secilensurecID = [];
    secilenpaketID = $("#ModelPaketlerId").val();
    secilenekhizmetID = $("#ModelEkHizmetlerId").val();
    secilensurecID = $("#ModelSureclerId").val();

    // ------ Paketler Listesindeki paket seçimi
    var PaketListesi = $("#Paketler :checkbox");
    PaketListesi.on("change", function () {
        SecilenPaketler();
    });
    function SecilenPaketler() {
        secilenpaketID = [];
        $("#slimtest2").empty();
        toplampaketfiyat = 0;
        if (secilenpaketID.length == 0) {
            paketfiyat = 0;
            toplampaketfiyat = 0;
            ToplamTutar = parseFloat(toplampaketfiyat) + parseFloat(toplamekhizmetfiyat);
            iskonto = $("#Iskonto").val().replace(" TL", "");
            iskonto = iskonto.replace(".", "");
            iskonto = iskonto.replace(",", ".");
            if (iskonto != "") {
                var ToplamTutar2 = parseFloat(ToplamTutar) - parseFloat(iskonto);
                $("#PaketTutar").val(formattedOutput.format(parseFloat(toplampaketfiyat)));
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar2)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
            }
            else {
                $("#PaketTutar").val(formattedOutput.format(parseFloat(toplampaketfiyat)));
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
            }
        }
        PaketListesi.filter(":checked").each(function () {
            secilenpaketID.push($(this).data("paketid"));
            console.log("secilenpaketidleri=" + secilenpaketID);
            console.log("secilenekhizmetID=" + secilenekhizmetID);
            paketfiyat = $(this).data("fiyat").replace(" TL", "");
            paketfiyat = paketfiyat.replace(".", "");
            paketfiyat = paketfiyat.replace(",", ".");
            toplampaketfiyat = toplampaketfiyat + parseFloat(paketfiyat);
            //ToplamTutar = ToplamTutar + parseFloat(toplampaketfiyat) + parseFloat(toplamekhizmetfiyat);
            $("#PaketTutar").val(formattedOutput.format(parseFloat(toplampaketfiyat)));
            $("#slimtest2").append("<div class='alert alert-primary mb-2'>" +
                                               "<h5 class='card-title'>" + $(this).val() + "<span class='fw-700'> - " + $(this).data("fiyat") + "</span></h5>" +
                                               $(this).data("paketdetay") +
                                               "</div>");
            ToplamTutar = parseFloat(toplampaketfiyat) + parseFloat(toplamekhizmetfiyat);
            iskonto = $("#Iskonto").val().replace(" TL", "");
            iskonto = iskonto.replace(".", "");
            iskonto = iskonto.replace(",", ".");
            if (iskonto != "") {
                var ToplamTutar2 = parseFloat(ToplamTutar) - parseFloat(iskonto);
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar2)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
            }
            else {
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
            }
        });

    }
    // ------ Paketler Listesindeki paket seçimi
    // ------ Ek Hizmetler Listesindeki paket seçimi
    var EkHizmetListesi = $("#EkHizmetler :checkbox");
    EkHizmetListesi.on("change", function () {
        SecilenEkHizmetler();
    });
    function SecilenEkHizmetler() {
        secilenekhizmetID = [];
        $("#slimtest4").empty();
        toplamekhizmetfiyat = 0;
        if (secilenekhizmetID.length == 0) {
            ekhizmetfiyat = 0;
            toplamekhizmetfiyat = 0;
            ToplamTutar = parseFloat(toplampaketfiyat) + parseFloat(toplamekhizmetfiyat);
            iskonto = $("#Iskonto").val().replace(" TL", "");
            iskonto = iskonto.replace(".", "");
            iskonto = iskonto.replace(",", ".");
            if (iskonto != "") {
                var ToplamTutar2 = parseFloat(ToplamTutar) - parseFloat(iskonto);
                $("#EkHizmetTutar").val(formattedOutput.format(parseFloat(toplamekhizmetfiyat)));
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar2)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
            }
            else {
                $("#EkHizmetTutar").val(formattedOutput.format(parseFloat(toplamekhizmetfiyat)));
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
            }
        }
        EkHizmetListesi.filter(":checked").each(function () {
            secilenekhizmetID.push($(this).data("ekhizmetid"));
            ekhizmetfiyat = $(this).data("fiyat").replace(" TL", "");
            ekhizmetfiyat = ekhizmetfiyat.replace(".", "");
            ekhizmetfiyat = ekhizmetfiyat.replace(",", ".");
            toplamekhizmetfiyat = toplamekhizmetfiyat + parseFloat(ekhizmetfiyat);
            $("#EkHizmetTutar").val(formattedOutput.format(parseFloat(toplamekhizmetfiyat)));
            $("#slimtest4").append("<div class='alert alert-secondary mb-2'>" +
                                               "<h5 class='card-title'>" + $(this).val() + "<span class='fw-700'> - " + $(this).data("fiyat") + "</span></h5>" +
                                               $(this).data("ekhizmetdetay") +
                                               "</div>");
            //ToplamTutar = 0;
            ToplamTutar = parseFloat(toplampaketfiyat) + parseFloat(toplamekhizmetfiyat);

            iskonto = $("#Iskonto").val().replace(" TL", "");
            iskonto = iskonto.replace(".", "");
            iskonto = iskonto.replace(",", ".");
            if (iskonto != "") {
                var ToplamTutar2 = parseFloat(ToplamTutar) - parseFloat(iskonto);
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar2)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
            }
            else {
                $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar)));
                $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
                $("#kalanalacak").html(accounting.formatMoney(ToplamTutar, "TL", 2, ".", ",", "%v %s"));
            }
        });
    }
    // ------ Ek Hizmetler Listesindeki hizmet seçimi
    // ------ Süreçler Listesindeki süreç seçimi
    var SurecListesi = $("#Surecler :checkbox");
    SurecListesi.on("change", function () {
        SecilenSurecler();
    });
    function SecilenSurecler() {
        secilensurecID = [];
        SurecListesi.filter(":checked").each(function () {
            secilensurecID.push($(this).data("surecid"));
        });
    }
    // ------ Süreçler Listesindeki süreç seçimi

    $(".tutarlar").on('change', function () {
        iskonto = $("#Iskonto").val().replace(" TL", "");
        iskonto = iskonto.replace(".", "");
        iskonto = iskonto.replace(",", ".");
        console.log("İskonto Öncesi: " + ToplamTutar);
        var ToplamTutar2 = parseFloat(ToplamTutar) - parseFloat(iskonto);
        console.log("İskonto Sonrası: " + ToplamTutar2);
        $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar2)));
        $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
        $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
        $("#kalanalacak").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
    });

    $("#Iskonto").keypress(function () {
        iskonto = $("#Iskonto").val().replace(" TL", "");
        iskonto = iskonto.replace(".", "");
        iskonto = iskonto.replace(",", ".");
        console.log("İskonto Öncesi: " + ToplamTutar);
        var ToplamTutar2 = parseFloat(ToplamTutar) - parseFloat(iskonto);
        console.log("İskonto Sonrası: " + ToplamTutar2);
        $("#ToplamTutar").val(formattedOutput.format(parseFloat(ToplamTutar2)));
        $("#sozlesmetutari").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
        $("#sozlesmekalan").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
        $("#kalanalacak").html(accounting.formatMoney(ToplamTutar2, "TL", 2, ".", ",", "%v %s"));
    });

    // ------  Paket ve EkHizmetler Sekmesi hesaplama ve yazma işlemleri  ------ //

    // ------ Alınan ve Gelecek Ödeme Ekleme/Silme İşlemleri
    $("#AlacakEkle").click(function () {
        var alacakgirilentutar = $("#KalanOdemeTutar").val(); // Eklenecek borç textboxtan okunuyor.
        alacakgirilentutar = alacakgirilentutar.replace(" TL", "");
        alacakgirilentutar = alacakgirilentutar.replace(".", "");
        alacakgirilentutar = alacakgirilentutar.replace(",", ".");

        var kalanalacaktutar = $("#kalanalacak").html(); // Kalan borç ilgili alandan okunuyor.
        kalanalacaktutar = kalanalacaktutar.replace("TL", "");
        kalanalacaktutar = kalanalacaktutar.replace(".", "");
        kalanalacaktutar = kalanalacaktutar.replace(",", ".");

        if (parseFloat(alacakgirilentutar) <= parseFloat(kalanalacaktutar)) { // Borç olarak girilen değer kalan borç miktarından küçük olmalıdır. değilse uyarı ver.
            if ($("#KalanOdemeTutar").val() == "0" || $("#KalanOdemeTutar").val() == "") { // validation kontrol
                $("#KalanOdemeTutar").focus();
                $("#KalanTutarHata").append("<div class='hata' style='color:red'>Tutar 0(sıfır) olamaz.</div>");
            }
            else {
                var AlacakData = new FormData();
                AlacakData.append("SozlesmeId", sozlesmeId);
                AlacakData.append("SozlesmeNo", sozlesmeNo);
                AlacakData.append("MusteriId", musteriid);
                AlacakData.append("KalanOdemeTarihi", $("#KalanOdemeTarihi").val());
                AlacakData.append("KalanOdemeTutar", $("#KalanOdemeTutar").val());
                AlacakData.append("KalanOdemeAciklama", $("#KalanOdemeAciklama").val());
                $.ajax({
                    type: "POST",
                    datatype: 'json',
                    data: AlacakData,
                    contentType: false,
                    processData: false,
                    url: "/Otomasyon/RezervasyonIslemleri/AlacakEkle/",
                    success: function (data) {
                        alacaktoplam = 0; // global değişken sıfırlanıyor.
                        var alacaktutar = 0;
                        $("#gelecekodemeler-satir").empty();
                        $.each(data, function (index, item) {
                            $.each(item, function (index_1, item_1) {
                                alacaktutar = item_1.Tutar.replace(" TL", "");
                                alacaktutar = alacaktutar.replace(".", "");
                                alacaktutar = alacaktutar.replace(",", ".");
                                alacaktoplam = alacaktoplam + parseFloat(alacaktutar);
                                if (item_1.OdemeAl == true) {
                                    $("#gelecekodemeler-satir").append("<tr class='bg-primary-50'>" +
                                                                   "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                    "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                    "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 alacaksmsgonder' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Ödeme Al'>" +
                                                                           "<i class='fal fa-plus-octagon'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 alacaksil' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Borcu Sil'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                                }
                                else {
                                    $("#gelecekodemeler-satir").append("<tr>" +
                                                                      "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                       "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                       "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                       "<td class='text-center'>" +
                                                                           "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 alacaksmsgonder' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                               "<i class='fal fa-mobile-alt'></i>" +
                                                                           "</a>" +
                                                                           "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemeal' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Ödeme Al'>" +
                                                                              "<i class='fal fa-plus-octagon'></i>" +
                                                                           "</a>" +
                                                                           "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 alacaksil' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Borcu Sil'>" +
                                                                              "<i class='fal fa-trash-alt'></i>" +
                                                                           "</a>" +
                                                                       "</td>" +
                                                                 "</tr>");

                                }
                            });
                        });
                        kalanalacaktutar = parseFloat(kalanalacaktutar) - parseFloat(alacakgirilentutar); // Kalan alacak miktarı ilgili alandan okunmuştu. bu miktardan toplam alacak tutarı düşülerek ilgili alana yeniden yazılıyor.
                        $("#kalanalacak").html(accounting.formatMoney(kalanalacaktutar, "TL", 2, ".", ",", "%v %s"));
                        //kalanbakiye = parseFloat(kalanbakiye) - parseFloat(alacaktutar);
                        //$("#Odemetoplam").html(accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));
                        //$("#kalanalacak").html(accounting.formatMoney(alacakkalan, "TL", 2, ".", ",", "%v %s"));
                        //$("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));
                        $("#KalanOdemeTarihi").val(dateFormat(new Date()));
                        $("#KalanOdemeTutar").val("");
                        $("#KalanOdemeAciklama").val("");
                    },
                    error: function (err) {
                        alert("HATA");
                    }
                });
            }
        }
        else {
            Swal.fire(
                        {
                            title: "Borç Uyarı",
                            //text: "<< " + baslangic + " - " + bitis + " => " + aciklama + " >> Tatil Günü zaten var!",
                            html: "<p style='color:red'> Sözleşme Tutarından veya Kalan Tutardan fazla bir borç ekleyemezsiniz!</p>",
                            type: "warning",
                            //showCancelButton: true,
                            confirmButtonText: "Tamam"
                        });
        }

    });
    $("#tb-gelecekodemeler").on('click', '.alacaksil', function () {
        var id = $(this).data("alacakid");
        var tarih = $(this).data("alacaktarih");
        var tutar = $(this).data("alacaktutar");

        var kalanalacaktutar = $("#kalanalacak").html(); // Kalan borç ilgili alandan okunuyor.
        kalanalacaktutar = kalanalacaktutar.replace("TL", "");
        kalanalacaktutar = kalanalacaktutar.replace(".", "");
        kalanalacaktutar = kalanalacaktutar.replace(",", ".");

        var silinentutar = tutar.replace(" TL", "");
        silinentutar = silinentutar.replace(".", "");
        silinentutar = silinentutar.replace(",", ".");

        Swal.fire(
        {
            title: "Kalan Ödeme Sil",
            //text: "< " + giskategori + " > adlı Günlük İş Katergorisi silinsin mi?",
            html: "<p style='color:red;font-weight:bold;font-size:12pt;'>&laquo; " + tarih + " - " + tutar + " &raquo;</p> <p style='color:red;font-size:12pt;'><h3> Kalan Ödeme silinsin mi?</h3></p> ",
            type: "error",
            showCancelButton: true,
            confirmButtonText: "Sil!"
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    datatype: 'json',
                    contentType: false,
                    processData: false,
                    url: "/Otomasyon/RezervasyonIslemleri/AlacakSil/" + id,
                    success: function (data) {
                        var alacaktoplam = 0;
                        var alacaktutar = 0;
                        $("#gelecekodemeler-satir").empty();
                        $.each(data, function (index, item) {
                            $.each(item, function (index_1, item_1) {
                                alacaktutar = item_1.Tutar.replace(" TL", "");
                                alacaktutar = alacaktutar.replace(".", "");
                                alacaktutar = alacaktutar.replace(",", ".");
                                alacaktoplam = alacaktoplam + parseFloat(alacaktutar);
                                if (item_1.OdemeAl == true) {
                                    $("#gelecekodemeler-satir").append("<tr class='bg-primary-50'>" +
                                                                  "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                   "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                   "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                   "<td class='text-center'>" +
                                                                       "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 alacaksmsgonder' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                           "<i class='fal fa-mobile-alt'></i>" +
                                                                       "</a>" +
                                                                       "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemeal' data-toggle='modal' data-target='#OdemeAlModal' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Ödeme Al'>" +
                                                                          "<i class='fal fa-plus-octagon'></i>" +
                                                                       "</a>" +
                                                                       "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 alacaksil' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Borcu Sil'>" +
                                                                          "<i class='fal fa-trash-alt'></i>" +
                                                                       "</a>" +
                                                                   "</td>" +
                                                             "</tr>");
                                }
                                else {
                                    $("#gelecekodemeler-satir").append("<tr>" +
                                                                  "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                   "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                   "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                   "<td class='text-center'>" +
                                                                       "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 alacaksmsgonder' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                           "<i class='fal fa-mobile-alt'></i>" +
                                                                       "</a>" +
                                                                       "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemeal' data-toggle='modal' data-target='#OdemeAlModal' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Ödeme Al'>" +
                                                                          "<i class='fal fa-plus-octagon'></i>" +
                                                                       "</a>" +
                                                                       "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 alacaksil' data-alacakid='" + item_1.Id + "' data-alacaktarih='" + item_1.Tarih + "' data-alacaktutar='" + item_1.Tutar + "' title='Borcu Sil'>" +
                                                                          "<i class='fal fa-trash-alt'></i>" +
                                                                       "</a>" +
                                                                   "</td>" +
                                                             "</tr>");
                                }

                            });
                        });
                        //alacakkalan = parseFloat(kalanbakiye) - parseFloat(alacaktutar);
                        //$("#Odemetoplam").html(accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));
                        var kalanbakiye = parseFloat(kalanalacaktutar) + parseFloat(silinentutar);
                        $("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));
                        $("#KalanOdemeTarihi").val(dateFormat(new Date()));
                        $("#KalanOdemeTutar").val("");
                        $("#KalanOdemeAciklama").val("");
                    },
                    error: function (err) {
                        alert("HATA");
                    }
                });
            }
        });
    }); // Kalan Ödeme Sil
    $("#tb-gelecekodemeler").on('click', '.odemeal', function () {
        var id = $(this).data("alacakid");
        var tarih = $(this).data("alacaktarih");
        var tutar = $(this).data("alacaktutar");
        $("#OdemeTarihi").data("alacakid", id)
        $("#OdemeTarihi").data("alacaktutar", tutar)
        $("#OdemeTarihi").val($(this).data("alacaktarih"));
        $("#OdemeTutar").val($(this).data("alacaktutar"));
        $("#OdemeSekli").focus();
        OdemeAlDurum = true;
    }); // BorcuÖdemeye dönüştür

    $("#OdemeAl").click(function () {
        var OdemeAlId = $("#OdemeTarihi").data("alacakid");
        var alacaktutar = $("#OdemeTarihi").data("alacaktutar");
        var kalanbakiye = $("#sozlesmekalan").html();
        kalanbakiye = kalanbakiye.replace(" TL", "");
        kalanbakiye = kalanbakiye.replace(".", "");
        kalanbakiye = kalanbakiye.replace(",", ".");

        alacaktutar = alacaktutar.replace(" TL", "");
        alacaktutar = alacaktutar.replace(".", "");
        alacaktutar = alacaktutar.replace(",", ".");

        var girilentutar = $("#OdemeTutar").val();
        girilentutar = girilentutar.replace(" TL", "");
        girilentutar = girilentutar.replace(".", "");
        girilentutar = girilentutar.replace(",", ".");
        console.log("Alacak Tutar= " + alacaktutar);
        console.log("Girilen Tutar= " + girilentutar);

        var SozlesmeTutar = $("#sozlesmetutari").html();
        SozlesmeTutar = SozlesmeTutar.replace(" TL", "");
        SozlesmeTutar = SozlesmeTutar.replace(".", "");
        SozlesmeTutar = SozlesmeTutar.replace(",", ".");
        if (OdemeAlDurum == true) { // Eğer Borçtan ödeme alınıyorsa kalan bakiye kontrolü yapılmayacak
            if (alacaktutar != girilentutar) {
                Swal.fire(
                       {
                           title: "Borç Uyarı",
                           html: "<p style='color:red'> Girilen Tutar ile Alacak Tutar aynı değil!</p>",
                           type: "warning",
                           confirmButtonText: "Tamam"
                       });
            }
            else
            {
                if ($("#OdemeTutar").val() == "0" || $("#OdemeTutar").val() == "") {
                    $("#OdemeTutar").focus();
                    $("#OdemeTutarHata").append("<div class='hata' style='color:red'>Tutar 0(sıfır) olamaz.</div>");
                }
                else {
                    console.log("OdemeTutar=" + $("#OdemeTutar").val());
                    alert($("#OdemeTutar").val());
                    var AlinanOdemeData = new FormData();
                    AlinanOdemeData.append("SozlesmeId", sozlesmeId);
                    AlinanOdemeData.append("SozlesmeNo", sozlesmeNo);
                    AlinanOdemeData.append("OdemeAdSoyad", $("#AdSoyad").val());
                    AlinanOdemeData.append("MusteriId", musteriid);
                    AlinanOdemeData.append("OdemeAlDurum", OdemeAlDurum);
                    AlinanOdemeData.append("OdemeAlacakId", $("#OdemeTarihi").data("alacakid"));
                    AlinanOdemeData.append("OdemeTarihi", $("#OdemeTarihi").val());
                    AlinanOdemeData.append("OdemeTutar", $("#OdemeTutar").val());
                    AlinanOdemeData.append("OdemeSekli", $("#OdemeSekli").find("option:selected").text());
                    AlinanOdemeData.append("OdemeAciklama", $("#OdemeAciklama").val());
                    $.ajax({
                        type: "POST",
                        datatype: 'json',
                        data: AlinanOdemeData,
                        contentType: false,
                        processData: false,
                        url: "/Otomasyon/RezervasyonIslemleri/AlinanOdemeEkle/",
                        success: function (data) {
                            //odemetoplam = 0;
                            var odemetutar = 0;
                            $("#alinanodemeler-satir").empty();
                            $.each(data, function (index, item) {
                                $.each(item, function (index_1, item_1) {
                                    odemetutar = item_1.Tutar.replace(" TL", "");
                                    odemetutar = odemetutar.replace(".", "");
                                    odemetutar = odemetutar.replace(",", ".");
                                    if (item_1.Iptal == true) { // Ödeme İptal edilmiş ise
                                        $("#alinanodemeler-satir").append("<tr class='bg-danger-50'>" +
                                                                       "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                        "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                        "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                        "<td class='text-center'>" +
                                                                            //"<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Makbuz Yazdır'>" +
                                                                            //    "<i class='fal fa-print'></i>" +
                                                                            //"</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                                "<i class='fal fa-mobile-alt'></i>" +
                                                                            "</a>" +
                                                                            //"<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                            //   "<i class='fal fa-times-circle'></i>" +
                                                                            //"</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal='" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                               "<i class='fal fa-trash-alt'></i>" +
                                                                            "</a>" +
                                                                        "</td>" +
                                                                  "</tr>");
                                    }
                                    else {
                                        odemetoplam = odemetoplam + parseFloat(odemetutar);
                                        $("#alinanodemeler-satir").append("<tr>" +
                                                                       "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                        "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                        "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                        "<td class='text-center'>" +
                                                                            "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Makbuz Yazdır'>" +
                                                                                "<i class='fal fa-print'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                                "<i class='fal fa-mobile-alt'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                               "<i class='fal fa-times-circle'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal='" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                               "<i class='fal fa-trash-alt'></i>" +
                                                                            "</a>" +
                                                                        "</td>" +
                                                                  "</tr>");
                                    }
                                });
                            });
                            var kalanbakiye = parseFloat(SozlesmeTutar) - parseFloat(odemetoplam);

                            $("#Odemetoplam").html(accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));
                            $("#sozlesmekalan").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));

                            // Kalan ödeme tarafında
                            if (OdemeAlDurum == false) {
                                if (parseFloat(alacaktoplam) == 0) { // alacak girilmişse alacaktoplam değişkeni sıfır olacak
                                    $("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s")); // kalan alacak alanına yapılan ödemeler sonucunda kalan bakiye yazılacak.
                                }
                                else // eger bir alacak kaydı girildiyse alacaktoplam değeri yapılan ödemeler sonucunda kalan bakiyeden alacak toplam değeri çıkılarak kalan bakiye yazılacak.
                                {
                                    var alacakkalan = parseFloat(kalanbakiye) - parseFloat(alacaktoplam);
                                    if (alacakkalan >= 0) {
                                        $("#kalanalacak").html(accounting.formatMoney(alacakkalan, "TL", 2, ".", ",", "%v %s"));
                                    }
                                }
                            }
                            else {
                                $("#" + OdemeAlId).parents('tr').addClass("bg-primary-50"); // Borç tarafında ödemesi alınan satırın rengini değiştiriyor.
                                $("#" + OdemeAlId).parents('tr').find('a').addClass("disabled");
                                OdemeAlDurum == false; // Ödeme durumu sıfırlanıyor.
                            }
                            $("#OdemeTarihi").val(dateFormat(new Date()));
                            $("#OdemeTutar").val("");
                            $("#OdemeAciklama").val("");
                            $("#OdemeSekli").val("1");


                        },
                        error: function (err) {
                            alert("HATA");
                        }
                    });
                }
            }
        }
        else {  // Eğer Borçtan ödeme alınmıyor doğrudan girilen değerden ödeme alınıyorsa kalan bakiye kontrolü yapılacak
            if (kalanbakiye == 0) {
                Swal.fire(
                            {
                                title: "Borç Bitti",
                                //text: "<< " + baslangic + " - " + bitis + " => " + aciklama + " >> Tatil Günü zaten var!",
                                html: "<p style='color:red'> Sözleşme tutarının tamamı ödendi!</p><p style='color:red'> Başka Ödeme Kalmadı!</p>",
                                type: "warning",
                                //showCancelButton: true,
                                confirmButtonText: "Tamam"
                            });
            }
            else {
                if ($("#OdemeTutar").val() == "0" || $("#OdemeTutar").val() == "") {
                    $("#OdemeTutar").focus();
                    $("#OdemeTutarHata").append("<div class='hata' style='color:red'>Tutar 0(sıfır) olamaz.</div>");
                }
                else {
                    console.log("OdemeTutar=" + $("#OdemeTutar").val());
                    alert($("#OdemeTutar").val());
                    var AlinanOdemeData = new FormData();
                    AlinanOdemeData.append("SozlesmeId", sozlesmeId);
                    AlinanOdemeData.append("SozlesmeNo", sozlesmeNo);
                    AlinanOdemeData.append("OdemeAdSoyad", $("#AdSoyad").val());
                    AlinanOdemeData.append("MusteriId", musteriid);
                    AlinanOdemeData.append("OdemeAlDurum", OdemeAlDurum);
                    AlinanOdemeData.append("OdemeAlacakId", $("#OdemeTarihi").data("alacakid"));
                    AlinanOdemeData.append("OdemeTarihi", $("#OdemeTarihi").val());
                    AlinanOdemeData.append("OdemeTutar", $("#OdemeTutar").val());
                    AlinanOdemeData.append("OdemeSekli", $("#OdemeSekli").find("option:selected").text());
                    AlinanOdemeData.append("OdemeAciklama", $("#OdemeAciklama").val());
                    $.ajax({
                        type: "POST",
                        datatype: 'json',
                        data: AlinanOdemeData,
                        contentType: false,
                        processData: false,
                        url: "/Otomasyon/RezervasyonIslemleri/AlinanOdemeEkle/",
                        success: function (data) {
                            //odemetoplam = 0;
                            var odemetutar = 0;
                            $("#alinanodemeler-satir").empty();
                            $.each(data, function (index, item) {
                                $.each(item, function (index_1, item_1) {
                                    odemetutar = item_1.Tutar.replace(" TL", "");
                                    odemetutar = odemetutar.replace(".", "");
                                    odemetutar = odemetutar.replace(",", ".");
                                    if (item_1.Iptal == true) { // Ödeme İptal edilmiş ise
                                        $("#alinanodemeler-satir").append("<tr class='bg-danger-50'>" +
                                                                       "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                        "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                        "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                        "<td class='text-center'>" +
                                                                            //"<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Makbuz Yazdır'>" +
                                                                            //    "<i class='fal fa-print'></i>" +
                                                                            //"</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                                "<i class='fal fa-mobile-alt'></i>" +
                                                                            "</a>" +
                                                                            //"<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                            //   "<i class='fal fa-times-circle'></i>" +
                                                                            //"</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal='" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                               "<i class='fal fa-trash-alt'></i>" +
                                                                            "</a>" +
                                                                        "</td>" +
                                                                  "</tr>");
                                    }
                                    else {
                                        odemetoplam = odemetoplam + parseFloat(odemetutar);
                                        $("#alinanodemeler-satir").append("<tr>" +
                                                                       "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                        "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                        "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                        "<td class='text-center'>" +
                                                                            "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Makbuz Yazdır'>" +
                                                                                "<i class='fal fa-print'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                                "<i class='fal fa-mobile-alt'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                               "<i class='fal fa-times-circle'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal='" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                               "<i class='fal fa-trash-alt'></i>" +
                                                                            "</a>" +
                                                                        "</td>" +
                                                                  "</tr>");
                                    }
                                });
                            });
                            var kalanbakiye = parseFloat(SozlesmeTutar) - parseFloat(odemetoplam);

                            $("#Odemetoplam").html(accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));
                            $("#sozlesmekalan").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));

                            // Kalan ödeme tarafında
                            if (OdemeAlDurum == false) {
                                if (parseFloat(alacaktoplam) == 0) { // alacak girilmişse alacaktoplam değişkeni sıfır olacak
                                    $("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s")); // kalan alacak alanına yapılan ödemeler sonucunda kalan bakiye yazılacak.
                                }
                                else // eger bir alacak kaydı girildiyse alacaktoplam değeri yapılan ödemeler sonucunda kalan bakiyeden alacak toplam değeri çıkılarak kalan bakiye yazılacak.
                                {
                                    var alacakkalan = parseFloat(kalanbakiye) - parseFloat(alacaktoplam);
                                    if (alacakkalan >= 0) {
                                        $("#kalanalacak").html(accounting.formatMoney(alacakkalan, "TL", 2, ".", ",", "%v %s"));
                                    }
                                }
                            }
                            else {
                                $("#" + OdemeAlId).parents('tr').addClass("bg-primary-50"); // Borç tarafında ödemesi alınan satırın rengini değiştiriyor.
                                $("#" + OdemeAlId).parents('tr').find('a').addClass("disabled");
                                OdemeAlDurum == false; // Ödeme durumu sıfırlanıyor.
                            }
                            $("#OdemeTarihi").val(dateFormat(new Date()));
                            $("#OdemeTutar").val("");
                            $("#OdemeAciklama").val("");
                            $("#OdemeSekli").val("1");


                        },
                        error: function (err) {
                            alert("HATA");
                        }
                    });
                }
            }
        }
        
    });
    $("#tb-alinanodemeler").on('click', '.odemeiptal', function () {
        var id = $(this).data("odemeid");
        var odemelergelecekid = $(this).data("odemelergelecekid");
        var tarih = $(this).data("odemetarih");
        var tutar = $(this).data("odemetutar");
        Swal.fire(
        {
            title: "Ödeme İptal",
            //text: "< " + giskategori + " > adlı Günlük İş Katergorisi silinsin mi?",
            html: "<p style='color:red;font-weight:bold;font-size:12pt;'>&laquo; " + tarih + " - " + tutar + " &raquo;</p> <p style='color:red;font-size:12pt;'><h3> Ödeme iptal edilsin mi?</h3></p> ",
            type: "error",
            showCancelButton: true,
            confirmButtonText: "Evet, İptal Et!"
        }).then(function (result) {
            if (result.value) {
                var AlinanOdemeData = new FormData();
                AlinanOdemeData.append("Id", id);
                AlinanOdemeData.append("OdemelerGelecekId", odemelergelecekid);
                $.ajax({
                    type: "POST",
                    datatype: 'json',
                    data: AlinanOdemeData,
                    contentType: false,
                    processData: false,
                    url: "/Otomasyon/RezervasyonIslemleri/AlinanOdemeIptal/" + id,
                    success: function (data) {
                        var odemetoplam = $("#Odemetoplam").html(); // yapılmış ödemeler toplamının yazıldığı alandan bu değeri okuyorum.
                        odemetoplam = odemetoplam.replace(" TL", "");
                        odemetoplam = odemetoplam.replace(".", "");
                        odemetoplam = odemetoplam.replace(",", ".");
                        var iptaltutar = 0; // iptal edilen tutar
                        iptaltutar = tutar.replace(" TL", "");
                        iptaltutar = iptaltutar.replace(".", "");
                        iptaltutar = iptaltutar.replace(",", ".");
                        $("#alinanodemeler-satir").empty();
                        odemetoplam = odemetoplam - parseFloat(iptaltutar); // toplam ödeme tutarından iptal edilen tutardüşülüyor.
                        $.each(data, function (index, item) {
                            $.each(item, function (index_1, item_1) {
                                if (item_1.Iptal == true) {
                                    $("#alinanodemeler-satir").append("<tr class='bg-danger-50'>" +
                                                                   "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                    "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                    "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        //"<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Makbuz Yazdır'>" +
                                                                        //    "<i class='fal fa-print'></i>" +
                                                                        //"</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        //"<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                        //   "<i class='fal fa-times-circle'></i>" +
                                                                        //"</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal'" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                                }
                                else {
                                    $("#alinanodemeler-satir").append("<tr>" +
                                                                   "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                    "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                    "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "'title='Makbuz Yazdır'>" +
                                                                            "<i class='fal fa-print'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                           "<i class='fal fa-times-circle'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal'" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                                }

                            });
                        });
                        var kalanbakiye = parseFloat(ToplamTutar) - parseFloat(odemetoplam); // İptal sonucu kalan bakiye gündelleniyor.
                        $("#Odemetoplam").html(accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));
                        $("#sozlesmekalan").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));


                        // Kalan ödeme tarafında
                        if (parseFloat(alacaktoplam) == 0) { // alacak girilmişse alacaktoplam değişkeni sıfır olacak
                            $("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s")); // kalan alacak alanına yapılan ödemeler sonucunda kalan bakiye yazılacak.
                        }
                        else // eger bir alacak kaydı girildiyse alacaktoplam değeri yapılan ödemeler sonucunda kalan bakiyeden alacak toplam değeri çıkılarak kalan bakiye yazılacak.
                        {
                            var alacakkalan = parseFloat(kalanbakiye) - parseFloat(alacaktoplam);
                            if (alacakkalan >= 0) {
                                $("#kalanalacak").html(accounting.formatMoney(alacakkalan, "TL", 2, ".", ",", "%v %s"));
                            }
                        }
                        if (odemelergelecekid != 0 || odemelergelecekid != null) {
                            $("#" + odemelergelecekid).parents('tr').removeClass("bg-primary-50"); //Alınan ödeme iptal edildiğinde Borç tarafında ödemesi alınan satır eski haline getiriliyor.
                            $("#" + odemelergelecekid).parents('tr').find('a').removeClass("disabled");
                        }

                        //if (parseFloat(alacaktoplam) == 0) { // alacak girilmişse alacaktoplam değişkeni sıfır olacak
                        //    $("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s")); // kalan alacak alanına yapılan ödemeler sonucunda kalan bakiye yazılacak.
                        //}
                        //else // eger bir alacak kaydı girildiyse alacaktoplam değeri yapılan ödemeler sonucunda kalan bakiyeden alacak toplam değeri çıkılarak kalan bakiye yazılacak.
                        //{
                        //    var alacakkalan = parseFloat(kalanbakiye) - parseFloat(alacaktoplam);
                        //    $("#kalanalacak").html(accounting.formatMoney(alacakkalan, "TL", 2, ".", ",", "%v %s"));
                        //}

                        //$("#Odemetoplam").data("odemetoplam",accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));
                        //$("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));
                    },
                    error: function (err) {
                        alert("HATA");
                    }
                });
            }
        });
    }); // Alınan Ödeme İptal
    $("#tb-alinanodemeler").on('click', '.odemesil', function () {
        var id = $(this).data("odemeid");
        var tarih = $(this).data("odemetarih");
        var tutar = $(this).data("odemetutar");
        var iptal = $(this).data("iptal");
        Swal.fire(
        {
            title: "Ödeme Sil",
            //text: "< " + giskategori + " > adlı Günlük İş Katergorisi silinsin mi?",
            html: "<p style='color:red;font-weight:bold;font-size:12pt;'>&laquo; " + tarih + " - " + tutar + " &raquo;</p> <p style='color:red;font-size:12pt;'><h3> Ödeme silinsin mi?</h3></p> ",
            type: "error",
            showCancelButton: true,
            confirmButtonText: "Sil!"
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    datatype: 'json',
                    contentType: false,
                    processData: false,
                    url: "/Otomasyon/RezervasyonIslemleri/AlinanOdemeSil/" + id,
                    success: function (data) {
                        var odemetoplam = 0;
                        var odemetutar = 0;
                        $("#alinanodemeler-satir").empty();
                        $.each(data, function (index, item) {
                            $.each(item, function (index_1, item_1) {
                                odemetutar = item_1.Tutar.replace(" TL", "");
                                odemetutar = odemetutar.replace(".", "");
                                odemetutar = odemetutar.replace(",", ".");
                                if (item_1.Iptal == true) {
                                    $("#alinanodemeler-satir").append("<tr class='bg-danger-50'>" +
                                                                   "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                    "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                    "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        //"<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Makbuz Yazdır'>" +
                                                                        //    "<i class='fal fa-print'></i>" +
                                                                        //"</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        //"<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                        //   "<i class='fal fa-times-circle'></i>" +
                                                                        //"</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal'" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                                }
                                else {
                                    odemetoplam = odemetoplam + parseFloat(odemetutar);
                                    $("#alinanodemeler-satir").append("<tr>" +
                                                                   "<td class='text-center'>" + item_1.Tarih + "</td>" +
                                                                    "<td class='text-right'>" + item_1.Tutar + "</td>" +
                                                                    "<td class='text-center'>" + item_1.Aciklama + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 makbuzyazdir' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Makbuz Yazdır'>" +
                                                                            "<i class='fal fa-print'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-success btn-icon mr-1 odemesmsgonder' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='SMS Gönder'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 odemeiptal' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' title='Ödemeyi İptal Et'>" +
                                                                           "<i class='fal fa-times-circle'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 odemesil' data-odemelergelecekid='" + item_1.OdemelerGelecekId + "' data-odemeid='" + item_1.Id + "' data-odemetarih='" + item_1.Tarih + "' data-odemetutar='" + item_1.Tutar + "' data-iptal'" + item_1.Iptal + "' title='Ödemeyi Sil'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                                }

                            });
                        });
                        if (iptal == true) { // Silinmek istenen iptal edilmiş satır ise bakiyelerde güncelleme olmayacak

                        }
                        else {
                            var kalanbakiye = parseFloat(ToplamTutar) - parseFloat(odemetoplam);
                            $("#Odemetoplam").html(accounting.formatMoney(odemetoplam, "TL", 2, ".", ",", "%v %s"));
                            $("#sozlesmekalan").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));

                            // Kalan ödeme tarafında
                            if (parseFloat(alacaktoplam) == 0) { // alacak girilmişse alacaktoplam değişkeni sıfır olacak
                                $("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s")); // kalan alacak alanına yapılan ödemeler sonucunda kalan bakiye yazılacak.
                            }
                            else // eger bir alacak kaydı girildiyse alacaktoplam değeri yapılan ödemeler sonucunda kalan bakiyeden alacak toplam değeri çıkılarak kalan bakiye yazılacak.
                            {
                                var alacakkalan = parseFloat(kalanbakiye) - parseFloat(alacaktoplam);
                                if (alacakkalan >= 0) {
                                    $("#kalanalacak").html(accounting.formatMoney(alacakkalan, "TL", 2, ".", ",", "%v %s"));
                                }
                            }
                            if (odemelergelecekid != 0 || odemelergelecekid != null) {
                                $("#" + odemelergelecekid).parents('tr').removeClass("bg-primary-50"); //Alınan ödeme silindiğinde Borç tarafında ödemesi alınan satır eski haline getiriliyor.
                                $("#" + odemelergelecekid).parents('tr').find('a').removeClass("disabled");
                            }


                            //// Kalan ödeme tarafında
                            //if (parseFloat(alacaktoplam) == 0) { // alacak girilmişse alacaktoplam değişkeni sıfır olacak
                            //    $("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s")); // kalan alacak alanına yapılan ödemeler sonucunda kalan bakiye yazılacak.
                            //}
                            //else // eger bir alacak kaydı girildiyse alacaktoplam değeri yapılan ödemeler sonucunda kalan bakiyeden alacak toplam değeri çıkılarak kalan bakiye yazılacak.
                            //{
                            //    var alacakkalan = parseFloat(kalanbakiye) - parseFloat(alacaktoplam);
                            //    $("#kalanalacak").html(accounting.formatMoney(alacakkalan, "TL", 2, ".", ",", "%v %s"));
                            //}
                            ////$("#kalanalacak").html(accounting.formatMoney(kalanbakiye, "TL", 2, ".", ",", "%v %s"));
                            ////$("#kalanalacak").html(accounting.formatMoney(alacakkalan2, "TL", 2, ".", ",", "%v %s"));
                        }
                    },
                    error: function (err) {
                        alert("HATA");
                    }
                });
            }
        });
    }); // Alınan Ödeme Sil
    $("#tb-alinanodemeler").on('click', '.makbuzyazdir', function () {
        var id = $(this).data("odemeid");
        window.open("/Otomasyon/RezervasyonIslemleri/AlinanOdemeMakbuzYazdir/" + id, '_blank');
    }); // Ödeme Makbuzu Yazdır
    // ------ Alınan ve Gelecek Ödeme Ekleme/Silme İşlemleri
    // ------ Çekim Randevusu Ekleme/Silme İşlemleri
    $("#CekimEkle").click(function () {
        if (($("#CekimBaslangic").val() == $("#CekimBitis").val())) {
            $("#CekimBaslangic").focus();
            $("#CekimZamanHata").append("<div class='hata' style='color:red'>Başlangıç ve Bitiş zamanları aynı olamaz.</div>");
        }
        else if (($("#CekimYeri").val() == "")) {
            $("#CekimYeri").focus();
            $("#CekimYeriHata").append("<div class='hata' style='color:red'>Çekim yapılacak yeri yazınız.</div>");
        }
        else if (($("#CekimPersonelId").val() == "0")) {
            $("#CekimPersonelId").focus();
            $("#CekimPersonelIdHata").append("<div id='cekimpersonelihata' style='color:red'>Çekim yacak personeli seçiniz.</div>");
        }
        else {
            var CekimData = new FormData();
            CekimData.append("SozlesmeId", sozlesmeId);
            CekimData.append("SozlesmeNo", sozlesmeNo);
            CekimData.append("MusteriId", musteriid);
            CekimData.append("PersonelId", $("#CekimPersonelId").val());
            CekimData.append("CekimTarihi", $("#CekimTarihi").val());
            CekimData.append("CekimBaslangic", $("#CekimBaslangic").val());
            CekimData.append("CekimBitis", $("#CekimBitis").val());
            CekimData.append("CekimYeri", $("#CekimYeri").val());
            $.ajax({
                type: "POST",
                datatype: 'json',
                data: CekimData,
                contentType: false,
                processData: false,
                url: "/Otomasyon/RezervasyonIslemleri/CekimRandevuEkle/",
                success: function (data) {
                    $("#cekimrandevu-satir").empty();
                    $.each(data, function (index, item) {
                        $.each(item, function (index_1, item_1) {
                            if (item_1.Iptal == true) {
                                $("#cekimrandevu-satir").append("<tr class='bg-danger-50'>" +
                                                               "<td>" + item_1.Tarih + "</td>" +
                                                                "<td>" + item_1.Baslangic + " - " + item_1.Bitis + "</td>" +
                                                                "<td>" + item_1.Aciklama + "</td>" +
                                                                "<td>" + item_1.Personel + "</td>" +
                                                                "<td class='text-center'>" +
                                                                    "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 cekimranduevusms' title='Bilgilendirme SMS'i Gönder' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                        "<i class='fal fa-mobile-alt'></i>" +
                                                                    "</a>" +
                                                                    //"<a href='#' class='btn btn-xs btn-info btn-icon mr-1 disabled cekimranduevuiptal' title='Randevuyu İptal Et' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                    //   "<i class='fal fa-times-circle'></i>" +
                                                                    //"</a>" +
                                                                    "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 cekimranduevusil' title='Randevuyu Sil' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                       "<i class='fal fa-trash-alt'></i>" +
                                                                    "</a>" +
                                                                "</td>" +
                                                          "</tr>");
                            }
                            else {
                                $("#cekimrandevu-satir").append("<tr>" +
                                                                   "<td>" + item_1.Tarih + "</td>" +
                                                                    "<td>" + item_1.Baslangic + " - " + item_1.Bitis + "</td>" +
                                                                    "<td>" + item_1.Aciklama + "</td>" +
                                                                    "<td>" + item_1.Personel + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 cekimranduevusms' title='Bilgilendirme SMS'i Gönder' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 cekimranduevuiptal' title='Randevuyu İptal Et' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                           "<i class='fal fa-times-circle'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 cekimranduevusil' title='Randevuyu Sil' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                            }
                        });
                    });
                    $("#CekimTarihi").datepicker("setDate", new Date());
                    $("#CekimBaslangic").val("00:00");
                    $("#CekimBitis").val("00:00");
                    $("#CekimYeri").val("");
                    $("#CekimPersonelId").val("0");
                },
                error: function (err) {
                    alert("HATA");
                }
            });
        }
    });
    $("#tb-cekimrandeulari").on('click', '.cekimranduevuiptal', function () {
        var id = $(this).data("cekimid");
        var tarih = $(this).data("cekimtarih");
        var baslangic = $(this).data("cekimbaslama");
        var bitis = $(this).data("cekimbitis");
        Swal.fire(
        {
            title: "Randevu İptal",
            //text: "< " + giskategori + " > adlı Günlük İş Katergorisi silinsin mi?",
            html: "<p style='color:red;font-weight:bold;font-size:12pt;'>&laquo; " + tarih + " - " + baslangic + "-" + bitis + " &raquo;</p> <p style='color:red;font-size:12pt;'><h3> Çekim Randevusunu iptal edilsin mi?</h3></p> ",
            type: "error",
            showCancelButton: true,
            confirmButtonText: "Evet, İptal Et!"
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    datatype: 'json',
                    contentType: false,
                    processData: false,
                    url: "/Otomasyon/RezervasyonIslemleri/CekimRandevuIptal/" + id,
                    success: function (data) {
                        $("#cekimrandevu-satir").empty();
                        $.each(data, function (index, item) {
                            $.each(item, function (index_1, item_1) {
                                if (item_1.Iptal == true) {
                                    $("#cekimrandevu-satir").append("<tr class='bg-danger-50'>" +
                                                                   "<td>" + item_1.Tarih + "</td>" +
                                                                    "<td>" + item_1.Baslangic + " - " + item_1.Bitis + "</td>" +
                                                                    "<td>" + item_1.Aciklama + "</td>" +
                                                                    "<td>" + item_1.Personel + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 cekimranduevusms' title='Bilgilendirme SMS'i Gönder' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 disabled cekimranduevuiptal' title='Randevuyu İptal Et' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                           "<i class='fal fa-times-circle'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 cekimranduevusil' title='Randevuyu Sil' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                                }
                                else {
                                    $("#cekimrandevu-satir").append("<tr>" +
                                                                       "<td>" + item_1.Tarih + "</td>" +
                                                                        "<td>" + item_1.Baslangic + " - " + item_1.Bitis + "</td>" +
                                                                        "<td>" + item_1.Aciklama + "</td>" +
                                                                        "<td>" + item_1.Personel + "</td>" +
                                                                        "<td class='text-center'>" +
                                                                            "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 cekimranduevusms' title='Bilgilendirme SMS'i Gönder' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                                "<i class='fal fa-mobile-alt'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 cekimranduevuiptal' title='Randevuyu İptal Et' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                               "<i class='fal fa-times-circle'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 cekimranduevusil' title='Randevuyu Sil' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                               "<i class='fal fa-trash-alt'></i>" +
                                                                            "</a>" +
                                                                        "</td>" +
                                                                  "</tr>");
                                }
                            });
                        });
                    },
                    error: function (err) {
                        alert("HATA");
                    }
                });
            }
        });
    }); // Çekim Randevusu İptal
    $("#tb-cekimrandeulari").on('click', '.cekimranduevusil', function () {
        var id = $(this).data("cekimid");
        var tarih = $(this).data("cekimtarih");
        var baslangic = $(this).data("cekimbaslama");
        var bitis = $(this).data("cekimbitis");
        Swal.fire(
        {
            title: "Randevu İptal",
            //text: "< " + giskategori + " > adlı Günlük İş Katergorisi silinsin mi?",
            html: "<p style='color:red;font-weight:bold;font-size:12pt;'>&laquo; " + tarih + " - " + baslangic + "-" + bitis + " &raquo;</p> <p style='color:red;font-size:12pt;'><h3> Çekim Randevusunu Silinsin mi?</h3></p> ",
            type: "warning",
            showCancelButton: true,
            confirmButtonText: "Sil!"
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    datatype: 'json',
                    contentType: false,
                    processData: false,
                    url: "/Otomasyon/RezervasyonIslemleri/CekimRandevuSil/" + id,
                    success: function (data) {
                        $("#cekimrandevu-satir").empty();
                        $.each(data, function (index, item) {
                            $.each(item, function (index_1, item_1) {
                                if (item_1.Iptal == true) {
                                    $("#cekimrandevu-satir").append("<tr class='bg-danger-50'>" +
                                                                   "<td>" + item_1.Tarih + "</td>" +
                                                                    "<td>" + item_1.Baslangic + " - " + item_1.Bitis + "</td>" +
                                                                    "<td>" + item_1.Aciklama + "</td>" +
                                                                    "<td>" + item_1.Personel + "</td>" +
                                                                    "<td class='text-center'>" +
                                                                        "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 cekimranduevusms' title='Bilgilendirme SMS'i Gönder' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                            "<i class='fal fa-mobile-alt'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 disabled cekimranduevuiptal' title='Randevuyu İptal Et' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                           "<i class='fal fa-times-circle'></i>" +
                                                                        "</a>" +
                                                                        "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 cekimranduevusil' title='Randevuyu Sil' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                           "<i class='fal fa-trash-alt'></i>" +
                                                                        "</a>" +
                                                                    "</td>" +
                                                              "</tr>");
                                }
                                else {
                                    $("#cekimrandevu-satir").append("<tr>" +
                                                                       "<td>" + item_1.Tarih + "</td>" +
                                                                        "<td>" + item_1.Baslangic + " - " + item_1.Bitis + "</td>" +
                                                                        "<td>" + item_1.Aciklama + "</td>" +
                                                                        "<td>" + item_1.Personel + "</td>" +
                                                                        "<td class='text-center'>" +
                                                                            "<a href='#' class='btn btn-xs btn-primary btn-icon mr-1 cekimranduevusms' title='Bilgilendirme SMS'i Gönder' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                                "<i class='fal fa-mobile-alt'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-info btn-icon mr-1 cekimranduevuiptal' title='Randevuyu İptal Et' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                               "<i class='fal fa-times-circle'></i>" +
                                                                            "</a>" +
                                                                            "<a href='#' class='btn btn-xs btn-danger btn-icon mr-1 cekimranduevusil' title='Randevuyu Sil' data-cekimid='" + item_1.Id + "' data-cekimtarih='" + item_1.Tarih + "' data-cekimbaslama='" + item_1.Baslangic + "' data-cekimbitis='" + item_1.Bitis + "'>" +
                                                                               "<i class='fal fa-trash-alt'></i>" +
                                                                            "</a>" +
                                                                        "</td>" +
                                                                  "</tr>");
                                }
                            });
                        });

                    },
                    error: function (err) {
                        alert("HATA");
                    }
                });
            }
        });
    }); // Çekim Randevusu Sil
    // ------ Çekim Randevusu Ekleme/Silme İşlemleri
    $('input').on('focusout', function (e) {
        if ($(this).val != "") {
            $(this).next("div.hata").remove();
        }
    }); // Validation hata kaldırma

    $('#PersonelId').on('focusout', function (e) {
        if ($(this).val != "0") {
            $("#PersonelHata").remove();
        }
    }); // zorunlu alan kontrolü
    $('#RezervasyonTurId').on('focusout', function (e) {
        if ($(this).val != "0") {
            $("#RezervasyonTurHata").remove();
        }
    }); // zorunlu alan kontrolü
    $('#OpsiyonTarihi').on('focusout', function (e) {
        if ($(this).val != "0") {
            $("#opsiyonhata").remove();
        }
    }); // zorunlu alan kontrolü
    $('#ZamanDilimiId').on('focusout', function (e) {
        if ($(this).val != "0") {
            $("#zamandilimihata").remove();
        }
    }); // zorunlu alan kontrolü
    $('#CekimPersonelId').on('focusout', function (e) {
        if ($(this).val != "0") {
            $("#cekimpersonelihata").remove();
        }
    }); // zorunlu alan kontrolü
    $("#RezervasyonBilgileriDevam").click(function () {
        // Zorunlu alanların kontrolü yapılıyor.
        var opsiyontarih = $("#OpsiyonTarihi").val().split('.');
        var rezervasyontarih = $("#RezervasyonTarihi").val().split('.');
        opsiyontarih = opsiyontarih[2] + opsiyontarih[1] + opsiyontarih[0];
        rezervasyontarih = rezervasyontarih[2] + rezervasyontarih[1] + rezervasyontarih[0];
        //if ($("#YetkiliId").val() == "0") {
        //    $("#YetkiliId").focus();
        //    $("#YetkiliIdHata").append("<div id='YetkiliHata' style='color:red'>Yetkili Seçiniz.</div>");
        //}
        if ($("#PersonelId").val() == "0") {
            $("#PersonelId").focus();
            $("#PersonelIdHata").append("<div id='PersonelHata' style='color:red'>Personel Seçiniz.</div>");
        }
        else if ($("#RezervasyonTurId").val() == "0") {
            $("#RezervasyonTurId").focus();
            $("#RezervasyonTurIdHata").append("<div id='RezervasyonTurHata' style='color:red'>Rezervasyon Türünü Seçiniz.</div>");
        }
        else if (($("#RezervasyonSekliId").val() != "1") && (parseInt(opsiyontarih) >= parseInt(rezervasyontarih))) {
            $("#OpsiyonTarihi").focus();
            $("#OpsiyonTarihiHata").append("<div id='opsiyonhata' style='color:red'>Opsiyon Tarihi Rezervasyon Tarihinden geri bir tarih olmalıdır.</div>");
        }
        else if (($("#ZamanDilimiId").val() == "0") && ($("#BaslangicZaman").val() == $("#BitisZaman").val())) {
            $("#BaslangicZaman").focus();
            $("#ZamanHata").append("<div id='zamandilimihata' style='color:red'>Başlangıç ve Bitiş zamanları aynı olamaz.</div>");
        }
        else {
            // Rezervasyon Bilgileri Sozlesme tablosuna kaydediliyor. Daha sonraki sayfalar bu kaydın güncellenmesi şeklinde devam edecek.
            var SozlesmeData = new FormData();
            SozlesmeData.append("SozlesmeTarihi", $("#SozlesmeTarihi").val());
            SozlesmeData.append("SozlesmeNo", $("#SozlesmeNo").val());
            SozlesmeData.append("Yetkili", $("#Yetkili").val());
            SozlesmeData.append("PersonelId", $("#PersonelId").val());
            SozlesmeData.append("RezervasyonTurId", $("#RezervasyonTurId").val());
            SozlesmeData.append("OrganizasyonYeri", $("#OrganizasyonYeri").val());
            SozlesmeData.append("RezervasyonSekliId", $("#RezervasyonSekliId").val());
            SozlesmeData.append("OpsiyonTarihi", $("#OpsiyonTarihi").val());
            SozlesmeData.append("RezervasyonTarihi", $("#RezervasyonTarihi").val());
            SozlesmeData.append("ZamanDilimiId", $("#ZamanDilimiId").val());
            SozlesmeData.append("BaslangicZaman", $("#BaslangicZaman").val());
            SozlesmeData.append("BitisZaman", $("#BitisZaman").val());

            $.ajax({
                type: "POST",
                datatype: 'json',
                data: SozlesmeData,
                contentType: false,
                processData: false,
                url: "/Otomasyon/RezervasyonIslemleri/SozlesmeGuncelle/" + sozlesmeId,
                success: function (data) {
                    //sozlesmeId = data.sozlesmeId;
                    //randevuId = data.randevuId;
                    //sozlesmeNo = data.sozlesmeNo;
                    //MusteriBilgileriAktif();
                    // Kayıt Başarılı mesajı
                    if (data.Sonuc==true) {
                        Command: toastr["success"]("Değişiklikler Başarıyla Kaydedildi")

                        toastr.options = {
                            "closeButton": false,
                            "debug": false,
                            "newestOnTop": false,
                            "progressBar": true,
                            "positionClass": "toast-top-center",
                            "preventDuplicates": true,
                            "onclick": null,
                            "showDuration": 300,
                            "hideDuration": 100,
                            "timeOut": 1000,
                            "extendedTimeOut": 1000,
                            "showEasing": "swing",
                            "hideEasing": "linear",
                            "showMethod": "fadeIn",
                            "hideMethod": "fadeOut"
                        }
                    }
                    else
                    {
                        Command: toastr["error"]("Kayıtlar Değiştirilemedi ", "HATA")

                        toastr.options = {
                            "closeButton": false,
                            "debug": false,
                            "newestOnTop": false,
                            "progressBar": true,
                            "positionClass": "toast-top-center",
                            "preventDuplicates": true,
                            "onclick": null,
                            "showDuration": 300,
                            "hideDuration": 100,
                            "timeOut": 2000,
                            "extendedTimeOut": 1000,
                            "showEasing": "swing",
                            "hideEasing": "linear",
                            "showMethod": "fadeIn",
                            "hideMethod": "fadeOut"
                        }
                    }
                   
                },
                error: function (err) {
                    alert("HATA");
                }
            });
        }
    });
    $("#MusteriBilgileriDevam").click(function () {
        if (musteriid == 0) { // Müşteri seçilmemiş ise zorunlu alanlar kontrol edilecek sonra müşteri kaydı yapılacak.
            if ($("#AdSoyad").val() == "") // Kişi seçilmişse zorunlu alan kontrolü
            {
                $("#AdSoyad").focus();
                $("#AdSoyadHata").append("<div class='hata' style='color:red'>Müşteri Adını Soyadını Giriniz.</div>");
            }
            else if ($("#CepTel").val() == "") // Kişi seçilmişse zorunlu alan kontrolü
            {
                $("#CepTel").focus();
                $("#CepTelHata").append("<div class='hata' style='color:red'>Müşterinin Cep Telefonu Numarasını Giriniz.</div>");
            }
            else {
                var Musteri = new FormData();
                Musteri.append("MusteriKodu", $("#MusteriKodu").val());
                Musteri.append("TCKimlikNo", $("#TCKimlikNo").val());
                Musteri.append("AdSoyad", $("#AdSoyad").val());
                Musteri.append("CepTel", $("#CepTel").val());
                Musteri.append("SabitTel", $("#SabitTel").val());
                Musteri.append("Email", $("#Email").val());
                Musteri.append("Adres", $("#Adres").val());
                Musteri.append("Notlar", $("#Notlar").val());
                $.ajax({
                    type: "POST",
                    datatype: 'json',
                    data: Musteri,
                    contentType: false,
                    processData: false,
                    url: "/Otomasyon/Musteriler/MusteriEkle/",
                    success: function (data) {
                        musteriid = data.MusteriId;
                        var Sozlesme = new FormData();
                        Sozlesme.append("SozlesmeId", sozlesmeId);
                        Sozlesme.append("SozlesmeNo", sozlesmeNo);
                        Sozlesme.append("MusteriId", musteriid);
                        $.ajax({
                            type: "POST",
                            datatype: 'json',
                            data: Sozlesme,
                            contentType: false,
                            processData: false,
                            url: "/Otomasyon/RezervasyonIslemleri/SozlesmeMusteriGuncelle/",
                            success: function (data) {
                                DigerBilgilerAktif();
                            },
                            error: function (err) {
                                alert("HATA");
                            }
                        });
                    },
                    error: function (err) {
                        alert("HATA");
                    }
                });
            }
        }
        else {
            // varolan müşteriler içinden seçilen müşteri bilgilerinde değişiklik varsa müşteri tablosunda güncellenmeli
            var Musteri = new FormData();
            Musteri.append("MusteriKodu", $("#MusteriKodu").val());
            Musteri.append("TCKimlikNo", $("#TCKimlikNo").val());
            Musteri.append("AdSoyad", $("#AdSoyad").val());
            Musteri.append("CepTel", $("#CepTel").val());
            Musteri.append("SabitTel", $("#SabitTel").val());
            Musteri.append("Email", $("#Email").val());
            Musteri.append("Adres", $("#Adres").val());
            Musteri.append("Notlar", $("#Notlar").val());
            $.ajax({
                type: "POST",
                datatype: 'json',
                data: Musteri,
                contentType: false,
                processData: false,
                url: "/Otomasyon/Musteriler/MusteriGuncelle/" + musteriid,
                success: function (data) {
                },
                error: function (err) {
                    alert("HATA");
                }
            });

            var Sozlesme = new FormData();
            Sozlesme.append("SozlesmeId", sozlesmeId);
            Sozlesme.append("SozlesmeNo", sozlesmeNo);
            Sozlesme.append("MusteriId", musteriid);
            $.ajax({
                type: "POST",
                datatype: 'json',
                data: Sozlesme,
                contentType: false,
                processData: false,
                url: "/Otomasyon/RezervasyonIslemleri/SozlesmeMusteriGuncelle/",
                success: function (data) {
                    DigerBilgilerAktif();
                },
                error: function (err) {
                    alert("HATA");
                }
            });
        }
    });
    $("#MusteriBilgileriGeri").click(function () {
        var SozlesmeData = new FormData();
        SozlesmeData.append("SozlesmeNo", sozlesmeNo);
        SozlesmeData.append("SozlesmeId", sozlesmeId);
        SozlesmeData.append("RandevuId", randevuId);
        $.ajax({
            type: "POST",
            datatype: 'json',
            data: SozlesmeData,
            contentType: false,
            processData: false,
            url: "/Otomasyon/RezervasyonIslemleri/SozlesmeSil/",
            success: function (data) {
                RezervasyonBilgileriAktif();
            },
            error: function (err) {
                alert("HATA");
            }
        });
    });
    $("#DigerBilgilerDevam").click(function () {
        var DegerBilgiler = new FormData();
        DegerBilgiler.append("SozlesmeId", sozlesmeId);
        DegerBilgiler.append("RandevuId", randevuId);
        DegerBilgiler.append("SozlesmeNo", sozlesmeNo);
        DegerBilgiler.append("GelinAdSoyad", $("#GelinAdSoyad").val());
        DegerBilgiler.append("GelinCep", $("#GelinCep").val());
        DegerBilgiler.append("GelinEmail", $("#GelinEmail").val());
        DegerBilgiler.append("DamatAdSoyad", $("#DamatAdSoyad").val());
        DegerBilgiler.append("DamatCep", $("#DamatCep").val());
        DegerBilgiler.append("DamatEmail", $("#DamatEmail").val());
        DegerBilgiler.append("BebekAdSoyad", $("#BebekAdSoyad").val());
        DegerBilgiler.append("BabaAdSoyad", $("#BabaAdSoyad").val());
        DegerBilgiler.append("AnneAdSoyad", $("#AnneAdSoyad").val());
        DegerBilgiler.append("BabaCep", $("#BabaCep").val());
        DegerBilgiler.append("AnneCep", $("#AnneCep").val());
        DegerBilgiler.append("BabaEmail", $("#BabaEmail").val());
        DegerBilgiler.append("Urunler", $("#Urunler").val());
        DegerBilgiler.append("UrunYetkiliAdSoyad", $("#UrunYetkiliAdSoyad").val());
        DegerBilgiler.append("UrunYetkiliCep", $("#UrunYetkiliCep").val());
        DegerBilgiler.append("UrunYetkiliEmail", $("#UrunYetkiliEmail").val());
        DegerBilgiler.append("Modeller", $("#Modeller").val());
        DegerBilgiler.append("ModelYetkiliAdSoyad", $("#ModelYetkiliAdSoyad").val());
        DegerBilgiler.append("ModelYetkiliCep", $("#ModelYetkiliCep").val());
        DegerBilgiler.append("ModelYetkiliEmail", $("#ModelYetkiliEmail").val());
        DegerBilgiler.append("YetkiliAdSoyad", $("#YetkiliAdSoyad").val());
        DegerBilgiler.append("YetkiliCep", $("#YetkiliCep").val());
        DegerBilgiler.append("YetkiliEmail", $("#YetkiliEmail").val());
        $.ajax({
            type: "POST",
            datatype: 'json',
            data: DegerBilgiler,
            contentType: false,
            processData: false,
            url: "/Otomasyon/RezervasyonIslemleri/SozlesmeDigerBilgilerGuncelle/",
            success: function (data) {

                PaketVeEkhizmetlerAktif();
            },
            error: function (err) {
                alert("HATA");
            }
        });
    });
    $("#DigerBilgilerGeri").click(function () {
        MusteriBilgileriAktif();
    });
    $("#PaketlerDevam").click(function () {
        var PaketEkHizmetData = new FormData();
        PaketEkHizmetData.append("SozlesmeId", sozlesmeId);
        PaketEkHizmetData.append("SozlesmeNo", sozlesmeNo);
        PaketEkHizmetData.append("secilenpaketID", secilenpaketID);
        PaketEkHizmetData.append("secilenekhizmetID", secilenekhizmetID);
        PaketEkHizmetData.append("PaketlerFiyat", $("#PaketTutar").val());
        PaketEkHizmetData.append("EkHizmetlerFiyat", $("#EkHizmetTutar").val());
        PaketEkHizmetData.append("Iskonto", $("#Iskonto").val());
        PaketEkHizmetData.append("ToplamTutar", $("#ToplamTutar").val());
        $.ajax({
            type: "POST",
            datatype: 'json',
            data: PaketEkHizmetData,
            contentType: false,
            processData: false,
            url: "/Otomasyon/RezervasyonIslemleri/SozlesmePaketlerEkhizmetlerGuncelle/",
            success: function (data) {
                SureclerAktif();
            },
            error: function (err) {
                alert("HATA");
            }
        });
    });
    $("#PaketlerGeri").click(function () {
        DigerBilgilerAktif();
    });
    $("#SureclerDevam").click(function () {
        var SureclerData = new FormData();
        SureclerData.append("SozlesmeId", sozlesmeId);
        SureclerData.append("SozlesmeNo", sozlesmeNo);
        SureclerData.append("secilensurecID", secilensurecID);
        $.ajax({
            type: "POST",
            datatype: 'json',
            data: SureclerData,
            contentType: false,
            processData: false,
            url: "/Otomasyon/RezervasyonIslemleri/SureclerGuncelle/",
            success: function (data) {
                CekimRandevuAktif();
            },
            error: function (err) {
                alert("HATA");
            }
        });
    });
    $("#SureclerGeri").click(function () {
        PaketVeEkhizmetlerAktif();
    });
    $("#CekimRandevuDevam").click(function () {
        OdemeBilgileriAktif();
    });
    $("#CekimRandevuGeri").click(function () {
        SureclerAktif();
    });
    $("#OdemelerGeri").click(function () {
        CekimRandevuAktif();
    });
    $("#OdemelerSozlesmeGoruntule").click(function () {
        document.getElementById("SozlesmeOnizleme").src = "/Otomasyon/RezervasyonIslemleri/SozlesmeYazdir/" + sozlesmeId;
        SozlesmeOnizlemeAktif();
        //window.location.replace("/Otomasyon/RezervasyonIslemleri/RezervasyonListesi");
    });
    $("#OnizlemeGeri").click(function () {
        OdemeBilgileriAktif();
    });
    $("#SozlesmeYazdir").click(function () {
        //window.location.replace("/Otomasyon/RezervasyonIslemleri/SozlesmeYazdir2");
        window.open("/Otomasyon/RezervasyonIslemleri/SozlesmeYazdir/" + sozlesmeId, '_blank');
    });
});