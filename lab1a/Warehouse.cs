using System;

namespace WWW
{
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    [Serializable]
    public class CartProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
    public class Warehouse
    {
        public static readonly Product[] PRODUCTS = new Product[]
        {
            new Product() { Id = 1, Name = "Laptop", Price = 2999 },
            new Product() { Id = 2, Name = "Powerbank", Price = 69 },
            new Product() { Id = 3, Name = "Mobile Phone", Price = 1599 },
            new Product() { Id = 4, Name = "Sport Camera", Price = 2399 },
            new Product() { Id = 5, Name = "Speakers", Price = 199 },
            new Product() { Id = 6, Name = "Printer", Price = 349 },
            new Product() { Id = 7, Name = "Keyboard", Price = 149 },
            new Product() { Id = 8, Name = "Mouse", Price = 49 },
            new Product() { Id = 9, Name = "Headphones", Price = 129 },
            new Product() { Id = 10, Name = "Drone", Price = 3899 },
            new Product() { Id = 11, Name = "Digital camera", Price = 999 }
        };
    }
}