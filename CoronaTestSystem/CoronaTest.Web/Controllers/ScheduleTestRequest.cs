using System;
using System.ComponentModel.DataAnnotations;

namespace CoronaTest.Web.Controllers
{
    public class ScheduleTestRequest
    {
        [Required] public string Location { get; set; }

        [Required] public DateTimeOffset ScheduledOn { get; set; }

        [Required] public string TestSubjectIdentificatieNummer { get; set; }

        [Required] public string TestSubjectName { get; set; }
    }
}