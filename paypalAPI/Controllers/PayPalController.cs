using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Orders;
using Samples;
using BraintreeHttp;

namespace paypalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPalController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> CreateOrder(bool debug = false)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody());
            //3. Call PayPal to set up a transaction
            var response = await PayPalClient.client().Execute(request);

            if (debug)
            {
                var output = response.Result<Order>();
                Console.WriteLine("Status: {0}", output.Status);
                Console.WriteLine("Order Id: {0}", output.Id);
                Console.WriteLine("Intent: {0}", output.Intent);
                Console.WriteLine("Links:");
                foreach (LinkDescription link in output.Links)
                {
                    Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                }
                AmountWithBreakdown amount = output.PurchaseUnits[0].Amount;
                Console.WriteLine("Total Amount: {0} {1}", amount.CurrencyCode, amount.Value);
            }
            var result = response.Result<Order>();
            var r = new PayPalOrder();
            r.OrderId = result.Id;
            return Ok(r);
        }

        /*
          Method to generate sample create order body with CAPTURE intent

          @return OrderRequest with created order request
         */
        private static OrderRequest BuildRequestBody()
        {
            OrderRequest orderRequest = new OrderRequest()
            {
                Intent = "CAPTURE",

                ApplicationContext = new ApplicationContext
                {
                    BrandName = "EXAMPLE INC",
                    LandingPage = "BILLING",
                    UserAction = "CONTINUE",
                    ShippingPreference = "SET_PROVIDED_ADDRESS"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                  new PurchaseUnitRequest{
                    ReferenceId =  "PUHF",
                    Description = "Sporting Goods",
                    CustomId = "CUST-HighFashions",
                    SoftDescriptor = "HighFashions",
                    Amount = new AmountWithBreakdown
                    {
                      CurrencyCode = "USD",
                      Value = "230.00",
                      Breakdown = new AmountBreakdown
                      {
                        ItemTotal = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "180.00"
                        },
                        Shipping = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "30.00"
                        },
                        Handling = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "10.00"
                        },
                        TaxTotal = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "20.00"
                        },
                        ShippingDiscount = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "10.00"
                        }
                      }
                    },
                    Items = new List<Item>
                    {
                      new Item
                      {
                        Name = "T-shirt",
                        Description = "Green XL",
                        Sku = "sku01",
                        UnitAmount = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "90.00"
                        },
                        Tax = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "10.00"
                        },
                        Quantity = "1",
                        Category = "PHYSICAL_GOODS"
                      },
                      new Item
                      {
                        Name = "Shoes",
                        Description = "Running, Size 10.5",
                        Sku = "sku02",
                        UnitAmount = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "45.00"
                        },
                        Tax = new Money
                        {
                          CurrencyCode = "USD",
                          Value = "5.00"
                        },
                        Quantity = "2",
                        Category = "PHYSICAL_GOODS"
                      }
                    },
                    Shipping = new ShippingDetails
                    {
                      Name = new Name
                      {
                        FullName = "John Doe"
                      },
                      AddressPortable = new AddressPortable
                      {
                        AddressLine1 = "123 Townsend St",
                        AddressLine2 = "Floor 6",
                        AdminArea2 = "San Francisco",
                        AdminArea1 = "CA",
                        PostalCode = "94107",
                        CountryCode = "US"
                      }
                    }
                  }
                }
            };

            return orderRequest;
        }

        [HttpGet]
        [Route("capture/{orderId}")]

        public async Task<IActionResult> CaptureOrder(string orderId, bool debug = false)
        {
            string captureId = null;
            var request = new OrdersCaptureRequest(orderId);
            request.Prefer("return=representation");
            request.RequestBody(new OrderActionRequest());
            //3. Call PayPal to capture an order
            var response = await PayPalClient.client().Execute(request);
            //4. Save the capture ID to your database. Implement logic to save capture to your database for future reference.
            if (debug)
            {
                var output = response.Result<Order>();
                Console.WriteLine("Status: {0}", output.Status);
                Console.WriteLine("Order Id: {0}", output.Id);
                Console.WriteLine("Intent: {0}", output.Intent);
                Console.WriteLine("Links:");
                foreach (LinkDescription link in output.Links)
                {
                    Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                }
                Console.WriteLine("Capture Ids: ");
                foreach (PurchaseUnit purchaseUnit in output.PurchaseUnits)
                {
                    foreach (Capture capture in purchaseUnit.Payments.Captures)
                    {
                        captureId = capture.Id;
                        Console.WriteLine("\t {0}", capture.Id);
                    }
                }
                AmountWithBreakdown amount = output.PurchaseUnits[0].Amount;
                Console.WriteLine("Buyer:");
                Console.WriteLine("\tEmail Address: {0}\n\tName: {1}\n\tPhone Number: {2}{3}", output.Payer.EmailAddress, output.Payer.Name.FullName, output.Payer.Phone.CountryCode, output.Payer.Phone.NationalNumber);
            }

            var result = response.Result<Order>();
            var r = new PayPalCapture();
            r.OrderId = orderId;
            foreach (PurchaseUnit purchaseUnit in result.PurchaseUnits)
            {
                foreach (Capture capture in purchaseUnit.Payments.Captures)
                {
                    captureId = capture.Id;
                }
            }
            r.CaptureId = captureId;
            return Ok(r);
        }

    }
}