//using Stripe.Checkout;
//using System.Security.Claims;
//using Web.Entites.ViewModels;

//namespace ShopEgypt.Web.Controllers
//{
//    public class ShoppingCartController : Controller
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        public ShoppingCartController(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//        public IActionResult Index()
//        {
//            var claims = (ClaimsIdentity)User.Identity;
//            var userID = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
//            var carts = new ShoppingCartVM
//            {
//                ShoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userID, includeObj: "Product").ToList(),
//                OrderHeader = new OrderHeader()
//            };
//            carts.OrderHeader.TotalPrice=_unitOfWork.ShoppingCartRepository.GetTotalPrice(carts.ShoppingCarts);
//            return View(carts);
//        }
//        [HttpGet]
//        public IActionResult Plus(int id)
//        {
//            var claims = (ClaimsIdentity)User.Identity;
//            var userID = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            var cartFromDB=_unitOfWork.ShoppingCartRepository.GetBy(x=>x.ShoppingCartId == id);
//            if(cartFromDB != null)
//            {
//                cartFromDB.Count += 1;
//                _unitOfWork.Save();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(cartFromDB);
//        }
//        [HttpGet]
//        public IActionResult Minus(int id)
//        {
//            var claims = (ClaimsIdentity)User.Identity;
//            var userID = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            var cartFromDB = _unitOfWork.ShoppingCartRepository.GetBy(x => x.ShoppingCartId == id);
            
//            if (cartFromDB != null)
//            {
//                cartFromDB.Count -= 1;
//                if (cartFromDB.Count < 1)
//                    _unitOfWork.ShoppingCartRepository.Remove(cartFromDB);
//                _unitOfWork.Save();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(cartFromDB);
//        }
//        [HttpGet]
//        public IActionResult Delete(int id)
//        {
//            var CartFromeDB=_unitOfWork.ShoppingCartRepository.GetBy(x=>x.ShoppingCartId==id);
//            if (CartFromeDB != null)
//            {
//                _unitOfWork.ShoppingCartRepository.Remove(CartFromeDB);
//                _unitOfWork.Save();
//            }
//            return RedirectToAction(nameof(Index));
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Create(ShoppingCart item)
//        {
//            if(item.Count==0)
//                return RedirectToAction(nameof(Index), "Home", new { area = SD.CustomerRole });

//            item.Product=_unitOfWork.ProductRepository.GetBy(x=>x.Id==item.ProductId);
//            var claims = (ClaimsIdentity)User.Identity;
//            var userID = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
//            var cartFromDb=_unitOfWork.ShoppingCartRepository.GetBy(x=>x.ProductId==item.ProductId&&x.UserId==userID);
//            if(cartFromDb==null)
//            {
//                _unitOfWork.ShoppingCartRepository.Add(item);
//                _unitOfWork.Save();
//                return RedirectToAction(nameof(Index), "Home", new { area = SD.CustomerRole });
//            }
//            else
//            {
//                cartFromDb.Count=item.Count;

//                _unitOfWork.Save();
//                return RedirectToAction(nameof(Index), "Home", new { area = SD.CustomerRole });
//            }
            
//        }
//        [HttpGet]
//        public IActionResult Summary()
//        {
//            var Claims=(ClaimsIdentity)User.Identity;
//            var UserId = Claims.FindFirst(ClaimTypes.NameIdentifier).Value;
//            var CurrentUser=_unitOfWork.ApplicaionUserRepository.GetBy(x=>x.Id==UserId);

//            var Carts = new ShoppingCartVM
//            {
//                ShoppingCarts=_unitOfWork.ShoppingCartRepository.GetAll(x=>x.UserId==UserId,"Product"),
//                OrderHeader=new OrderHeader()
//            };
//            Carts.OrderHeader.TotalPrice=_unitOfWork.ShoppingCartRepository.GetTotalPrice(Carts.ShoppingCarts);
//            Carts.OrderHeader.Name=CurrentUser.Name;
//            Carts.OrderHeader.Email=CurrentUser.Email;
//            Carts.OrderHeader.PhoneNumber=CurrentUser.Phone;
//            Carts.OrderHeader.City=CurrentUser.City;

//            return View(Carts);
//        }
//        [HttpPost]
//        [ActionName("Summary")]
//        [ValidateAntiForgeryToken]
//        public IActionResult SummaryPost(ShoppingCartVM CartVM)
//        {
//            var Claims = (ClaimsIdentity)User.Identity;
//            var UserId = Claims.FindFirst(ClaimTypes.NameIdentifier).Value;
//            var CurrentUser = _unitOfWork.ApplicaionUserRepository.GetBy(x => x.Id == UserId);

//            CartVM.ShoppingCarts=_unitOfWork.ShoppingCartRepository.GetAll(x=>x.UserId==UserId,"Product");
//            CartVM.OrderHeader.OrderStatus=MyStatus.StatusPending;
//            CartVM.OrderHeader.PaymentStatus=MyStatus.StatusPending;
//            CartVM.OrderHeader.CreatedDate=DateTime.Now;
//            CartVM.OrderHeader.ApplicationUser=CurrentUser;
//            CartVM.OrderHeader.TotalPrice = _unitOfWork.ShoppingCartRepository.GetTotalPrice(CartVM.ShoppingCarts);
//            _unitOfWork.OrderHeaderReposittory.Add(CartVM.OrderHeader);
//            var res= _unitOfWork.Save();

//            foreach(var item in CartVM.ShoppingCarts)
//            {
//                var OrderDetail = new OrderDetail
//                {
//                    ProductId = item.ProductId,
//                    OrderHeaderId = CartVM.OrderHeader.Id,
//                    Count = item.Count,
//                    Price = item.Product.Price
//                };
//                _unitOfWork.OrderDetailReposittory.Add(OrderDetail);
//                _unitOfWork.Save();
//            }
//            var Domain = "https://localhost:44394/";
//            var options = new SessionCreateOptions
//            {
//                LineItems = new List<SessionLineItemOptions>(),
//                Mode = "payment",
//                SuccessUrl = $"{Domain}Customer/OrderConfirmation/{CartVM.OrderHeader.Id}",
//                CancelUrl =  $"{Domain}Customer/Index"
//            };
//            foreach (var item in CartVM.ShoppingCarts)
//            {
//                var sessionLineItem = new SessionLineItemOptions
//                {
//                    PriceData = new SessionLineItemPriceDataOptions
//                    {
//                        UnitAmount = (long)(item.Product.Price * 100),
//                        Currency = "usd",
//                        ProductData = new SessionLineItemPriceDataProductDataOptions
//                        {
//                            Name = item.Product.Name
//                        }
//                    },
//                    Quantity = item.Count
//                };
//                options.LineItems.Add(sessionLineItem);
//            }
//            var service = new SessionService();
//            Session session = service.Create(options);
//            CartVM.OrderHeader.SessionId = session.Id;
//            CartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
//            _unitOfWork.Save();

//            Response.Headers.Add("Location", session.Url);
//            return new StatusCodeResult(303);
//        }
//        [HttpGet]
//        [Route("Customer/OrderConfirmation/{id}")]
//        public IActionResult OrderConfirmation(int id)
//        {

//            var OrderHeader = _unitOfWork.OrderHeaderReposittory.GetBy(x => x.Id == id);
//            var service = new SessionService();
//            Session Session = service.Get(OrderHeader.SessionId);
//            if(Session.PaymentStatus.ToLower()=="paid")
//            {
//                OrderHeader.OrderStatus = MyStatus.StatusApproved;
//                OrderHeader.PaymentStatus = MyStatus.PaymentStatusApproved;
//                OrderHeader.PaymentIntentId=Session.PaymentIntentId;
//                _unitOfWork.Save();
//            }
//            var Carts=_unitOfWork.ShoppingCartRepository.GetAll(x=>x.UserId==OrderHeader.UserId).ToList();
//            _unitOfWork.ShoppingCartRepository.RemoveRange(Carts);
//            _unitOfWork.Save();
//            return View(id);

//        }

//    }
//}
