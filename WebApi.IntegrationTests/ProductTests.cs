using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;
using WebApiTemplate.Entities;
using WebApiTemplate.IntegrationTests.Builders;
using WebApiTemplate.Models;
using WebApiTemplate.ResourceAccess.Uow;

namespace WebApiTemplate.IntegrationTests
{
    public class ActieServiceTests
    {
        [Description("Demo for usage webapi template")]
        public class When_getting_all_prodcuts : ServiceTest
        {
            private HttpResponseMessage _response;
            private IEnumerable<ProductDto> _result;
            private IEnumerable<Product> _given;
            protected override void Given()
            {
                _given = ProductBuilder.ListOf(12);
                using (var uow = UnitOfWork.Start())
                {
                    foreach (var actie in _given)
                    {
                        Repository.Add(actie);
                    }
                    uow.SaveChanges();
                }
            }

            protected override void When()
            {
                var request = "/api/product";
                _response = CreateGetRequest(request).Result;
                var json = _response.Content.ReadAsStringAsync().Result;
                _result = DeserializeJsonWithRootObject<IEnumerable<ProductDto>>(json);
            }

            [Test]
            public void Then_the_response_should_be_OK()
            {
                _response.IsSuccessStatusCode.Should().BeTrue();
            }

            [Test]
            public void Then_the_count_should_be_12()
            {
                _result.Count().ShouldBeEquivalentTo(12);
            }


            [Test]
            public void Then_the_name_should_be_filled_in()
            {
                foreach (var productDto in _result)
                {
                    productDto.Name.Should().StartWith("Name");
                }
            }
        }
    }
}