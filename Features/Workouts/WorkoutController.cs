using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using C_Sharp_Web_API.Features.Workouts.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.Workouts;


[ApiController]
[Authorize]
[Route("workout-api/workouts")]
public class WorkoutController(
    IWorkoutRepository workoutRepository,
    IMapper mapper
    ) : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository =
        workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IMapper _mapper =
        mapper ?? throw new ArgumentNullException(nameof(mapper));
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutWithoutExercisesDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
        )
    {
        var (workoutEntities, paginationMetadata) = 
            await _workoutRepository.GetAllAsync(GetCurrentUserId(), name, searchQuery, pageNumber, pageSize);

        var mappedWorkoutEntities = _mapper.Map<IEnumerable<WorkoutWithoutExercisesDto>>(workoutEntities);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(mappedWorkoutEntities);
    }

    [HttpGet("{workoutId:int}", Name = "GetWorkout")]
    public async Task<IActionResult> Get(int workoutId)
    {
        var workoutEntity = await _workoutRepository.GetAsync(GetCurrentUserId(), workoutId);

        if (workoutEntity is null) return NotFound();
        
        var mappedWorkoutWithoutExercises = _mapper.Map<WorkoutWithoutExercisesDto>(workoutEntity);
        return Ok(mappedWorkoutWithoutExercises);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDto>> Create(WorkoutCreateRequestDto createRequest)
    {
        var workout = _mapper.Map<Workout>(createRequest);

        workout.ApiUserId = GetCurrentUserId();

        await _workoutRepository.CreateAsync(workout);

        await _workoutRepository.SaveChangesAsync();

        var createdWorkout = _mapper.Map<WorkoutWithoutExercisesDto>(workout);

        return CreatedAtRoute(
            "GetWorkout",
            new { workoutId = createdWorkout.Id },
            createdWorkout
        );
    }

    [HttpPut("{workoutId:int}")]
    public async Task<ActionResult> Update(int workoutId, WorkoutUpdateRequestDto updateRequest)
    {
        var workout = await _workoutRepository.GetAsync(GetCurrentUserId(), workoutId);

        if (workout is null) return NotFound();

        _mapper.Map(updateRequest, workout);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{workoutId:int}")]
    public async Task<ActionResult> PartiallyUpdate(
        int workoutId,
        JsonPatchDocument<WorkoutUpdateRequestDto> patchDocument)
    {
        var workout = await _workoutRepository.GetAsync(GetCurrentUserId(), workoutId);

        if (workout is null) return NotFound();

        var workoutToPatch = _mapper.Map<WorkoutUpdateRequestDto>(workout);
        
        patchDocument.ApplyTo(workoutToPatch, ModelState);
        
        if (!ModelState.IsValid || !TryValidateModel(ModelState))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(workoutToPatch, workout);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{workoutId:int}")]
    public async Task<ActionResult> Delete(int workoutId)
    {
        var workoutToDelete = await _workoutRepository.GetAsync(GetCurrentUserId(), workoutId);

        if (workoutToDelete is null) return NoContent();
        
        _workoutRepository.Delete(workoutToDelete);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }
    
    public Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}