using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EaglesJungscharen.Azure.Model;
using Codecrete.SwissQRBill.Generator;

namespace EaglesJungscharen.Azure.Functions
{
    public static class GenerateQRBill
    {
        [FunctionName("GenerateQRBill")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            InputBill data = JsonConvert.DeserializeObject<InputBill>(requestBody);
            if (data == null) {
                return new BadRequestObjectResult(new {error="No Bill found!"});
            }
            Bill bill = new Bill
            {
                // creditor data
                Account = data.Account,
                Creditor = new Address
                {
                    Name = data.Creditor.Name,
                    AddressLine1 = data.Creditor.AddressLine1,
                    AddressLine2 = data.Creditor.AddressLine2,
                    CountryCode = data.Creditor.CountryCode
                },

                // payment data
                Amount = data.Amount,
                Currency = data.Currency,
                
                // debtor data
                Debtor = new Address
                {
                    Name = data.Debitor.Name,
                    AddressLine1 = data.Debitor.AddressLine1,
                    AddressLine2 = data.Debitor.AddressLine2,
                    CountryCode = data.Debitor.CountryCode
                },

                // more payment data
                Reference = data.ReferenceNumber,
                UnstructuredMessage = data.InfoText,
                
            };
            bill.Format.Language = Language.DE;
            // Generate QR bill
            byte[] svg = QRBill.Generate(bill);

            // Save generated SVG file
            return new FileContentResult(svg, "image/svg+xml") {
                FileDownloadName = "qrbill.svg"
            };
        }
    }
}
