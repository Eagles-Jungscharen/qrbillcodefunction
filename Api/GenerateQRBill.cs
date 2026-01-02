using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EaglesJungscharen.Azure.Model;
using Codecrete.SwissQRBill.Generator;
using Microsoft.Azure.Functions.Worker;
using Codecrete.SwissQRBill.PixelCanvas;

namespace EaglesJungscharen.Azure.Api;

public class GenerateQRBill(ILogger<GenerateQRBill> logger)
{
    private readonly ILogger<GenerateQRBill> _logger = logger;

    [Function("GenerateQRBill")]
    public async Task<IActionResult> RunGenerateQRBill([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("Start Generating QR Code");
        try
        {
            bool asPNG = req.Query.ContainsKey("png") && req.Query["png"] == "1";
            var data = await req.ReadFromJsonAsync<InputBill>();
            if (data is null)
            {
                return new BadRequestObjectResult(new { error = "No Bill found!" });
            }
            Bill bill = new()
            {
                // creditor data
                Account = data.Account,
                Creditor = new Address
                {
                    Name = data.Creditor.Name,
                    Street = data.Creditor.Street,
                    HouseNo = data.Creditor.HouseNumber,
                    PostalCode = data.Creditor.PostalCode,
                    Town = data.Creditor.Town,
                    CountryCode = data.Creditor.CountryCode
                },

                // payment data
                Currency = data.Currency,

                // debtor data
                Debtor = new Address
                {
                    Name = data.Debitor.Name,
                    Street = data.Debitor.Street,
                    HouseNo = data.Debitor.HouseNumber,
                    PostalCode = data.Debitor.PostalCode,
                    Town = data.Debitor.Town,
                    CountryCode = data.Debitor.CountryCode
                },

                // more payment data
                UnstructuredMessage = data.InfoText,

            };

            if (data.Amount.HasValue)
            {
                bill.Amount = data.Amount.Value;
            }
            if (!string.IsNullOrEmpty(data.ReferenceNumber))
            {
                bill.CreateAndSetCreditorReference(data.ReferenceNumber);
            }
            _logger.LogInformation("ReferenceNumber {ReferenceNumber}", data.ReferenceNumber);
            bill.Format.Language = Language.DE;
            if (asPNG)
            {
                using PNGCanvas canvas = new(QRBill.QrBillWidth, QRBill.QrBillHeight, 300, "Arial");
                QRBill.Draw(bill, canvas);
                byte[] png = canvas.ToByteArray();
                return new FileContentResult(png, "image/png")
                {
                    FileDownloadName = "qrbill.png"
                };
            }
            else
            {
                byte[] svg = QRBill.Generate(bill);
                return new FileContentResult(svg, "image/svg+xml")
                {
                    FileDownloadName = "qrbill.svg"
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "General Error");
            return new BadRequestObjectResult(new{ error=e.Message});
        }
    }
}
