using System.Globalization;
using System.Net;
using GsFashion.Repository.Models.Rental;
using HtmlRendererCore.PdfSharp;
using Microsoft.AspNetCore.Hosting;
using PdfSharp;

namespace GsFashion.MVC.Services;

/// <summary>Creates rental invoices from an editable HTML template and keeps the PDF in memory.</summary>
public class RentalBillPdfService
{
    private readonly IWebHostEnvironment _environment;
    public RentalBillPdfService(IWebHostEnvironment environment) => _environment = environment;

    public MemoryStream Generate(RentalModel rental)
    {
        ArgumentNullException.ThrowIfNull(rental);
        var templatePath = Path.Combine(_environment.ContentRootPath, "Templates", "RentalBillTemplate.html");
        if (!File.Exists(templatePath)) throw new FileNotFoundException("Rental bill HTML template was not found.", templatePath);

        var html = PopulateTemplate(File.ReadAllText(templatePath), rental);
        var pdfDocument = PdfGenerator.GeneratePdf(html, PageSize.A4);
        var pdfStream = new MemoryStream();
        pdfDocument.Save(pdfStream, false);
        pdfStream.Position = 0;
        return pdfStream;
    }

    private string PopulateTemplate(string template, RentalModel rental)
    {
        var items = rental.InventoryItemModels?.ToList() ?? [];
        var totalAmount = rental.GrandTotal != 0 ? rental.GrandTotal : rental.TotalRentAmount + rental.SecurityDeposit + rental.LateFee + rental.DamageFee - rental.Discount;
        var values = new Dictionary<string, string>
        {
            ["{{LOGO}}"] = GetLogoDataUri(), ["{{BILL_NUMBER}}"] = $"RENT-{rental.RentalId:D5}",
            ["{{BOOKING_DATE}}"] = Date(rental.BookingDate, "dd MMM yyyy, hh:mm tt"), ["{{RENTAL_START_DATE}}"] = Date(rental.RentalStartDate, "dd MMM yyyy"), ["{{EXPECTED_RETURN_DATE}}"] = Date(rental.ExpectedReturnDate, "dd MMM yyyy"),
            ["{{CUSTOMER_NAME}}"] = Encode($"{rental.CustomerFirstName} {rental.CustomerLastName}".Trim()), ["{{CUSTOMER_PHONE}}"] = Encode(rental.CustomerPhoneNumber), ["{{CUSTOMER_EMAIL}}"] = Encode(rental.CustomerEmail), ["{{CUSTOMER_ADDRESS}}"] = Encode(rental.CustomerAddress),
            ["{{ITEM_COUNT}}"] = items.Count.ToString(CultureInfo.InvariantCulture), ["{{ITEM_ROWS}}"] = ItemRows(items),
            ["{{TOTAL_RENT}}"] = Money(rental.TotalRentAmount), ["{{TOTAL_DEPOSIT}}"] = Money(rental.SecurityDeposit), ["{{DISCOUNT}}"] = Money(rental.Discount), ["{{TOTAL_AMOUNT}}"] = Money(totalAmount), ["{{AMOUNT_PAID}}"] = Money(rental.AmountPaid), ["{{BALANCE_AMOUNT}}"] = Money(rental.BalanceAmount), ["{{NOTES}}"] = Encode(rental.Notes)
        };
        foreach (var (token, value) in values) template = template.Replace(token, value, StringComparison.Ordinal);
        return template;
    }

    private string GetLogoDataUri()
    {
        var logoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
        return File.Exists(logoPath) ? $"data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes(logoPath))}" : string.Empty;
    }

    private static string ItemRows(IEnumerable<GsFashion.Repository.Models.InventoryItem.InventoryItemModel> items) =>
        string.Join(Environment.NewLine, items.Select((item, index) => $"<tr><td>{index + 1}</td><td>{Encode(item.SkuCode)}</td><td>{Encode(item.Name)}</td><td class=\"amount\">{Money(item.BaseRentalPrice)}</td><td class=\"amount\">{Money(item.SecurityDeposit)}</td><td class=\"amount\">{Money(item.BaseRentalPrice + item.SecurityDeposit)}</td></tr>"));
    private static string Date(DateTime? value, string format) => value?.ToString(format, CultureInfo.InvariantCulture) ?? "-";
    private static string Money(decimal amount) => $"&#8377; {amount:N2}";
    private static string Encode(string? value) => WebUtility.HtmlEncode(value ?? "-");
}
