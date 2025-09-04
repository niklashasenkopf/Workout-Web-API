using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using C_Sharp_Web_API.Features.WorkoutExercises.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.WorkoutExercises;

/// <summary>
/// Controller for managing workout exercises.
/// Provides endpoints for CRUD operations specific to exercises within a workout.
/// </summary>
[Route("workout-api/workouts/{workoutId:int}/exercises")]
[Authorize]
[ApiController]
[ApiVersion(0.1)]
public class WorkoutExerciseController(
    IWorkoutRepository workoutRepository,
    IMapper mapper
    ) : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository
        = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IMapper _mapper
        = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <summary>
    /// Retrieves all exercises associated with a specific workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier for the workout whose exercises are to be retrieved.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the exercises, each represented as a <see cref="WorkoutExerciseDto"/>.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutExerciseDto>>> GetAll(int workoutId)
    {
        var workoutExerciseEntities =
            await _workoutRepository.GetAllExercisesAsync(GetCurrentUserId(), workoutId);

        var mappedExerciseEntities = _mapper.Map<WorkoutExerciseDto>(workoutExerciseEntities);

        return Ok(mappedExerciseEntities);
    }

    /// <summary>
    /// Retrieves a specific exercise from a workout based on its identifiers.
    /// </summary>
    /// <param name="workoutId">The unique identifier for the workout containing the exercise.</param>
    /// <param name="workoutExerciseId">The unique identifier for the exercise to be retrieved.</param>
    /// <returns>An <see cref="IActionResult"/> containing the exercise data represented as a <see cref="WorkoutExerciseDto"/>, or a NotFound response if the exercise does not exist.</returns>
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

    /// <summary>
    /// Creates a new exercise and associates it with a specified workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout to which the new exercise will be added.</param>
    /// <param name="createRequest">The data required to create the exercise, encapsulated in a <see cref="WorkoutExerciseCreateRequestDto"/>.</param>
    /// <returns>A <see cref="ActionResult{T}"/> containing the created exercise, represented as a <see cref="WorkoutExerciseDto"/>.</returns>
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

    /// <summary>
    /// Updates an existing workout exercise associated with a specific workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier for the workout that contains the exercise to be updated.</param>
    /// <param name="workoutExerciseId">The unique identifier for the workout exercise to be updated.</param>
    /// <param name="updateRequest">The updated data for the workout exercise encapsulated in a <see cref="WorkoutExerciseUpdateRequestDto"/>.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the outcome of the update operation. Returns <see cref="NoContentResult"/> if the update is successful, or <see cref="NotFoundResult"/> if the specified workout exercise does not exist.</returns>
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

    /// <summary>
    /// Applies a partial update to a specific workout exercise within a workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier for the workout that contains the exercise to be updated.</param>
    /// <param name="workoutExerciseId">The unique identifier for the workout exercise to be partially updated.</param>
    /// <param name="patchDocument">The JSON patch document containing the operations to apply to the workout exercise.</param>
    /// <returns>A status indication of the update operation: <c>NoContent</c> when successful, <c>NotFound</c> if the exercise is not found, or <c>BadRequest</c> if the update is invalid.</returns>
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

    /// <summary>
    /// Deletes a specific exercise associated with a workout.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout containing the exercise to be deleted.</param>
    /// <param name="workoutExerciseId">The unique identifier of the exercise to be deleted within the workout.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the delete operation. Returns <see cref="ActionResult"/> indicating the outcome.</returns>
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

    [NonAction]
    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

}