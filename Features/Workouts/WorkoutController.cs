using System.Security.Claims;
using System.Text.Json;
using Asp.Versioning;
using AutoMapper;
using C_Sharp_Web_API.Features.Workouts.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.Workouts;

/// <summary>
/// Controller for managing workout resources, including retrieving, creating, updating, and deleting workouts.
/// </summary>
[ApiController]
[Authorize]
[Route("workout-api/workouts")]
[ApiVersion(0.1)]
public class WorkoutController(
    IWorkoutRepository workoutRepository,
    IMapper mapper
    ) : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository =
        workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IMapper _mapper =
        mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <summary>
    /// Retrieves a collection of workouts for the currently authenticated user, optionally filtered by name or search query and paginated.
    /// </summary>
    /// <param name="name">An optional filter to retrieve workouts that match the specified name.</param>
    /// <param name="searchQuery">An optional filter to retrieve workouts that match the specified search query.</param>
    /// <param name="pageNumber">The page number for pagination. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page for pagination. Defaults to 10.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing a collection of <see cref="WorkoutWithoutExercisesDto"/> for the requested criteria and pagination metadata.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Retrieves a workout by its ID for the currently authenticated user.
    /// </summary>
    /// <param name="workoutId">The ID of the workout to retrieve.</param>
    /// <returns>An <see cref="IActionResult"/> containing the workout details if found; otherwise, a NotFound result.</returns>
    [HttpGet("{workoutId:int}", Name = "GetWorkout")]
    public async Task<IActionResult> Get(int workoutId)
    {
        var workoutEntity = await _workoutRepository.GetAsync(GetCurrentUserId(), workoutId);

        if (workoutEntity is null) return NotFound();
        
        var mappedWorkoutWithoutExercises = _mapper.Map<WorkoutWithoutExercisesDto>(workoutEntity);
        return Ok(mappedWorkoutWithoutExercises);
    }

    /// <summary>
    /// Creates a new workout for the currently authenticated user.
    /// </summary>
    /// <param name="createRequest">The details of the workout to be created, encapsulated in a <see cref="WorkoutCreateRequestDto"/>.</param>
    /// <returns>The created workout as a <see cref="WorkoutDto"/> along with a route to access it.</returns>
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

    /// <summary>
    /// Updates an existing workout with the provided details.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout to be updated.</param>
    /// <param name="updateRequest">The object containing the updated properties of the workout.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation, which returns a <see cref="NoContentResult"/> if the update is successful, or a <see cref="NotFoundResult"/> if the workout does not exist.</returns>
    [HttpPut("{workoutId:int}")]
    public async Task<ActionResult> Update(int workoutId, WorkoutUpdateRequestDto updateRequest)
    {
        var workout = await _workoutRepository.GetAsync(GetCurrentUserId(), workoutId);

        if (workout is null) return NotFound();

        _mapper.Map(updateRequest, workout);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing workout using a JsonPatch document.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout to be updated.</param>
    /// <param name="patchDocument">A JSON patch document containing the updates to be applied to the workout.</param>
    /// <returns>A <see cref="Task{ActionResult}"/> signifying the result of the partial update operation.
    /// Returns 204 No Content on success or 404 Not Found if the workout does not exist.</returns>
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

    /// <summary>
    /// Deletes a workout for the currently authenticated user based on the provided workout ID.
    /// </summary>
    /// <param name="workoutId">The unique identifier of the workout to be deleted.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. If the workout does not exist, returns a NoContent response.</returns>
    [HttpDelete("{workoutId:int}")]
    public async Task<ActionResult> Delete(int workoutId)
    {
        var workoutToDelete = await _workoutRepository.GetAsync(GetCurrentUserId(), workoutId);

        if (workoutToDelete is null) return NoContent();
        
        _workoutRepository.Delete(workoutToDelete);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }
    
    [NonAction]
    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}