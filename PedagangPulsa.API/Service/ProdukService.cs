using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using PedagangPulsa.API.Interface;
using PedagangPulsa.API.Model;
using System.Net;

namespace PedagangPulsa.API.Service
{
    public class ProdukService : IProdukService
    {
        private readonly string _connectionString;
        private static readonly HttpClient _httpClient = new HttpClient();


        public ProdukService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Sinkronisasi produk dari API eksternal
        public async Task SyncProdukAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://dflash.co.id/harga/pricelist_json2.php");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");

            var content = new StringContent("", null, "text/plain");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            Console.WriteLine(await response.Content.ReadAsStringAsync());


            var jsonData = JArray.Parse(await response.Content.ReadAsStringAsync()); // Parse JSON
            
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                foreach (var item in jsonData)
                {
                    var provider = item["provider"].ToString();
                    var kategori = item["kategori"].ToString();

                    // Insert atau Update KategoriProduk
                    var kategoriID = InsertOrUpdateKategori(connection, provider, kategori);

                    foreach (var produk in item["data"])
                    {
                        var kode = produk["kode"].ToString();
                        var nama = produk["nama"].ToString();
                        var harga = decimal.Parse(produk["harga"].ToString());
                        var status = int.Parse(produk["status"].ToString());

                        // Insert atau Update DetailProduk
                        InsertOrUpdateProduk(connection, kategoriID, kode, nama, harga, status);
                    }
                }

                connection.Close();
            }
        }

        // Insert atau Update manual produk
        public async Task UpsertProdukAsync(DetailProduk produk)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Insert atau update produk
                InsertOrUpdateProduk(connection, produk.KategoriID, produk.Kode, produk.Nama, produk.Harga, produk.Status);

                connection.Close();
            }

            await Task.CompletedTask;
        }

        // Insert atau Update KategoriProduk
        private int InsertOrUpdateKategori(SqliteConnection connection, string provider, string kategori)
        {
            var selectKategoriQuery = "SELECT KategoriID FROM KategoriProduk WHERE Provider = @Provider AND Kategori = @Kategori";
            using (var command = new SqliteCommand(selectKategoriQuery, connection))
            {
                command.Parameters.AddWithValue("@Provider", provider);
                command.Parameters.AddWithValue("@Kategori", kategori);
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    // Kategori sudah ada, return KategoriID
                    return Convert.ToInt32(result);
                }
                else
                {
                    // Insert kategori baru
                    var insertKategoriQuery = "INSERT INTO KategoriProduk (Provider, Kategori) VALUES (@Provider, @Kategori)";
                    using (var insertCommand = new SqliteCommand(insertKategoriQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Provider", provider);
                        insertCommand.Parameters.AddWithValue("@Kategori", kategori);
                        insertCommand.ExecuteNonQuery();

                        // Ambil KategoriID yang baru dimasukkan
                        var lastInsertIdQuery = "SELECT last_insert_rowid()";
                        using (var lastInsertCommand = new SqliteCommand(lastInsertIdQuery, connection))
                        {
                            var lastInsertId = lastInsertCommand.ExecuteScalar();
                            return Convert.ToInt32(lastInsertId); // Return KategoriID baru
                        }
                    }
                }
            }
        }

        // Insert atau Update DetailProduk
        private void InsertOrUpdateProduk(SqliteConnection connection, int kategoriID, string kode, string nama, decimal harga, int status)
        {
            var selectProdukQuery = "SELECT ProdukID FROM DetailProduk WHERE Kode = @Kode AND KategoriID = @KategoriID";
            using (var command = new SqliteCommand(selectProdukQuery, connection))
            {
                command.Parameters.AddWithValue("@Kode", kode);
                command.Parameters.AddWithValue("@KategoriID", kategoriID);
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    // Produk sudah ada, lakukan update
                    var updateProdukQuery = "UPDATE DetailProduk SET Nama = @Nama, Harga = @Harga, Status = @Status WHERE ProdukID = @ProdukID";
                    using (var updateCommand = new SqliteCommand(updateProdukQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Nama", nama);
                        updateCommand.Parameters.AddWithValue("@Harga", harga);
                        updateCommand.Parameters.AddWithValue("@Status", status);
                        updateCommand.Parameters.AddWithValue("@ProdukID", Convert.ToInt32(result));
                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Insert produk baru
                    var insertProdukQuery = "INSERT INTO DetailProduk (KategoriID, Kode, Nama, Harga, Status) VALUES (@KategoriID, @Kode, @Nama, @Harga, @Status)";
                    using (var insertCommand = new SqliteCommand(insertProdukQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@KategoriID", kategoriID);
                        insertCommand.Parameters.AddWithValue("@Kode", kode);
                        insertCommand.Parameters.AddWithValue("@Nama", nama);
                        insertCommand.Parameters.AddWithValue("@Harga", harga);
                        insertCommand.Parameters.AddWithValue("@Status", status);
                        insertCommand.ExecuteNonQuery();

                        // Jika perlu mengambil ProdukID yang baru
                        var lastInsertIdQuery = "SELECT last_insert_rowid()";
                        using (var lastInsertCommand = new SqliteCommand(lastInsertIdQuery, connection))
                        {
                            var lastInsertId = lastInsertCommand.ExecuteScalar();
                            // ProdukID dari produk yang baru dimasukkan dapat diakses dari sini jika diperlukan
                        }
                    }
                }
            }
        }
    }
}
