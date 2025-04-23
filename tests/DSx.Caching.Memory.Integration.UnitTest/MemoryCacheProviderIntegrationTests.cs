using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DSx.Caching.Abstractions.Models;
using DSx.Caching.SharedKernel.Validation;
using DSx.Caching.Providers.Memory;

namespace DSx.Caching.Memory.Integration.UnitTest
{
    /// <summary>
    /// Test di integrazione per il provider di cache in memoria
    /// </summary>
    public class MemoryCacheProviderIntegrationTests : IDisposable
    {
        private MemoryCacheProvider _cacheProvider;
        private readonly MemoryCache _memoryCache;
        private readonly Mock<ILogger<MemoryCacheProvider>> _mockLogger = new();

        /// <summary>
        /// Inizializza una nuova istanza del provider di cache per i test
        /// </summary>
        public MemoryCacheProviderIntegrationTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheProvider = new MemoryCacheProvider(
                cache: _memoryCache,
                logger: _mockLogger.Object,
                keyValidator: new CacheKeyValidator(),
                defaultExpiration: TimeSpan.FromMinutes(10)
            );
        }

        /// <summary>
        /// Test per verificare l'inserimento e il recupero di un oggetto complesso
        /// </summary>
        [Fact]
        public async Task SetAndGetComplexObject_ShouldWorkCorrectly()
        {
            // Arrange
            var order = new Order(
                id: Guid.NewGuid(),
                customerName: "Mario Rossi",
                items: new List<OrderItem>
                {
                    new("Laptop", 1, 1200.50m),
                    new("Mouse", 2, 25.99m)
                });

            // Act
            await _cacheProvider.SetAsync("order_123", order);
            var result = await _cacheProvider.GetAsync<Order>("order_123");

            // Assert
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Items.Count);
        }

        /// <summary>
        /// Esegue la pulizia delle risorse dopo l'esecuzione dei test
        /// </summary>
        public void Dispose()
        {
            _cacheProvider.ClearAllAsync().GetAwaiter().GetResult();
            _cacheProvider = null!;
            _memoryCache.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dati di test per rappresentare un ordine
        /// </summary>
        public class Order
        {
            /// <summary>
            /// Identificatore univoco dell'ordine
            /// </summary>
            public Guid Id { get; }

            /// <summary>
            /// Nome del cliente
            /// </summary>
            public string CustomerName { get; }

            /// <summary>
            /// Articoli contenuti nell'ordine
            /// </summary>
            public List<OrderItem> Items { get; }

            /// <summary>
            /// Crea una nuova istanza di Order
            /// </summary>
            /// <param name="id">Identificatore univoco</param>
            /// <param name="customerName">Nome cliente</param>
            /// <param name="items">Lista articoli</param>
            public Order(Guid id, string customerName, List<OrderItem> items)
            {
                Id = id;
                CustomerName = customerName;
                Items = items;
            }
        }

        /// <summary>
        /// Dati di test per rappresentare un articolo all'interno di un ordine
        /// </summary>
        public class OrderItem
        {
            /// <summary>
            /// Nome del prodotto
            /// </summary>
            public string ProductName { get; }

            /// <summary>
            /// Quantità ordinata
            /// </summary>
            public int Quantity { get; }

            /// <summary>
            /// Prezzo unitario del prodotto
            /// </summary>
            public decimal UnitPrice { get; }

            /// <summary>
            /// Crea una nuova istanza di OrderItem
            /// </summary>
            /// <param name="productName">Nome prodotto</param>
            /// <param name="quantity">Quantità</param>
            /// <param name="unitPrice">Prezzo unitario</param>
            public OrderItem(string productName, int quantity, decimal unitPrice)
            {
                ProductName = productName;
                Quantity = quantity;
                UnitPrice = unitPrice;
            }
        }
    }
}
