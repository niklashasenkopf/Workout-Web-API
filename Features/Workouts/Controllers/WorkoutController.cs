using System.Text.Json;
using AutoMapper;
using C_Sharp_Web_API.Features.Exercises.Persistence;
using C_Sharp_Web_API.Features.Workouts.Domain;
using C_Sharp_Web_API.Features.Workouts.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.Workouts.Controllers;


[ApiController]
[Authorize]
[Route("api/workouts")]
public class WorkoutController(
    IWorkoutRepository workoutRepository,
    IMapper mapper,
    IExerciseRepository exerciseRepository
    ) : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository =
        workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IMapper _mapper =
        mapper ?? throw new ArgumentNullException(nameof(mapper));

    private readonly IExerciseRepository _exerciseRepository =
        exerciseRepository ?? throw new ArgumentNullException((nameof(exerciseRepository)));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutWithoutExercisesDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
        )
    {
        var (workoutEntities, paginationMetadata) = 
            await _workoutRepository.GetAllAsync(name, searchQuery, pageNumber, pageSize);

        var mappedWorkoutEntities = _mapper.Map<IEnumerable<WorkoutWithoutExercisesDto>>(workoutEntities);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(mappedWorkoutEntities);
    }

    [HttpGet("{workoutId:int}", Name = "GetWorkout")]
    public async Task<IActionResult> Get(int workoutId, bool includeExercises)
    {
        var workoutEntity = await _workoutRepository.GetAsync(workoutId, includeExercises);

        if (workoutEntity is null) return NotFound();

        if (includeExercises)
        {
            var mappedWorkoutWithExercises = _mapper.Map<WorkoutDto>(workoutEntity);
            return Ok(mappedWorkoutWithExercises);
        }

        var mappedWorkoutWithoutExercises = _mapper.Map<WorkoutWithoutExercisesDto>(workoutEntity);
        return Ok(mappedWorkoutWithoutExercises);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutDto>> Create(WorkoutCreateRequestDto createRequest)
    {
        var workout = _mapper.Map<Workout>(createRequest);

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
        var workout = await _workoutRepository.GetAsync(workoutId);

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
        var workout = await _workoutRepository.GetAsync(workoutId);

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
        var workoutToDelete = await _workoutRepository.GetAsync(workoutId);

        if (workoutToDelete is null) return NoContent();
        
        _workoutRepository.Delete(workoutToDelete);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{workoutId:int}/exercises/{exerciseId:int}")]
    public async Task<IActionResult> AddExerciseToWorkout(int workoutId, int exerciseId)
    {
        var workout = await _workoutRepository.GetAsync(workoutId);
        if (workout is null) return NotFound("Workout not found");

        var exercise = await _exerciseRepository.GetAsync(exerciseId);
        if (exercise is null) return NotFound("Exercise not found");

        _workoutRepository.AddExerciseToWorkout(workout, exercise);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{workoutId:int}/exercises/{exerciseId:int}")]
    public async Task<IActionResult> RemoveExerciseFromWorkout(int workoutId, int exerciseId)
    {
        var workout = await _workoutRepository.GetAsync(workoutId, true);
        if (workout is null) return NoContent();

        var exercise = await _exerciseRepository.GetAsync(exerciseId);
        if (exercise is null) return NoContent();

        _workoutRepository.RemoveExerciseFromWorkout(workout, exercise);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }
}