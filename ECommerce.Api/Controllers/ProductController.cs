using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IRepository<Product> productRepository, IUnitOfWork unitOfWork, ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto productDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productDto.Name) || productDto.Price <= 0)
                {
                    _logger.LogWarning("Geçersiz ürün adı veya fiyatı girişi yapıldı.");
                    return BadRequest("Geçerli bir ürün adı ve fiyatı girin.");
                }

                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    OrderItems = new List<OrderItem>()
                };

                await _productRepository.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Yeni ürün eklendi: {product.Name}, Fiyat: {product.Price}");
                return Ok("Ürün başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün eklenirken bir hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDto updatedProductDto)
        {
            try
            {
                // Ürünü id ile bul
                var product = await _productRepository.GetByIdAsync(updatedProductDto.Id);

                if (product == null)
                {
                    _logger.LogWarning($"Ürün bulunamadı: {updatedProductDto.Id}");
                    return NotFound("Ürün bulunamadı.");
                }

                // DTO'dan gelen veriler ile ürünü güncelle
                product.Name = updatedProductDto.Name;
                product.Description = updatedProductDto.Description;
                product.Price = updatedProductDto.Price;

                // Ürünü veritabanında güncelle
                await _productRepository.UpdateAsync(product);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Ürün güncellendi: {product.Name}, Fiyat: {product.Price}");
                return Ok("Ürün başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün güncellenirken bir hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Silinmek istenen ürün bulunamadı: {id}");
                    return NotFound("Ürün bulunamadı.");
                }

                await _productRepository.DeleteAsync(product);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Ürün silindi: {product.Name}, Fiyat: {product.Price}");
                return Ok("Ürün silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün silinirken bir hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                _logger.LogInformation($"Toplam {products.Count()} ürün listelendi.");
                return Ok(products.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün listelenirken bir hata oluştu: {ex.Message}");
                return StatusCode(500, "İç sistem hatası oluştu.");
            }
        }
    }
}
