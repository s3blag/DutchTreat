using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<StoreUser> _userManager;

        public OrdersController(IDutchRepository repository, ILogger<OrdersController> logger, IMapper mapper, UserManager<StoreUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                var results = _repository.GetAllOrdersByUser(User.Identity.Name,  includeItems);
                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Failed to get orders");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = _repository.GetOrderById(User.Identity.Name, id);
                if (order == null)
                    return NotFound();

                return Ok(_mapper.Map<Order, OrderViewModel>(order));            
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get order by Id: {ex}");
                return BadRequest("Failed to get order by Id");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);

                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    newOrder.User = currentUser;

                    _repository.AddEntity(newOrder);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/orders/{newOrder.Id}", _mapper.Map<Order,OrderViewModel>(newOrder));
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new order: {ex}");
            }

            return BadRequest("Failed to save new order");
        }
    }
}

