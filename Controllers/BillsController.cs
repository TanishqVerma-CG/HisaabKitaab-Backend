using Microsoft.AspNetCore.Mvc;
using BillingSystem.DTOs;
using BillingSystem.Services;

namespace BillingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;
    private readonly IPdfService _pdfService;

    public BillsController(IBillService billService, IPdfService pdfService)
    {
        _billService = billService;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BillDto>>> GetAll()
    {
        var bills = await _billService.GetAllBillsAsync();
        return Ok(bills);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BillDto>> GetById(int id)
    {
        var bill = await _billService.GetBillByIdAsync(id);
        return Ok(bill);
    }

    [HttpPost]
    public async Task<ActionResult<BillDto>> Create(CreateBillDto dto)
    {
        var bill = await _billService.CreateBillAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = bill.Id }, bill);
    }

    [HttpGet("{id}/pdf")]
    public async Task<ActionResult> DownloadPdf(int id)
    {
        try
        {
            var bill = await _billService.GetBillByIdAsync(id);

            if (string.IsNullOrEmpty(bill.PdfPath))
            {
                return BadRequest(new { message = "PDF not generated for this bill" });
            }

            var pdfBytes = await _pdfService.GetBillPdfAsync(bill.PdfPath);

            return File(pdfBytes, "application/pdf", $"{bill.BillNumber}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}