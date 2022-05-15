using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using SBI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SBI.Controllers
{
    //    public class PaymentController : Controller
    //    {
    //        public IActionResult Index()
    //        {
    //            return View();
    //        }
    //    }
    //}
    //public class PaymentController : Controller
    //{
    //    // GET: Payment
    //    public IActionResult Index()
    //    {
    //        return View();
    //    }


    //    [HttpPost]
    //    public IActionResult CreateOrder(Models.PaymentInitiateModel _requestData)
    //    {
    //        // Generate random receipt number for order
    //        Random randomObj = new Random();
    //        string transactionId = randomObj.Next(10000000, 100000000).ToString();

    //        Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_live_jR2zihWG8pg9Hw", "5zLPpowUbjr7mJ5FAOfuljf4");
    //        Dictionary<string, object> options = new Dictionary<string, object>();
    //        options.Add("amount", _requestData.amount * 100);  // Amount will in paise
    //        options.Add("receipt", transactionId);
    //        options.Add("currency", "INR");
    //        options.Add("payment_capture", "0"); // 1 - automatic  , 0 - manual
    //                                             //options.Add("notes", "-- You can put any notes here --");
    //        Razorpay.Api.Order orderResponse = client.Order.Create(options);
    //        string orderId = orderResponse["id"].ToString();

    //        // Create order model for return on view
    //        OrderModel orderModel = new OrderModel
    //        {
    //            orderId = orderResponse.Attributes["id"],
    //            razorpayKey = "rzp_live_jR2zihWG8pg9Hw",
    //            amount = _requestData.amount * 100,
    //            currency = "INR",
    //            name = _requestData.name,
    //            email = _requestData.email,
    //            contactNumber = _requestData.contactNumber,
    //            address = _requestData.address,
    //            description = "Testing description"
    //        };

    //        // Return on PaymentPage with Order data
    //        return View("PaymentPage", orderModel);
    //    }

    //    public class OrderModel
    //    {
    //        public string orderId { get; set; }
    //        public string razorpayKey { get; set; }
    //        public int amount { get; set; }
    //        public string currency { get; set; }
    //        public string name { get; set; }
    //        public string email { get; set; }
    //        public string contactNumber { get; set; }
    //        public string address { get; set; }
    //        public string description { get; set; }
    //    }


    //    //[HttpPost]
    //    //public IActionResult Complete()
    //    //{
    //    //    // Payment data comes in url so we have to get it from url

    //    //    // This id is razorpay unique payment id which can be use to get the payment details from razorpay server
    //    //    string paymentId = Request.Params["rzp_paymentid"];

    //    //    // This is orderId
    //    //    string orderId = Request.Params["rzp_orderid"];

    //    //    Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_live_jR2zihWG8pg9Hw", "5zLPpowUbjr7mJ5FAOfuljf4");

    //    //    Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

    //    //    // This code is for capture the payment 
    //    //    Dictionary<string, object> options = new Dictionary<string, object>();
    //    //    options.Add("amount", payment.Attributes["amount"]);
    //    //    Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
    //    //    string amt = paymentCaptured.Attributes["amount"];

    //    //    //// Check payment made successfully

    //    //    if (paymentCaptured.Attributes["status"] == "captured")
    //    //    {
    //    //        // Create these action method
    //    //        return RedirectToAction("Success");
    //    //    }
    //    //    else
    //    //    {
    //    //        return RedirectToAction("Failed");
    //    //    }
    //    //}

    //    public IActionResult Success()
    //    {
    //        return View();
    //    }

    //    public IActionResult Failed()
    //    {
    //        return View();
    //    }
    //}
    //}
    public class PaymentController : Controller
    {
        private const string _key = "rzp_live_jR2zihWG8pg9Hw";
        private const string _secret = "5zLPpowUbjr7mJ5FAOfuljf4";
            public ViewResult Registration()
            {
                var model = new RegistrationModel() { Amount = 2 };
                return View(model);
            }

            public ViewResult Payment(RegistrationModel registration)
            {
                OrderModel order = new OrderModel()
                {
                    OrderAmount = registration.Amount,
                    Currency = "INR",
                    Payment_Capture = 1,    // 0 - Manual capture, 1 - Auto capture
                    Notes = new Dictionary<string, string>()
                {
                    { "note 1", "first note while creating order" }, { "note 2", "you can add max 15 notes" },
                    { "note for account 1", "this is a linked note for account 1" }, { "note 2 for second transfer", "it's another note for 2nd account" }
                }
                };
                // var orderId = CreateOrder(order);
                var orderId = CreateTransfersViaOrder(order);

                RazorPayOptionsModel razorPayOptions = new RazorPayOptionsModel()
                {
                    Key = _key,
                    AmountInSubUnits = order.OrderAmountInSubUnits,
                    Currency = order.Currency,
                    Name = "RICASE",
                    Description = "for Dotnet",
                    ImageLogUrl = "",
                    OrderId = orderId,
                    ProfileName = registration.Name,
                    ProfileContact = registration.Mobile,
                    ProfileEmail = registration.Email,
                    Notes = new Dictionary<string, string>()
                {
                    { "note 1", "this is a payment note" }, { "note 2", "here also, you can add max 15 notes" }
                }
                };
                return View(razorPayOptions);
            }

            private string CreateOrder(OrderModel order)
            {
                try
                {
                    RazorpayClient client = new RazorpayClient(_key, _secret);
                    Dictionary<string, object> options = new Dictionary<string, object>();
                    options.Add("amount", order.OrderAmountInSubUnits);
                    options.Add("currency", order.Currency);
                    options.Add("payment_capture", order.Payment_Capture);
                    options.Add("notes", order.Notes);

                    Order orderResponse = client.Order.Create(options);
                    var orderId = orderResponse.Attributes["id"].ToString();
                    return orderId;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            private string CreateTransfersViaOrder(OrderModel order)
            {
                try
                {
                    RazorpayClient client = new RazorpayClient(_key, _secret);
                    Dictionary<string, object> options = new Dictionary<string, object>();
                    options.Add("amount", order.OrderAmountInSubUnits);
                    options.Add("currency", order.Currency);
                    options.Add("payment_capture", order.Payment_Capture);
                    options.Add("notes", order.Notes);

                    List<Dictionary<string, object>> transfers = new List<Dictionary<string, object>>();

                    // Tranfer to Account 1
                    Dictionary<string, object> transfer = new Dictionary<string, object>();
                    transfer.Add("account", "acc_FrZdKIHffMifPl");              // account 1
                    transfer.Add("amount", order.OrderAmountInSubUnits / 2);    // 50% amount of the total amount
                    transfer.Add("currency", "INR");
                    transfer.Add("notes", order.Notes);
                    List<string> linkedAccountNotes = new List<string>();
                    linkedAccountNotes.Add("note for account 1");
                    transfer.Add("linked_account_notes", linkedAccountNotes);
                    transfers.Add(transfer);

                    // Transfer to Account 2
                    transfer = new Dictionary<string, object>();
                    transfer.Add("account", "acc_FrZbSTT96Jfp6n");              // account 2
                    transfer.Add("amount", order.OrderAmountInSubUnits / 2);    // 50% amount of the total amount
                    transfer.Add("currency", "INR");
                    transfer.Add("notes", order.Notes);
                    linkedAccountNotes = new List<string>();
                    linkedAccountNotes.Add("note 2 for second transfer");
                    transfer.Add("linked_account_notes", linkedAccountNotes);
                    transfers.Add(transfer);

                    // Add transfers to options object
                    options.Add("transfers", transfers);

                    Order orderResponse = client.Order.Create(options);
                    var orderId = orderResponse.Attributes["id"].ToString();
                    return orderId;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public ViewResult AfterPayment()
            {
                var paymentStatus = Request.Form["paymentstatus"].ToString();
                if (paymentStatus == "Fail")
                    return View("Fail");

                var orderId = Request.Form["orderid"].ToString();
                var paymentId = Request.Form["paymentid"].ToString();
                var signature = Request.Form["signature"].ToString();

                var validSignature = CompareSignatures(orderId, paymentId, signature);
                if (validSignature)
                {
                    ViewBag.Message = "Congratulations!! Your payment was successful";
                    return View("Success");
                }
                else
                {
                    return View("Fail");
                }
            }

            private bool CompareSignatures(string orderId, string paymentId, string razorPaySignature)
            {
                var text = orderId + "|" + paymentId;
                var secret = _secret;
                var generatedSignature = CalculateSHA256(text, secret);
                if (generatedSignature == razorPaySignature)
                    return true;
                else
                    return false;
            }

            private string CalculateSHA256(string text, string secret)
            {
                string result = "";
                var enc = Encoding.Default;
                byte[]
                baText2BeHashed = enc.GetBytes(text),
                baSalt = enc.GetBytes(secret);
                System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
                byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
                result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
                return result;
            }

            public ViewResult Capture()
            {
                return View();
            }

            public ViewResult CapturePayment(string paymentId)
            {
                RazorpayClient client = new RazorpayClient(_key, _secret);
                Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);
                var amount = payment.Attributes["amount"];
                var currency = payment.Attributes["currency"];

                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", amount);
                options.Add("currency", currency);
                Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
                ViewBag.Message = "Payment capatured!";
                return View("Success");
            }
        }
    }

