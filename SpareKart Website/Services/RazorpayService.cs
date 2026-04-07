using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SpareKart_Website.Services
{
    public class RazorpayService
    {
        private readonly string _keyId;
        private readonly string _keySecret;
        private readonly HttpClient _httpClient;

        public RazorpayService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"]!;
            _keySecret = configuration["Razorpay:KeySecret"]!;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.razorpay.com/v1/");

            // Basic Auth header
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_keyId}:{_keySecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        public string KeyId => _keyId;

        /// <summary>
        /// Creates a Razorpay order and returns the order ID.
        /// Amount should be in rupees — this method converts to paise.
        /// </summary>
        public async Task<string?> CreateOrderAsync(decimal amountInRupees, string currency = "INR", string receipt = "")
        {
            var body = new
            {
                amount = (int)(amountInRupees * 100), // Convert to paise
                currency = currency,
                receipt = receipt,
                payment_capture = 1 // Auto-capture
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("orders", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    return doc.RootElement.GetProperty("id").GetString();
                }
                else
                {
                    Console.WriteLine($"Razorpay order creation failed: {responseBody}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Razorpay error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifies the Razorpay payment signature to ensure authenticity.
        /// </summary>
        public bool VerifyPaymentSignature(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature)
        {
            var payload = $"{razorpayOrderId}|{razorpayPaymentId}";
            var secret = Encoding.UTF8.GetBytes(_keySecret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(secret);
            var hash = hmac.ComputeHash(payloadBytes);
            var expectedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return expectedSignature == razorpaySignature;
        }
    }
}
