using System;
using System.Threading.Tasks;
using CoronaTest.Web.Commands;
using CoronaTest.Web.Persistence;
using CoronaTest.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoronaTest.Web.Controllers
{
    public class CoronaTestController : Controller
    {
        private readonly IMediator _mediator;

        public CoronaTestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/CoronaTest/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = new GetCoronaTestQuery()
            {
                Id = id
            };

            var result = await _mediator.Send(request);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("/CoronaTest")]
        public async Task<IActionResult> Schedule([FromBody] ScheduleTestRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new ScheduleTestAppointmentCommand()
            {
                Location = request.Location,
                ScheduledOn = request.ScheduledOn,
                TestSubjectIdentificatieNummer = request.TestSubjectIdentificatieNummer,
                TestSubjectName = request.TestSubjectName
            };

            var result = await _mediator.Send(command);

            return MapToActionResult(result);

        }

        [HttpPost("/CoronaTest/{id}/Schedule")]
        public async Task<IActionResult> Schedule([FromRoute] Guid id, [FromBody] DateTimeOffset on)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new RecordTestAdministeredCommand()
            {
                Id = id,
                AdministeredOn = on
            };

            var result = await _mediator.Send(command);

            return MapToActionResult(result);

        }

        [HttpPost("/CoronaTest/{id}/Result")]
        public async Task<IActionResult> Result([FromRoute] Guid id, [FromBody] TestResult testResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new RecordTestResultCommand()
            {
                Id = id,
                TestResult = testResult
            };

            var result = await _mediator.Send(command);

            return MapToActionResult(result);

        }

        private IActionResult MapToActionResult(CommandResponse result)
        {
            if (result is SuccessResponse sr)
            {
                return Ok(sr);
            }

            if (result is NotFoundResponse)
            {
                return NotFound();
            }

            if (result is FailureResponse err)
            {
                return UnprocessableEntity(err.Reason);
            }

            throw new InvalidOperationException("Unknown result type: " + result);
        }
    }
}