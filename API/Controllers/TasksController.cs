using Application.DTOs.Base;
using Application.DTOs.Task;
using Application.Interface.Service;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TasksController(ITaskService taskService) : BaseController
    {
        [ApiVersion(1)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {
            var task = await taskService.GetAllTasks();
            return Ok(task);
        }

        [ApiVersion(1)]
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var task = await taskService.GetTaskById(id);
            if (task.Data is not null) return Ok(task);
            return NotFound("Task does not exist");
        }

        [ApiVersion(1)]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTaskAsync([FromServices] IValidator<TaskCreateRequest> validator,
                                                   [FromBody] TaskCreateRequest dto)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (validationResult.IsValid is false)
            {
                var errors = validationResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await taskService.CreateTaskAsync(dto);
            if (result.IsSucceed is true) return Created(string.Empty, result);
            return BadRequest();
        }

        [ApiVersion(1)]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateTaskAsync([FromServices] IValidator<TaskUpdateRequest> validator,
                                                         [FromBody] TaskUpdateRequest dto)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (validationResult.IsValid is false)
            {
                var errors = validationResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await taskService.UpdateTaskAsync(dto);
            if (result.IsSucceed is true) return Created(string.Empty, result);
            return BadRequest();
        }

        [ApiVersion(1)]
        [HttpPut("status")]
        [Authorize]
        public async Task<IActionResult> UpdateTaskStatusAsync([FromServices] IValidator<TaskChangeStatusRequest> validator,
                                                               [FromBody] TaskChangeStatusRequest dto)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (validationResult.IsValid is false)
            {
                var errors = validationResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await taskService.UpdateTaskStatusAsync(dto);
            if (result.IsSucceed is true) return Created(string.Empty, result);
            return BadRequest();
        }

        [ApiVersion(1)]
        [HttpPut("priority")]
        [Authorize]
        public async Task<IActionResult> UpdateTaskPriorityAsync([FromServices] IValidator<TaskChangePriorityRequest> validator,
                                                               [FromBody] TaskChangePriorityRequest dto)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (validationResult.IsValid is false)
            {
                var errors = validationResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await taskService.UpdateTaskPriorityAsync(dto);
            if (result.IsSucceed is true) return Created(string.Empty, result);
            return BadRequest();
        }

        [ApiVersion(1)]
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> RemoveTaskAsync([FromServices] IValidator<TaskRemoveRequest> validator,
                                                         [FromRoute] int id)
        {
            var dto = new TaskRemoveRequest { Id = id };
            var validationResult = await validator.ValidateAsync(dto);
            if (validationResult.IsValid is false)
            {
                var errors = validationResult.Errors.ConvertAll(err => new ResponseResult<string>
                {
                    Data = err.PropertyName,
                    Message = err.ErrorMessage,
                    IsSucceed = false
                });
                return BadRequest(errors);
            }
            var result = await taskService.RemoveTaskAsync(dto);
            if (result.IsSucceed is true) return Created(string.Empty, result);
            return BadRequest();
        }
    }
}
