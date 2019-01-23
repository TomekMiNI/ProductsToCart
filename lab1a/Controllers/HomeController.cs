using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using lab1a.Models;
using WWW;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace lab1a.Controllers
{
    public class HomeController : Controller
    {
        private List<CartProduct> Cart = new List<CartProduct>();
        public IActionResult Index()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\"/>");
            sb.AppendLine("<title></title>");
            sb.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/style.css\"/>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            //sb.AppendLine("<div id=\"order\">");
            //sb.AppendLine("<h2>Order Summary:</h2>");
            //sb.AppendLine("<table><tr><th>Product</th><th>Price per product</th><th>Quantity</th></tr>");
            //sb.AppendLine("</table>");
            sb.AppendLine("<div id=\"products\">");
            sb.AppendLine("<h2>Products:</h2>");
            for(int i=0;i<Warehouse.PRODUCTS.Length;i++)
            {
                var el = Warehouse.PRODUCTS[i];
                sb.AppendLine("<div class=\"product\">");
                sb.AppendLine($"<form method=\"get\" action=\"/home/addProduct?nr=10\">");
                sb.AppendLine($"<input type=\"hidden\" name=\"prindex\" value=\"{i}\"/>");
                Request.QueryString.Add("number", i.ToString());
             
                sb.AppendLine($"<p class=\"name\">Product name {el.Name}</p>");
                sb.AppendLine($"<p class=\"price\"><strong>Price: </strong>{el.Price}zł</p>");
                sb.AppendLine($"<p class=\"name\">{el.Name}</p>");
                sb.AppendLine("<span class=\"quantity\"><strong>Quantity: </strong></span><input type=\"number\" name=\"quantity\" value=\"1\" min=\"1\" />");
                sb.AppendLine("<input type=\"submit\" value=\"Add to cart\"  />");
                sb.AppendLine("</form>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div>");
            sb.AppendLine("</table>");
            sb.AppendLine("<div id=\"cart\">");
            sb.AppendLine("<h2>Cart:</h2>");
            deserializeCart();
            if (Cart.Count == 0)
                sb.AppendLine("<p>Cart is empty!</p>");
            else
                ShowCart(sb);
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return Content(sb.ToString(), "text/html");
        }
        public IActionResult deleteProduct()
        {
            int indexToRem;
            deserializeCart();
            if (int.TryParse(Request.Query["rmindex"], out indexToRem))
                Cart.RemoveAt(indexToRem);
            serializeCart();
            return Redirect("/home/index");
        }
        public IActionResult addProduct()
        {
            int indexToAdd;
            deserializeCart();
            if (int.TryParse(Request.Query["prindex"], out indexToAdd))
            {
                bool added = false;
                int quant;
                int.TryParse(Request.Query["quantity"], out quant);
                for (int i = 0; i < Cart.Count; i++)
                    if (Cart[i].Id == indexToAdd)
                    {
                        Cart[i].Quantity += quant;
                        added = true;
                    }
                if(!added)
                    Cart.Add(new CartProduct { Id = indexToAdd, Name = Warehouse.PRODUCTS[indexToAdd].Name, Price = Warehouse.PRODUCTS[indexToAdd].Price,  Quantity=quant});

            }
            serializeCart();
            return Redirect($"/home/index");
        }
        private void deserializeCart()
        {
            byte[] data = HttpContext.Session.Get("activecart");
            if (data != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    Cart = (List<CartProduct>)bf.Deserialize(ms);
                }
            }
        }
        private void serializeCart()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, Cart);
                HttpContext.Session.Set("activecart", ms.ToArray());
            }
        }
        public IActionResult increase()
        {
            deserializeCart();
            int id;
            int.TryParse(Request.Query["id"], out id);
            Cart[id].Quantity++;
            serializeCart();
            return Redirect("/home");
        }
        public IActionResult decrease()
        {
            deserializeCart();
            int id;
            int.TryParse(Request.Query["id"], out id);
            Cart[id].Quantity--;
            if (Cart[id].Quantity == 0)
                return Redirect($"/home/deleteProduct?rmindex={id}");
            else
            {
                serializeCart();
                return Redirect("/home");
            }
        }
        public IActionResult clear()
        {
            serializeCart();
            return Redirect("/home/index");
        }
        public IActionResult checkout()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\"/>");
            sb.AppendLine("<title></title>");
            sb.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\"/>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            deserializeCart();
            sb.AppendLine("<div id=\"order\">");
            sb.AppendLine("<h2>Order Summary:</h2>");
            sb.AppendLine("<table border=\"1\">");
            sb.AppendLine("<tr><th>Product</th><th>Price per product</th><th>Quantity</th></tr>");
            double suma = 0;
            foreach(var el in Cart)
            {
                sb.AppendLine($"<tr><td>{el.Name}</td><td>{el.Price}zł</td><td>{el.Quantity}</td></tr>");
                suma += el.Price*el.Quantity;
            }
            sb.AppendLine("</table>");
            sb.AppendLine($"<p><strong>Total price: {suma} zł</p>");
            sb.AppendLine("<a href=\"/home/clear\"><input type=\"button\" value=\"Create new order\"></a>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return Content(sb.ToString(), "text/html");
        }
        public void ShowCart(StringBuilder sb)
        {
            for(int i=0;i<Cart.Count;i++)
            {
                var el = Cart[i];
                sb.AppendLine("<div class=\"product\">");
                sb.AppendLine($"<p class=\"name\">Product name {el.Name}</p>");
                sb.AppendLine($"<p class=\"price\"><strong>Price per product: {el.Price} zł</p>");
                sb.AppendLine("<p class=\"quantity\">");
                sb.AppendLine($"<strong>Quantity: {el.Quantity}");
                sb.AppendLine($"<a href=\"/home/increase?id={i}\"><input type=\"button\" value=\" + \" /></a>");
                sb.AppendLine($"<a href=\"/home/decrease?id={i}\"><input type=\"button\" value=\" - \" /></a></p>");
                sb.AppendLine("<form method=\"get\" action=\"/home/deleteProduct\">");
                sb.AppendLine($"<input type=\"hidden\" name=\"rmindex\" value=\"{i}\"/>");
                sb.AppendLine("<input type=\"submit\" value=\"Remove from cart\" />");
                sb.AppendLine("</form>");
                sb.AppendLine("</div>");
                sb.AppendLine("<div class=\"controlButtons\">");
                sb.AppendLine("<a href=\"/home/clear\"><input type=\"button\" value=\"ClearCart\" /></a>");
                sb.AppendLine("<a href=\"/home/checkout\"><input type=\"button\" value=\"Checkout\" /></a>");
                sb.AppendLine("</div>");
            }
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
