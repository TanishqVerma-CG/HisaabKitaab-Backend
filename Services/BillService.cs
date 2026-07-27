using AutoMapper;
using BillingSystem.Data.Repositories;
using BillingSystem.DTOs;
using BillingSystem.Models;

namespace BillingSystem.Services;

public class BillService : IBillService
{
    private readonly IBillRepository _billRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerProductPriceRepository _priceRepository;
    private readonly IPdfService _pdfService;
    private readonly IMapper _mapper;

    public BillService(
        IBillRepository billRepository,
        IProductRepository productRepository,
        ICustomerProductPriceRepository priceRepository,
        IPdfService pdfService,
        IMapper mapper)
    {
        _billRepository = billRepository;
        _productRepository = productRepository;
        _priceRepository = priceRepository;
        _pdfService = pdfService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BillDto>> GetAllBillsAsync()
    {
        var bills = await _billRepository.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<BillDto>>(bills);
    }

    public async Task<BillDto> GetBillByIdAsync(int id)
    {
        var bill = await _billRepository.GetBillWithDetailsAsync(id);
        if (bill == null)
            throw new Exception("Bill not found");

        return _mapper.Map<BillDto>(bill);
    }

    public async Task<BillDto> CreateBillAsync(CreateBillDto dto)
    {
        var billNumber = await _billRepository.GenerateNextBillNumberAsync();

        var bill = new Bill
        {
            BillNumber = billNumber,
            CustomerId = dto.CustomerId,
            BillDate = dto.BillDate,
            BillItems = new List<BillItem>()
        };

        decimal subtotal = 0;
        decimal totalGst = 0;

        foreach (var item in dto.BillItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new Exception($"Product {item.ProductId} not found");

            var customerPrice = await _priceRepository.GetByCustomerAndProductAsync(dto.CustomerId, item.ProductId);
            var rate = customerPrice?.Price ?? product.BasePrice;

            var amount = item.Quantity * rate;
            var gstAmount = amount * (product.GstRate / 100);

            var billItem = new BillItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Rate = rate,
                Amount = amount,
                GstRate = product.GstRate
            };

            bill.BillItems.Add(billItem);
            subtotal += amount;
            totalGst += gstAmount;
        }

        bill.Subtotal = subtotal;
        bill.GstAmount = totalGst;
        bill.GrandTotal = subtotal + totalGst;

        await _billRepository.AddAsync(bill);

        var createdBill = await _billRepository.GetBillWithDetailsAsync(bill.Id);

        var pdfPath = await _pdfService.GenerateBillPdfAsync(createdBill);
        createdBill.PdfPath = pdfPath;
        await _billRepository.UpdateAsync(createdBill);

        return _mapper.Map<BillDto>(createdBill);
    }
}