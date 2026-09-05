using GsFashion.Repository.Models.Rental;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GsFashion.MVC.Services
{
    public class RentalBillPdfService
    {
        public byte[] Generate(RentalModel rental)
        {
            QuestPDF.Settings.License =
                LicenseType.Community;

            var document =
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);

                        page.Margin(30);

                        page.DefaultTextStyle(
                            x => x.FontSize(10));

                        // =====================================
                        // HEADER
                        // =====================================

                        page.Header()
                            .Column(column =>
                            {
                                column.Item()
                                    .AlignCenter()
                                    .Text("GS FASHION")
                                    .Bold()
                                    .FontSize(24);

                                column.Item()
                                    .AlignCenter()
                                    .Text("CHOLI RENTAL BILL")
                                    .Bold()
                                    .FontSize(14);

                                column.Item()
                                    .PaddingTop(5)
                                    .LineHorizontal(1);
                            });


                        // =====================================
                        // CONTENT
                        // =====================================

                        page.Content()
                            .PaddingVertical(15)
                            .Column(column =>
                            {
                                // ---------------------------------
                                // Bill Information
                                // ---------------------------------

                                column.Item()
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Column(left =>
                                            {
                                                left.Item()
                                                    .Text(
                                                        $"Bill No: RENT-{rental.RentalId:D5}");

                                                left.Item()
                                                    .Text(
                                                        $"Booking Date: {rental.BookingDate:dd-MM-yyyy HH:mm}");
                                            });

                                        row.RelativeItem()
                                            .AlignRight()
                                            .Column(right =>
                                            {
                                                right.Item()
                                                    .Text(
                                                        $"Rental Start: {rental.RentalStartDate:dd-MM-yyyy}");

                                                right.Item()
                                                    .Text(
                                                        $"Expected Return: {rental.ExpectedReturnDate:dd-MM-yyyy}");
                                            });
                                    });


                                column.Item()
                                    .PaddingVertical(10);


                                // ---------------------------------
                                // Customer
                                // ---------------------------------

                                column.Item()
                                    .Text("CUSTOMER DETAILS")
                                    .Bold()
                                    .FontSize(12);


                                column.Item()
                                    .PaddingTop(5)
                                    .Border(1)
                                    .Padding(8)
                                    .Column(customer =>
                                    {
                                        customer.Item()
                                            .Text(
                                                $"Name: {rental.CustomerFirstName} {rental.CustomerLastName}");

                                        customer.Item()
                                            .Text(
                                                $"Phone: {rental.CustomerPhoneNumber}");

                                        if (!string.IsNullOrWhiteSpace(
                                            rental.CustomerEmail))
                                        {
                                            customer.Item()
                                                .Text(
                                                    $"Email: {rental.CustomerEmail}");
                                        }

                                        if (!string.IsNullOrWhiteSpace(
                                            rental.CustomerAddress))
                                        {
                                            customer.Item()
                                                .Text(
                                                    $"Address: {rental.CustomerAddress}");
                                        }
                                    });


                                column.Item()
                                    .PaddingVertical(10);


                                // ---------------------------------
                                // Choli Items
                                // ---------------------------------

                                column.Item()
                                    .Text("RENTAL ITEMS")
                                    .Bold()
                                    .FontSize(12);


                                column.Item()
                                    .PaddingTop(5)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(
                                            columns =>
                                            {
                                                columns.ConstantColumn(35);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(3);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(2);
                                            });


                                        // Header

                                        table.Header(header =>
                                        {
                                            header.Cell()
                                                .Element(HeaderStyle)
                                                .Text("#");

                                            header.Cell()
                                                .Element(HeaderStyle)
                                                .Text("SKU");

                                            header.Cell()
                                                .Element(HeaderStyle)
                                                .Text("Choli");

                                            header.Cell()
                                                .Element(HeaderStyle)
                                                .AlignRight()
                                                .Text("Rent");

                                            header.Cell()
                                                .Element(HeaderStyle)
                                                .AlignRight()
                                                .Text("Deposit");
                                        });


                                        var index = 1;


                                        foreach (
                                            var item
                                            in rental.InventoryItemModels)
                                        {
                                            table.Cell()
                                                .Element(CellStyle)
                                                .Text(
                                                    index.ToString());

                                            table.Cell()
                                                .Element(CellStyle)
                                                .Text(
                                                    item.SkuCode);

                                            table.Cell()
                                                .Element(CellStyle)
                                                .Text(
                                                    item.Name);

                                            table.Cell()
                                                .Element(CellStyle)
                                                .AlignRight()
                                                .Text(
                                                    $"₹ {item.BaseRentalPrice:N2}");

                                            table.Cell()
                                                .Element(CellStyle)
                                                .AlignRight()
                                                .Text(
                                                    $"₹ {item.SecurityDeposit:N2}");

                                            index++;
                                        }
                                    });


                                column.Item()
                                    .PaddingVertical(10);


                                // ---------------------------------
                                // Charges
                                // ---------------------------------

                                column.Item()
                                    .AlignRight()
                                    .Width(250)
                                    .Column(charges =>
                                    {
                                        charges.Item()
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("Total Rent:");

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"₹ {rental.TotalRentAmount:N2}");
                                            });


                                        charges.Item()
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("Security Deposit:");

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"₹ {rental.SecurityDeposit:N2}");
                                            });


                                        charges.Item()
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("Late Fee:");

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"₹ {rental.LateFee:N2}");
                                            });


                                        charges.Item()
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("Damage Fee:");

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"₹ {rental.DamageFee:N2}");
                                            });


                                        charges.Item()
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("Discount:");

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"- ₹ {rental.Discount:N2}");
                                            });


                                        charges.Item()
                                            .PaddingTop(5)
                                            .LineHorizontal(1);


                                        charges.Item()
                                            .PaddingTop(5)
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("GRAND TOTAL")
                                                    .Bold();

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"₹ {rental.GrandTotal:N2}")
                                                    .Bold();
                                            });


                                        charges.Item()
                                            .PaddingTop(5)
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("Amount Paid:");

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"₹ {rental.AmountPaid:N2}");
                                            });


                                        charges.Item()
                                            .Row(row =>
                                            {
                                                row.RelativeItem()
                                                    .Text("Balance:")
                                                    .Bold();

                                                row.RelativeItem()
                                                    .AlignRight()
                                                    .Text(
                                                        $"₹ {rental.BalanceAmount:N2}")
                                                    .Bold();
                                            });
                                    });


                                column.Item()
                                    .PaddingTop(20);


                                // ---------------------------------
                                // Condition
                                // ---------------------------------

                                if (!string.IsNullOrWhiteSpace(
                                    rental.ConditionOut))
                                {
                                    column.Item()
                                        .Text("CONDITION OUT")
                                        .Bold()
                                        .FontSize(11);

                                    column.Item()
                                        .PaddingTop(5)
                                        .Text(
                                            rental.ConditionOut);
                                }


                                // ---------------------------------
                                // Notes
                                // ---------------------------------

                                if (!string.IsNullOrWhiteSpace(
                                    rental.Notes))
                                {
                                    column.Item()
                                        .PaddingTop(10)
                                        .Text("NOTES")
                                        .Bold()
                                        .FontSize(11);

                                    column.Item()
                                        .PaddingTop(5)
                                        .Text(
                                            rental.Notes);
                                }


                                column.Item()
                                    .PaddingTop(25)
                                    .AlignCenter()
                                    .Text(
                                        "Thank you for choosing GS Fashion.")
                                    .Bold();

                                column.Item()
                                    .AlignCenter()
                                    .Text(
                                        "Please keep this bill for your records.");
                            });


                        // =====================================
                        // FOOTER
                        // =====================================

                        page.Footer()
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span(
                                    "GS Fashion | Choli Rental");
                            });
                    });
                });


            return document.GeneratePdf();
        }


        // =========================================
        // TABLE STYLES
        // =========================================

        static IContainer HeaderStyle(
            IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten2)
                .Border(1)
                .BorderColor(Colors.Grey.Medium)
                .Padding(5);
        }


        static IContainer CellStyle(
            IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5);
        }
    }
}