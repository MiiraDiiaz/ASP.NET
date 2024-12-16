using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.WebHost.Models;
using YamlDotNet.Core;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }
        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        /// <param name="firstName"> Имя</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="email">Почта</param>
        /// <param name="promocod">Промокод</param>
        [HttpPut("{firstName}/{lastName}/{email}/{promocod}")]
        public ActionResult<EmployeeResponse> UpdateEmployee(string firstName, string lastName, string email, int promocod)
        {
            try
            {
                var newEmployee = new Employee()
                {
                    Id = Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"),
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Roles = new List<Role>()
                    {
                        FakeDataFactory.Roles.FirstOrDefault(x => x.Name == "PartnerManager")
                    },
                    AppliedPromocodesCount = promocod
                };
                _employeeRepository.Create(newEmployee);
                return Ok(newEmployee.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
            
        }
        /// <summary>
        /// Изменить сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="promocod"></param>
        /// <returns></returns>
        [HttpPost("{id}/{firstName}/{lastName}/{email}/{promocod}")]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(Guid id, string firstName, string lastName, string email, int promocod)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    return NotFound();

                var updateEmployee = new Employee()
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Roles = new List<Role>()
                    {
                        FakeDataFactory.Roles.FirstOrDefault(x => x.Name == "PartnerManager")
                    },
                    AppliedPromocodesCount = promocod
                };
                _employeeRepository.Update(id, updateEmployee);
                return Ok($"Сотрудник {employee.FullName} успешно изменен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
    
        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <param name="id"> ID</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployee(Guid id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    return NotFound();

                _employeeRepository.Delete(id);
                return Ok($"Сотрудник {employee.FullName} успешно удален");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
    }
}