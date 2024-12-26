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
        /// <response code="200">Запрос выполнен успешно</response>
        /// <returns>Список сотрудников</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return Ok(employeesModelList);
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <response code="200">Запрос выполнен успешно</response>
        /// <response code="404">Сотрудник с таким id не найден</response>
        /// <returns>Сотрудник</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            return Ok(employeeModel);
        }

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        /// <param name="employee">Модель сотрудника</param>
        /// <response code="201">Сотрудник успешно создан</response>
        /// <response code="400">Данные запроса невалидны</response>
        /// <returns>Созданный сотрудник</returns>
        [HttpPut]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<EmployeeResponse> CreateEmployee(Employee employee)
        {

            var newEmployee = new Employee()
            {
                Id = Guid.Parse("b86f2096-237a-4059-8329-1bbcea72769b"),
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Roles = new List<Role>()
                    {
                        FakeDataFactory.Roles.FirstOrDefault(x => x.Name == "PartnerManager")
                    },
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
            _employeeRepository.Add(newEmployee);
            return CreatedAtAction(nameof(CreateEmployee), newEmployee);
        }

        /// <summary>
        /// Изменить сотрудника
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <param name="employee">Модель сотрудника</param>
        /// <response code="200">Запрос выполнен успешно</response>
        /// <response code="400">Данные запроса невалидны</response>
        /// <response code="404">Сотрудник не найден</response>
        /// <returns>Данные сотрудника изменены</returns>
        [HttpPost("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(Guid id, Employee employee)
        {

            var foundEmployee = await _employeeRepository.GetByIdAsync(id);

            if (foundEmployee == null)
                return NotFound();

            var updateEmployee = new Employee()
            {
                Id = id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Roles = new List<Role>()
                    {
                        FakeDataFactory.Roles.FirstOrDefault(x => x.Name == "PartnerManager")
                    },
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
            _employeeRepository.Update(id, updateEmployee);
            return Ok(updateEmployee);
        }

        /// <summary>
        /// Удаление сотрудника
        /// </summary>
        /// <param name="id"> ID</param>
        /// <response code="204">Запрос выполнен успешно</response>
        /// <response code="400">Данные запроса невалидны</response>
        /// <response code="404">Сотрудник не найден</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeResponse>> DeleteEmployee(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            if (_employeeRepository.Delete(id)) return NoContent();
            return NotFound();
        }
    }
}
