using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using WebApiTemplate.Entities;
using WebApiTemplate.IntegrationTests.Builders;
using WebApiTemplate.Models;
using WebApiTemplate.ResourceAccess.Uow;
using Xunit;

namespace WebApiTemplate.IntegrationTests
{
    public class ActieServiceTests
    {
        public class When_getting_all_prodcuts : IClassFixture<ServiceTest>
        {
            private HttpResponseMessage _response;
            private IEnumerable<ProductDto> _result;
            private IEnumerable<Product> _given;
            ServiceTest fixture;
            public When_getting_all_prodcuts(ServiceTest serviceTest)
            {
                this.fixture = serviceTest;
                _given = ProductBuilder.ListOf(12);
                using (var uow = UnitOfWork.Start())
                {
                    foreach (var product in _given)
                    {
                        this.fixture.Repository.Add(product);
                    }
                    uow.SaveChanges();
                }
                using (UnitOfWork.Start())
                {
                    var result = this.fixture.Repository.GetAll<Product>().ToList();
                }
                
                var request = "/product";
                _response = this.fixture.CreateGetRequest(request).Result;
                var json = _response.Content.ReadAsStringAsync().Result;
                _result = this.fixture.DeserializeJson<IEnumerable<ProductDto>>(json);
            }

            [Fact]
            public void Then_the_response_should_be_OK()
            {
                _response.IsSuccessStatusCode.Should().BeTrue();
            }

            [Fact]
            public void Then_the_count_should_be_12()
            {
                _result.Count().ShouldBeEquivalentTo(12);
            }


            [Fact]
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