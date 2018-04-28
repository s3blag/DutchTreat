using DutchTreat.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DutchTreat.Data
{
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext _ctx;
        private readonly ILogger<DutchRepository> _logger;

        public DutchRepository(DutchContext ctx, ILogger<DutchRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                return _ctx.Products
                           .OrderBy(p => p.Title)
                           .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all products: {ex}");
                return null;
            }
           
        }

        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            try
            {
                return _ctx.Products
                           .Where(p => p.Category == category)
                           .ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to get products by category: {ex}");
                return null;
            }
           
        }


        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            try
            {
                if (includeItems)
                {
                    return _ctx.Orders
                               .Include(o => o.Items)
                               .ThenInclude(i => i.Product)
                               .ToList();
                }
                else
                {
                    return _ctx.Orders.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Orders: {ex}");
                return null;
            }
        }

        public IEnumerable<Order> GetAllOrdersByUser(string username, bool includeItems)
        {
            try
            {
                if (includeItems)
                {
                    return _ctx.Orders
                               .Where(o => o.User.UserName == username)
                               .Include(o => o.Items)
                               .ThenInclude(i => i.Product)
                               .ToList();
                }
                else
                {
                    return _ctx.Orders
                               .Where(o => o.User.UserName == username)
                               .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all Orders: {ex}");
                return null;
            }
        }

        public Order GetOrderById(string userName, int id)
        {
            try
            {
                return _ctx.Orders
                           .Include(o => o.Items)
                           .ThenInclude(i => i.Product)
                           .FirstOrDefault(o => o.Id == id && o.User.UserName == userName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get product by Id: {ex}");
                return null;
            }
        }


        public bool SaveAll()
        {
            try
            {
                return _ctx.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save changes: {ex}");
                return false;
            }
        }

        public void AddEntity(object model)
        {
            _ctx.Add(model);
        }
    }
}
