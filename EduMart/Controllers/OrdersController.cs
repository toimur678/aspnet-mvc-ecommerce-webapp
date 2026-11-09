using EduMart.Data.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduMart.Data.Services;
using EduMart.Data.ViewModels;
using System.Security.Claims;

namespace EduMart.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly ISkillsService _skillService;
        private readonly IOrdersService _ordersService;

        public OrdersController(ISkillsService skillService ,ShoppingCart shoppingCart, IOrdersService ordersService)
        {
            _skillService = skillService;
            _shoppingCart = shoppingCart;
            _ordersService = ordersService;
        }

        public async Task<IActionResult> Index()
         {
             string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); ;
             string userRole = User.FindFirstValue(ClaimTypes.Role);

             var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
             return View(orders);
         }

        public IActionResult ShoppingCart()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(response);
        }

        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await _skillService.GetSkillByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.AddItemToCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }
        
        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await _skillService.GetSkillByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        // POST with an antiforgery token, so another site cannot place an order for a signed-in user.
        // The PayPal payment itself is not verified on the server (see README).
        [HttpPost]
        public async Task<IActionResult> CompleteOrder()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            if (items.Count == 0) return RedirectToAction(nameof(ShoppingCart));
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            await _ordersService.StoreOrderAsync(items, userId, userEmailAddress );
            await _shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }

        public IActionResult Payment()
        {
            var totalAmount = _shoppingCart.GetShoppingCartTotal();
            ViewData["TotalAmount"] = totalAmount;
            return View();
        }
    }
}
