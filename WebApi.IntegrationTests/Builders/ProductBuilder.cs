using System.Collections.Generic;
using FizzWare.NBuilder;
using WebApiTemplate.Entities;

namespace WebApiTemplate.IntegrationTests.Builders
{
    public class ProductBuilder
    {
        public static Product Single()
        {
            return Builder<Product>.CreateNew().With(x => x.Id = 0).Build();
        }

        public static IList<Product> ListOf(int i)
        {
            return Builder<Product>.CreateListOfSize(i).All().With(x => x.Id = 0).Build();
        }  
    }
}