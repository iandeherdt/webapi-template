using System.Collections.Generic;
using FizzWare.NBuilder;
using WebApiTemplate.Models;

namespace WebApiTemplate.IntegrationTests.Builders
{
    public class ProductDtoBuilder
    {
        public static ProductDto Single()
        {
            return Builder<ProductDto>.CreateNew().With(x => x.Id = 0).Build();
        }

        public static IList<ProductDto> ListOf(int i)
        {
            return Builder<ProductDto>.CreateListOfSize(i).All().With(x => x.Id = 0).Build();
        }  
    }
}
