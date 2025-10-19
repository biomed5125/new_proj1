//using HMS.Api.Features.Appointment.Models.Dtos;
//using HMS.Api.Features.Appointment.Services;
//using HMS.SharedKernel.Results;
//using Microsoft.AspNetCore.Mvc;

//namespace HMS.Api.Features.Appointment.Controllers;

//[ApiController]
//[Route("api/appointments")]
//public sealed class AppointmentController : ControllerBase
//{
//    private readonly IAppointmentService _svc;
//    public AppointmentController(IAppointmentService svc) => _svc = svc;

//    [HttpPost]
//    public async Task<ActionResult<Result<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto, CancellationToken ct)
//        => Ok(await _svc.CreateAsync(dto, User?.Identity?.Name, ct));

//    [HttpPost("{id:long}/reschedule")]
//    public async Task<ActionResult<Result>> Reschedule(long id, [FromBody] RescheduleAppointmentDto dto, CancellationToken ct)
//        => Ok(await _svc.RescheduleAsync(id, dto, User?.Identity?.Name, ct));

//    [HttpPost("{id:long}/check-in")]
//    public async Task<ActionResult<Result>> CheckIn(long id, [FromBody] CheckInAppointmentDto dto, CancellationToken ct)
//        => Ok(await _svc.CheckInAsync(id, dto.WhenUtc, User?.Identity?.Name, ct));

//    [HttpPost("{id:long}/complete")]
//    public async Task<ActionResult<Result>> Complete(long id, [FromBody] CompleteAppointmentDto dto, CancellationToken ct)
//        => Ok(await _svc.CompleteAsync(id, dto.WhenUtc, User?.Identity?.Name, ct));

//    [HttpPost("{id:long}/cancel")]
//    public async Task<ActionResult<Result>> Cancel(long id, [FromBody] CancelAppointmentDto dto, CancellationToken ct)
//        => Ok(await _svc.CancelAsync(id, dto.Reason, User?.Identity?.Name, ct));
//}


////using HMS.Api.Features.Appointment.Models.Dtos;
////using HMS.Api.Features.Appointment.Services;
////using Microsoft.AspNetCore.Mvc;

////namespace HMS.Api.Features.Appointment.Controllers;

////[ApiController]
////[Route("api/appointments")]
////public class AppointmentController : ControllerBase
////{
////    private readonly IAppointmentService _service;
////    public AppointmentController(IAppointmentService service) => _service = service;

////    // Filter by optional query params: fromUtc, toUtc, patientId, doctorId
////    [HttpGet]
////    public async Task<IActionResult> List([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc,
////                                          [FromQuery] long? patientId, [FromQuery] long? doctorId,
////                                          CancellationToken ct)
////        => Ok((await _service.ListAsync(fromUtc, toUtc, patientId, doctorId, ct)).Value);

////    [HttpGet("{id:long}")]
////    public async Task<IActionResult> Get(long id, CancellationToken ct)
////    {
////        var r = await _service.GetAsync(id, ct);
////        return r.Succeeded ? Ok(r.Value) : NotFound(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpPost]
////    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto, CancellationToken ct)
////    {
////        var r = await _service.CreateAsync(dto, User?.Identity?.Name, ct);
////        return r.Succeeded
////            ? CreatedAtAction(nameof(Get), new { id = r.Value!.AppointmentId }, r.Value)
////            : BadRequest(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpPut("{id:long}")]
////    public async Task<IActionResult> Update(long id, [FromBody] UpdateAppointmentDto dto, CancellationToken ct)
////    {
////        if (id != dto.AppointmentId) return BadRequest(new { error = "Route id and payload id mismatch." });
////        var r = await _service.UpdateAsync(dto, User?.Identity?.Name, ct);
////        return r.Succeeded ? Ok(r.Value) : BadRequest(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpDelete("{id:long}")]
////    public async Task<IActionResult> Delete(long id, CancellationToken ct)
////    {
////        var r = await _service.DeleteAsync(id, User?.Identity?.Name, ct);
////        return r.Succeeded ? NoContent() : NotFound(new { error = string.Join("; ", r.Errors) });
////    }

////    // AppointmentController.cs
////    [HttpPost("{id:long}/check-in")]
////    public async Task<IActionResult> CheckIn(long id, CancellationToken ct)
////    {
////        var r = await _service.CheckInAsync(id, User?.Identity?.Name, ct);
////        return r.Succeeded ? Ok(r.Value) : BadRequest(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpPost("{id:long}/complete")]
////    public async Task<IActionResult> Complete(long id, CancellationToken ct)
////    {
////        var r = await _service.CompleteAsync(id, User?.Identity?.Name, ct);
////        return r.Succeeded ? Ok(r.Value) : BadRequest(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpPost("{id:long}/cancel")]
////    public async Task<IActionResult> Cancel(long id, CancellationToken ct)
////    {
////        var r = await _service.CancelAsync(id, User?.Identity?.Name, ct);
////        return r.Succeeded ? Ok(r.Value) : BadRequest(new { error = string.Join("; ", r.Errors) });
////    }

////    // AppointmentController.cs
////    [HttpGet("today")]
////    public async Task<IActionResult> Today([FromQuery] long doctorId, CancellationToken ct)
////    {
////        var start = DateTime.UtcNow.Date;
////        var end = start.AddDays(1);
////        var r = await _service.ListAsync(start, end, patientId: null, doctorId: doctorId, ct);
////        return Ok(r.Value);
////    }
////}
