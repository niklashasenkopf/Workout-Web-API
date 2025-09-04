using System.Security.Claims;
using AutoMapper;
using C_Sharp_Web_API.Features.WorkoutExercises.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.WorkoutExercises;

[Route("workout-api/workouts/{workoutId:int}/exercises")]
[Authorize]
[ApiController]
public class WorkoutExerciseController(
    IWorkoutRepository workoutRepository,
    IMapper mapper
    ) : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository
        = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IMapper _mapper
        = mapper ?? throw new ArgumentNullException(nameof(mapper));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutExerciseDto>>> GetAll(int workoutId)
    {
        var workoutExerciseEntities =
            await _workoutRepository.GetAllExercisesAsync(GetCurrentUserId(), workoutId);

        var mappedExerciseEntities = _mapper.Map<WorkoutExerciseDto>(workoutExerciseEntities);

        return Ok(mappedExerciseEntities);
    }

    [HttpGet("{workoutExerciseId:int}", Name = "GetWorkoutExercise")]
    public async Task<IActionResult> Get(
        int workoutId, 
        int workoutExerciseId)
    {
        var workoutExerciseEntity =
            await _workoutRepository.GetExerciseAsync(GetCurrentUserId(), workoutId, workoutExerciseId);

        if (workoutExerciseEntity is null)
        {
            return NotFound();
        }

        var mappedWorkoutExercise = _mapper.Map<WorkoutExerciseDto>(workoutExerciseEntity);

        return Ok(mappedWorkoutExercise);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutExerciseDto>> Create(
        int workoutId,
        WorkoutExerciseCreateRequestDto createRequest
        )
    {
        if (!await _workoutRepository.WorkoutExistsAsync(GetCurrentUserId(), workoutId)) return NotFound();

        var finalWorkoutExerciseEntry = _mapper.Map<WorkoutExercise>(createRequest);

        finalWorkoutExerciseEntry.WorkoutId = workoutId;

        await _workoutRepository.CreateExerciseAsync(finalWorkoutExerciseEntry);

        await _workoutRepository.SaveChangesAsync();

        var createdWorkoutExerciseToReturn = _mapper.Map<WorkoutExerciseDto>(finalWorkoutExerciseEntry);

        return CreatedAtRoute(
            "GetWorkoutExercise",
            new { workoutId, workoutExerciseId = finalWorkoutExerciseEntry.Id },
            createdWorkoutExerciseToReturn
        );
    }

    [HttpPut("{workoutExerciseId:int}")]
    public async Task<ActionResult> Update(
        int workoutId,
        int workoutExerciseId,
        WorkoutExerciseUpdateRequestDto updateRequest
        )
    {
        var workoutExerciseEntity = await _workoutRepository.GetExerciseAsync(
            GetCurrentUserId(), workoutId, workoutExerciseId);

        if (workoutExerciseEntity is null) return NotFound();

        _mapper.Map(updateRequest, workoutExerciseEntity);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{workoutExerciseId:int}")]
    public async Task<ActionResult> PartiallyUpdate(
        int workoutId, 
        int workoutExerciseId,
        JsonPatchDocument<WorkoutExerciseUpdateRequestDto> patchDocument)
    {
        var workoutExerciseEntity = await _workoutRepository.GetExerciseAsync(
            GetCurrentUserId(), workoutId, workoutExerciseId);

        if (workoutExerciseEntity is null) return NotFound();

        var workoutExerciseToPatch = _mapper.Map<WorkoutExerciseUpdateRequestDto>(workoutExerciseEntity);
        
        patchDocument.ApplyTo(workoutExerciseToPatch, ModelState);
        
        if (!ModelState.IsValid || !TryValidateModel(ModelState))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(workoutExerciseToPatch, workoutExerciseEntity);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{workoutExerciseId:int}")]
    public async Task<ActionResult> Delete(
        int workoutId,
        int workoutExerciseId
    )
    {
        var workoutExerciseToDelete = await _workoutRepository.GetExerciseAsync(
            GetCurrentUserId(), workoutId, workoutExerciseId);

        if (workoutExerciseToDelete is null) return NotFound();
        
        _workoutRepository.DeleteExercise(workoutExerciseToDelete);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    public Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

}