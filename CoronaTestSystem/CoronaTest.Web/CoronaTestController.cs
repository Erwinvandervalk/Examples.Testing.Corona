using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using CoronaTest.Web.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoronaTest.Web
{
    public class CoronaTestController : Controller
    {
        private readonly IMediator _mediator;

        public CoronaTestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/CoronaTest/{id}")]
        public string Get(Guid id)
        {
            return "hi";
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


    public class ScheduleTestRequest
    {
        [Required]
        public string Location { get; set; }

        [Required]
        public DateTimeOffset ScheduledOn { get; set; }

        [Required]
        public string TestSubjectIdentificatieNummer { get; set; }

        [Required]
        public string TestSubjectName { get; set; }
    }


    public class CoronaTestCommandHandlers : 
        IRequestHandler<ScheduleTestAppointmentCommand, CommandResponse>,
            IRequestHandler<RecordTestAdministeredCommand, CommandResponse>,
            IRequestHandler<RecordTestResultCommand, CommandResponse>
    {
        private readonly CoronaTestRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CoronaTestCommandHandlers(CoronaTestRepository repository, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CommandResponse> Handle(ScheduleTestAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = CoronaTestEntity.ScheduleNewAppointment(
                request.Location, 
                request.ScheduledOn,
                request.TestSubjectIdentificatieNummer, 
                request.TestSubjectName,
                _dateTimeProvider.GetNow());

            await _repository.SaveAsync(cancellationToken);

            return new SuccessResponse()
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }

        public async Task<CommandResponse> Handle(RecordTestAdministeredCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _repository.Get(request.Id, cancellationToken);

            if (appointment == null)
                return new NotFoundResponse();

            if (appointment.Status != TestStatus.Scheduled && appointment.Status != TestStatus.Administered)
            {
                return new FailureResponse(){Reason =
                    $"Can't record that test is administered. Status is: '{appointment.Status}'"
                };
            }

            appointment.AdministeredOn = request.AdministeredOn;
            appointment.Status = TestStatus.Administered;
            appointment.Version++;

            await _repository.SaveAsync(cancellationToken);

            return new SuccessResponse()
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }

        public async Task<CommandResponse> Handle(RecordTestResultCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _repository.Get(request.Id, cancellationToken);

            if (appointment == null)
                return new NotFoundResponse();

            if (appointment.Status != TestStatus.Administered && appointment.Status != TestStatus.ResultKnown)
            {
                return new FailureResponse()
                {
                    Reason =
                        $"Can't record that test is administered. Status is: '{appointment.Status}'"
                };
            }

            appointment.Status = TestStatus.ResultKnown;
            appointment.TestResult = request.TestResult;
            appointment.ResultsKnown = _dateTimeProvider.GetNow();
            appointment.Version++;

            await _repository.SaveAsync(cancellationToken);

            return new SuccessResponse()
            {
                Id = appointment.Id,
                Version = appointment.Version
            };
        }
    }

    public interface IDateTimeProvider
    {
        public DateTimeOffset GetNow();
    }


    public class CoronaTestRepository
    {
        private readonly CoronaDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CoronaTestRepository(CoronaDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CoronaTestEntity> Get(Guid id, CancellationToken ct = default)
        {
            var item = await _context.CoronaTests.FindAsync(id);
            return item;
        }

        public async Task SaveAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }

    public class CoronaDbContext : DbContext
    {
        public DbSet<CoronaTestEntity> CoronaTests { get; set; }

        public CoronaDbContext(DbContextOptions options) : base(options)
        {
        }
    }
    

    public class CoronaTestEntity
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastChanged { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public string TestSubjectIdentificatieNummer { get; set; }
        public string TestSubjectName { get; set; }
        public DateTimeOffset? AdministeredOn { get; set; }

        public TestStatus Status { get; set; }

        public DateTimeOffset? ResultsKnown { get; set; }
        public TestResult TestResult { get; set; }
        public long Version { get; set; }

        public static CoronaTestEntity ScheduleNewAppointment(string location, DateTimeOffset scheduledOn, string testSubjectIdentificatieNummer, string testSubjectName, DateTimeOffset now)
        {
            var entity = new CoronaTestEntity()
            {
                Id = Guid.NewGuid(),
                ScheduledOn = scheduledOn,
                TestSubjectIdentificatieNummer = testSubjectIdentificatieNummer,
                TestSubjectName = testSubjectName,
                Version = 1,
                CreatedOn = now,
                LastChanged = now
            };
            return entity;
        }
    }

    public enum TestStatus
    {
        Scheduled = 0,
        Administered = 1,
        Cancelled = 2,
        ResultKnown = 3
        
    }

    public enum TestResult
    {
        Unknown = 0,
        Positive = 1,
        Negative = 2
    }

}
