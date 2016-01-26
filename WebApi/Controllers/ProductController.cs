using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using WebApiTemplate.Entities;
using WebApiTemplate.Models;
using WebApiTemplate.ResourceAccess.Repositories;
using WebApiTemplate.ResourceAccess.Uow;

namespace WebApiTemplate.Controllers
{
    public class ProductController : ApiController
    {
        private readonly IRepository _repository;

        public ProductController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Product
        public IEnumerable<ProductDto> Get()
        {
            using (UnitOfWork.Start())
            {
                var products = _repository.GetAll<Product>().ToList();
                var productdtos = products.Select(Mapper.Map<ProductDto>);
                return productdtos;
            }
        }

        // GET: api/Product/5
        public ProductDto Get(int id)
        {
            using (UnitOfWork.Start())
            {
                var product = _repository.Get<Product>(id);
                return Mapper.Map<ProductDto>(product);
            }
        }

        // POST: api/Product
        public void Post([FromBody]ProductDto value)
        {
        }

        // PUT: api/Product/5
        public void Put(int id, [FromBody]ProductDto value)
        {
        }

        // DELETE: api/Product/5
        public void Delete(int id)
        {
        }
    }
}
