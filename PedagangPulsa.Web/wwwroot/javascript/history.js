// Data transaksi
const transactions = [
  {
    name: "Driver 10.000",
    phone: "081229231XXX",
    date: "30/09/2024 14:56",
    amount: "Rp12.120",
    status: "wp",
  },
  {
    name: "Driver 10.000",
    phone: "081401623XXX",
    date: "30/09/2024 14:55",
    amount: "Rp12.120",
    status: "wp",
  },
  {
    name: "Driver 40.000",
    phone: "085719659XXX",
    date: "30/09/2024 14:55",
    amount: "Rp42.120",
    status: "wp",
  },
  {
    name: "Indosat 58.000",
    phone: "085794866XXX",
    date: "30/09/2024 14:51",
    amount: "Rp58.850",
    status: "ok",
  },
  {
    name: "Driver 50.000",
    phone: "081258646XXX",
    date: "30/09/2024 14:50",
    amount: "Rp52.120",
    status: "ok",
  },
  {
    name: "Driver 47.000",
    phone: "081280103XXX",
    date: "30/09/2024 14:46",
    amount: "Rp49.120",
    status: "ok",
  },
  {
    name: "Indosat 10.000",
    phone: "081258646XXX",
    date: "30/09/2024 14:50",
    amount: "Rp10.850",
    status: "ok",
  },
];

// Fungsi untuk me-render transaksi
function renderTransactions() {
  const transactionList = document.getElementById("transaction-list");
  transactions.forEach((transaction) => {
    // Buat elemen transaksi
    const transactionItem = document.createElement("div");
    transactionItem.classList.add("transaction-item");

    const statusClass = transaction.status === "ok" ? "ok" : "wp";

    // Isi HTML transaksi
    transactionItem.innerHTML = `
                    <div class="transaction-info">
                        <span>${transaction.name}</span>
                        <small>${transaction.phone}</small>
                        <small>${transaction.date}</small>
                    </div>
                    <div class="transaction-amount">
                        <span>${transaction.amount}</span>
                        <div class="status ${statusClass}">${transaction.status.toUpperCase()}</div>
                    </div>
                `;
    // Tambahkan ke dalam list transaksi
    transactionList.appendChild(transactionItem);
  });
}

// Fungsi untuk membuka pop-up pencarian
function openSearchPopup() {
  document.getElementById("search-popup").style.display = "flex";
}

// Fungsi untuk menutup pop-up pencarian
function closeSearchPopup() {
  document.getElementById("search-popup").style.display = "none";
}

// Fungsi untuk melakukan pencarian berdasarkan nama atau nomor telepon
function searchTransactions() {
  const searchInput = document
    .getElementById("search-input")
    .value.toLowerCase();
  const filteredTransactions = transactions.filter(
    (transaction) =>
      transaction.name.toLowerCase().includes(searchInput) ||
      transaction.phone.toLowerCase().includes(searchInput)
  );
  renderTransactions(filteredTransactions);
}

// Panggil fungsi untuk menampilkan transaksi
renderTransactions();
