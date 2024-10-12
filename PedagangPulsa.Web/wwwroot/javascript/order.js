// Function untuk merender produk
function renderProducts(products) {
    const productList = document.getElementById("productList");
    productList.innerHTML = "";

    products.forEach((product) => {
        const productItem = document.createElement("div");
        productItem.className = "product-item";
        const formattedHarga = product.harga.toLocaleString('id-ID');

        productItem.innerHTML = `
            <h4>${product.nama_Produk}</h4>
            <p>${product.kode} - ${product.deskripsi_produk}</p>
            <p>${formattedHarga}</p>
        `;

        // Event listener untuk membuka modal
        productItem.addEventListener("click", function () {
            showProductDetails(product);
        });

        productList.appendChild(productItem);
    });
}

var productKode = "";
var selectedOperator = "";


// Function untuk menampilkan modal dengan detail produk
function showProductDetails(product) {
    document.getElementById("productType").innerText = "Pulsa";
    document.getElementById("productName").innerText = product.kode + " " +product.nama_Produk;;
    document.getElementById("provider").innerText = product.deskripsi_produk;
    document.getElementById("productPrice").innerText = product.harga.toLocaleString('id-ID');;
    document.getElementById("productPhone").innerText = document.getElementById("phoneNumber").value;

    const modal = document.getElementById("productModal");
    modal.style.display = "block";

    if (document.getElementById("metodeBayar").value != 1) 
        document.getElementById("payWithSaldo").hidden = true;
    else
        document.getElementById("payWithSaldo").hidden = false;

    productKode = product.kode;
}




async function sendPaymentRequest() {
    // Data yang akan dikirim ke backend
    const paymentRequest = {
        operator_Id: selectedOperator,  // Ganti dengan operator ID yang sesuai
        kode: productKode,    // Ganti dengan kode yang sesuai
        no_tujuan: document.getElementById("phoneNumber").value  // Nomor tujuan dari input pengguna
    };

    try {
        // Melakukan POST request ke endpoint /Payment
        const response = await fetch('/Transaksi/Payment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value // CSRF Token
            },
            body: JSON.stringify(paymentRequest)
        });

        const result = await response.json(); // Mendapatkan response dari server

        if (result.success) {
            console.log('Pembayaran berhasil, TRX ID:', result.trx_id);
            window.location.href = "/Transaksi/History?id=" + result.trx_id;
        } else {
            console.log('Gagal memproses pembayaran:', result.message);
            alert('Gagal memproses pembayaran: ' + result.message);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Terjadi kesalahan dalam memproses pembayaran.');
    }
}



// Close modal when clicked outside of it
window.onclick = function (event) {
    const modal = document.getElementById("productModal");
    if (event.target == modal) {
        modal.style.display = "none";
    }
};

// Filter produk berdasarkan operator
document.getElementById("operator").addEventListener("change", async function () {
    selectedOperator = this.value;
    console.log('selectedOperator:', selectedOperator);

    // Get data from backend using fetch API (improved with async-await for cleaner code)
    const url = "DetailProduk?operatorId=" + selectedOperator;
    console.log('url', url);
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }
        const products = await response.json(); // Assuming the response is JSON formatted
        console.log('products', products.data);

        // Render products
        renderProducts(products.data);

    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
});


// Close modal function
document.getElementById("closeModal").onclick = function () {
    const modal = document.getElementById("productModal");
    modal.style.display = "none";
};

// Button Beli
document.getElementById("buyNowButton").onclick = function () {
    sendPaymentRequest();
};