// document.getElementById('lunas').style.display = 'none';
// Create a new QRCode instance and attach it to the "qrcode" div
var qr = document.getElementById("qrPayment").value;
var qrcode = new QRCode(document.getElementById("qrcode"), {
    text: qr,  // URL atau teks yang ingin diubah menjadi QR code
    width: 256,                   // Lebar QR code
    height: 256,                  // Tinggi QR code
    colorDark: "#000000",         // Warna QR code
    colorLight: "#ffffff",        // Warna latar belakang
    correctLevel: QRCode.CorrectLevel.H  // Tingkat koreksi error (L, M, Q, H)
});
function downloadQr() {
    // Ambil elemen kanvas dari QR Code
    var qrCanvas = document.querySelector("#qrcode canvas");

    if (qrCanvas) {
        // Buat link download
        var downloadLink = document.createElement("a");
        downloadLink.href = qrCanvas.toDataURL("image/png");  // Konversi kanvas ke URL gambar PNG
        downloadLink.download = "QRCode.png";  // Nama file download
        downloadLink.click();  // Trigger klik untuk memulai download
    } else {
        alert("QR code tidak tersedia!");
    }
}

const client = mqtt.connect('wss://4f022b608d8849cc922c9c2b5e113148.s1.eu.hivemq.cloud:8884/mqtt', {
    username: 'hivemq.webclient.1728711517267',
    password: 'W%Tiy53.!64zBGjFwr<Y',
    protocol: 'wss'
});


client.on('connect', function () {
    console.log('Terhubung ke HiveMQ Cloud');
    client.subscribe(topic, function (err) {
        if (!err) {
            console.log('Berhasil berlangganan ke topik:', topic);
        }
    });
});

// Menangani pesan yang diterima
client.on('message', function (topic, message) {
    // Mengubah pesan menjadi string
    console.log(topic, message.toString());
    const receivedMessage = message.toString();

    // Jika pesan adalah 'paid', update status
    if (receivedMessage === 'paid') {
        document.getElementById('statusPaid').value = 'Dibayar';
        document.getElementById('lunas').style.display = 'block';
        document.getElementById('qrcode').innerHTML = '';

    } else {
        document.getElementById('statusPaid').value = 'Belum dibayar'; // Default jika bukan 'paid'
    }
});

client.on('error', function (err) {
    console.error('Terjadi error dalam koneksi MQTT:', err);
});