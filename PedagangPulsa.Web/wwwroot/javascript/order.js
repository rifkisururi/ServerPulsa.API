// Data produk untuk demo
const products = [
  { name: "AXIS 5.000", provider: "AXIS", price: "Rp 5.930" },
  { name: "AXIS 10.000", provider: "AXIS", price: "Rp 10.910" },
  { name: "AXIS 15.000", provider: "AXIS", price: "Rp 15.075" },
  { name: "AXIS 25.000", provider: "AXIS", price: "Rp 25.930" },
  { name: "AXIS 50.000", provider: "AXIS", price: "Rp 51.000" },
  { name: "AXIS 100.000", provider: "AXIS", price: "Rp 101.500" },

  { name: "TELKOMSEL 5.000", provider: "TELKOMSEL", price: "Rp 5.930" },
  { name: "TELKOMSEL 10.000", provider: "TELKOMSEL", price: "Rp 10.910" },
  { name: "TELKOMSEL 15.000", provider: "TELKOMSEL", price: "Rp 15.075" },
  { name: "TELKOMSEL 25.000", provider: "TELKOMSEL", price: "Rp 25.930" },
  { name: "TELKOMSEL 50.000", provider: "TELKOMSEL", price: "Rp 50.930" },
  { name: "TELKOMSEL 100.000", provider: "TELKOMSEL", price: "Rp 101.500" },

  { name: "XL 5.000", provider: "XL", price: "Rp 5.930" },
  { name: "XL 10.000", provider: "XL", price: "Rp 10.910" },
  { name: "XL 15.000", provider: "XL", price: "Rp 15.075" },
  { name: "XL 25.000", provider: "XL", price: "Rp 25.930" },
  { name: "XL 50.000", provider: "XL", price: "Rp 50.930" },
  { name: "XL 100.000", provider: "XL", price: "Rp 101.500" },

  { name: "INDOSAT 5.000", provider: "INDOSAT", price: "Rp 5.930" },
  { name: "INDOSAT 10.000", provider: "INDOSAT", price: "Rp 10.910" },
  { name: "INDOSAT 15.000", provider: "INDOSAT", price: "Rp 15.075" },
  { name: "INDOSAT 25.000", provider: "INDOSAT", price: "Rp 25.930" },
  { name: "INDOSAT 50.000", provider: "INDOSAT", price: "Rp 50.930" },
  { name: "INDOSAT 100.000", provider: "INDOSAT", price: "Rp 101.500" },

  { name: "SMARTFREN 5.000", provider: "SMARTFREN", price: "Rp 5.930" },
  { name: "SMARTFREN 10.000", provider: "SMARTFREN", price: "Rp 10.910" },
  { name: "SMARTFREN 15.000", provider: "SMARTFREN", price: "Rp 15.075" },
  { name: "SMARTFREN 25.000", provider: "SMARTFREN", price: "Rp 25.930" },
  { name: "SMARTFREN 50.000", provider: "SMARTFREN", price: "Rp 50.930" },
  { name: "SMARTFREN 100.000", provider: "SMARTFREN", price: "Rp 101.500" },
];

// Function untuk merender produk
function renderProducts(products) {
  const productList = document.getElementById("productList");
  productList.innerHTML = "";

  products.forEach((product) => {
    const productItem = document.createElement("div");
    productItem.className = "product-item";
    productItem.innerHTML = `
            <h4>${product.name}</h4>
            <p>${product.provider}</p>
            <p>${product.price}</p>
        `;

    // Event listener untuk membuka modal
    productItem.addEventListener("click", function () {
      showProductDetails(product);
    });

    productList.appendChild(productItem);
  });
}

// Function untuk menampilkan modal dengan detail produk
function showProductDetails(product) {
  document.getElementById("productType").innerText = "Pulsa";
  document.getElementById("provider").innerText = product.provider;
  document.getElementById("productName").innerText = product.name;
  document.getElementById("productPrice").innerText = product.price;
  document.getElementById("productPhone").innerText = document.getElementById(
    "phoneNumber"
  ).value = "08334343434";

  const modal = document.getElementById("productModal");
  modal.style.display = "block";
}

// Close modal function
document.getElementById("closeModal").onclick = function () {
  const modal = document.getElementById("productModal");
  modal.style.display = "none";
};

// Close modal when clicked outside of it
window.onclick = function (event) {
  const modal = document.getElementById("productModal");
  if (event.target == modal) {
    modal.style.display = "none";
  }
};

// Filter produk berdasarkan operator
document.getElementById("operator").addEventListener("change", function () {
  const selectedOperator = this.value;
  const filteredProducts = products.filter(
    (product) => product.provider === selectedOperator
  );
  renderProducts(filteredProducts);
});

// Render produk awal
renderProducts(products);
