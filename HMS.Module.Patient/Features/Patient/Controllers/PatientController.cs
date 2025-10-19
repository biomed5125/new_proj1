//using HMS.Module.Patient.Features.Patient.Models.Dtos;
//using HMS.Module.Patient.Features.Patient.Queries;
//using HMS.Module.Patient.Features.Patient.Repositories;
//using HMS.Module.Patient.Features.Patient.Services;
//using HMS.SharedKernel.Results;
//using Microsoft.AspNetCore.Mvc;

//namespace HMS.Api.Features.Patient.Controllers;

//[ApiController]
//[Route("api/patients")]
//public class PatientController : ControllerBase
//{
//    private readonly IPatientReadRepo _read;
//    private readonly IPatientService _svc;
//    public PatientController(IPatientReadRepo read, IPatientService svc)
//    { _read = read; _svc = svc; }

//    [HttpGet("{id:long}")]
//    public async Task<ActionResult<Result<PatientDto>>> Get(long id, CancellationToken ct)
//        => Ok(Result<PatientDto>.Success((await _read.GetAsync(id, ct))!));

//    [HttpGet]
//    public async Task<ActionResult<Result<IReadOnlyList<PatientDto>>>> List(
//        [FromQuery] PatientQuery q, CancellationToken ct)
//        => Ok(Result<IReadOnlyList<PatientDto>>.Success(await _read.ListAsync(q, ct)));

//    [HttpPost]
//    public async Task<ActionResult<Result<PatientDto>>> Create([FromBody] CreatePatientDto dto, CancellationToken ct)
//        => Ok(await _svc.CreateAsync(dto, User?.Identity?.Name, ct));

//    [HttpPut("{id:long}")]
//    public async Task<ActionResult<Result<PatientDto>>> Update(long id, [FromBody] UpdatePatientDto dto, CancellationToken ct)
//        => Ok(await _svc.UpdateAsync(id, dto, User?.Identity?.Name, ct));

//    [HttpDelete("{id:long}")]
//    public async Task<ActionResult<Result>> Delete(long id, CancellationToken ct)
//        => Ok(await _svc.DeleteAsync(id, User?.Identity?.Name, ct));
//}



////using HMS.Api.Features.Patient.Models.Dtos;
////using HMS.Api.Features.Patient.Services;
////using Microsoft.AspNetCore.Mvc;

////namespace HMS.Api.Features.Patient.Controllers;

////[ApiController]
////[Route("api/patients")]
////public class PatientController : ControllerBase
////{
////    private readonly IPatientService _service;
////    public PatientController(IPatientService service) => _service = service;

////    // src/HMS.Api/Features/Patient/Controllers/PatientController.cs
////    [HttpGet]
////    public async Task<IActionResult> List(CancellationToken ct)
////        => Ok((await _service.ListAsync(ct)).Value);

////    [HttpGet("{id:long}")]
////    public async Task<IActionResult> Get(long id, CancellationToken ct)
////    {
////        var r = await _service.GetAsync(id, ct);
////        return r.Succeeded ? Ok(r.Value) : NotFound(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpPost]
////    public async Task<IActionResult> Create([FromBody] CreatePatientDto dto, CancellationToken ct)
////    {
////        // With [ApiController] + FluentValidation v8, invalid dto → 400 automatically
////        var r = await _service.CreateAsync(dto, User?.Identity?.Name, ct);
////        return r.Succeeded
////            ? CreatedAtAction(nameof(Get), new { id = r.Value!.PatientId }, r.Value)
////            : BadRequest(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpPut("{id:long}")]
////    public async Task<IActionResult> Update(long id, [FromBody] UpdatePatientDto dto, CancellationToken ct)
////    {
////        if (id != dto.PatientId) return BadRequest(new { error = "Route id and payload id mismatch." });
////        var r = await _service.UpdateAsync(dto, User?.Identity?.Name, ct);
////        return r.Succeeded ? Ok(r.Value) : BadRequest(new { error = string.Join("; ", r.Errors) });
////    }

////    [HttpDelete("{id:long}")]
////    public async Task<IActionResult> Delete(long id, CancellationToken ct)
////    {
////        var r = await _service.DeleteAsync(id, User?.Identity?.Name, ct);
////        return r.Succeeded ? NoContent() : NotFound(new { error = string.Join("; ", r.Errors) });
////    }
////}
