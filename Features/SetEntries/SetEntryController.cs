using System.Security.Claims;
using System.Text.Json;
using Asp.Versioning;
using AutoMapper;
using C_Sharp_Web_API.Features.SetEntries.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.SetEntries;

/// <summary>
/// Controller responsible for managing set entries associated with specific exercises within workouts.
/// Provides functionality to retrieve, create, update, and delete set entries tied to a particular user's workout and exercise.
/// </summary>
[Route("workout-api/workouts/{workoutId:int}/exercises/{exerciseId:int}/setEntries")]
[Authorize]
[ApiController]
[ApiVersion(0.1)]
public class SetEntryController(
    IWorkoutRepository workoutRepository,
    IMapper mapper
    ) : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository = 
        workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IMapper _mapper =
        mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <summary>
    /// Retrieves a list of set entries for a specific exercise within a workout.
    /// </summary>
    /// <param name="workoutId">The ID of the workout to which the exercise belongs.</param>
    /// <param name="exerciseId">The ID of the exercise whose set entries are to be retrieved.</param>
    /// <param name="date">An optional filter to retrieve set entries for a specific date.</param>
    /// <param name="searchQuery">An optional search query to filter set entries by specific terms.</param>
    /// <param name="pageNumber">The current page number for pagination. Defaults to 1.</param>
    /// <param name="pageSize">The number of entries per page for pagination. Defaults to 10.</param>
    /// <returns>A list of set entries for the specified exercise, including pagination metadata in the response headers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SetEntryDto>>> GetAll(
        int workoutId,
        int exerciseId,
        [FromQuery] DateOnly? date,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        if (!await _workoutRepository.WorkoutExistsAsync(GetCurrentUserId(), workoutId))
        {
            return NotFound();
        }
        
        var (setEntriesOfExercise, paginationMetadata) = 
            await _workoutRepository.GetAllSetEntriesAsync(
                GetCurrentUserId(),workoutId, exerciseId, date, searchQuery, pageSize, pageNumber);

        var mappedSetEntries = _mapper.Map<IEnumerable<SetEntryDto>>(setEntriesOfExercise);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(mappedSetEntries);
    }

    /// <summary>
    /// Retrieves a specific set entry for a given workout and exercise.
    /// </summary>
    /// <param name="workoutId">The ID of the workout containing the target exercise and set entry.</param>
    /// <param name="exerciseId">The ID of the exercise associated with the set entry.</param>
    /// <param name="setEntryId">The ID of the set entry to retrieve.</param>
    /// <returns>The details of the requested set entry, or a NotFound result if no matching set entry is found.</returns>
    [HttpGet("{setEntryId:int}", Name = "GetSetEntry")]
    public async Task<ActionResult<SetEntryDto>> Get(
        int workoutId,
        int exerciseId,
        int setEntryId)
    {
        var setEntry = await _workoutRepository.GetSetEntryAsync(GetCurrentUserId(), workoutId, exerciseId, setEntryId);

        if (setEntry is null) return NotFound();

        var mappedSetEntry = _mapper.Map<SetEntryDto>(setEntry);

        return Ok(mappedSetEntry);
    }

    /// <summary>
    /// Creates a new set entry for a specific exercise within a workout.
    /// </summary>
    /// <param name="workoutId">The ID of the workout to which the exercise belongs.</param>
    /// <param name="exerciseId">The ID of the exercise for which the set entry is being created.</param>
    /// <param name="createRequest">The request object containing the details of the set entry to be created.</param>
    /// <returns>The newly created set entry as a data transfer object.</returns>
    [HttpPost]
    public async Task<ActionResult<SetEntryDto>> Create(
        int workoutId,
        int exerciseId,
        SetEntryCreateRequestDto createRequest)
    {
        if (!await _workoutRepository.WorkoutExerciseExistsAsync(GetCurrentUserId(), workoutId, exerciseId)) return NotFound();
        
        var finalSetEntry = _mapper.Map<SetEntry>(createRequest);

        finalSetEntry.WorkoutExerciseId = exerciseId;

        await _workoutRepository.CreateSetEntryAsync(finalSetEntry);

        await _workoutRepository.SaveChangesAsync();

        var createdSetEntryToReturn = _mapper.Map<SetEntryDto>(finalSetEntry);

        return CreatedAtRoute("GetSetEntry",
            new
            {
                exerciseId,
                setEntryId = createdSetEntryToReturn.Id
            }, createdSetEntryToReturn);
    }

    /// <summary>
    /// Updates an existing set entry for a specific exercise within a workout.
    /// </summary>
    /// <param name="workoutId">The ID of the workout to which the exercise belongs.</param>
    /// <param name="exerciseId">The ID of the exercise containing the set entry to be updated.</param>
    /// <param name="setEntryId">The ID of the set entry to be updated.</param>
    /// <param name="updateRequest">The updated set entry data.</param>
    /// <returns>No content if the update is successful. Returns NotFound if the specified set entry does not exist.</returns>
    [HttpPut("{setEntryId:int}")]
    public async Task<ActionResult> Update(
        int workoutId,
        int exerciseId,
        int setEntryId,
        SetEntryUpdateRequestDto updateRequest)
    {
        var setEntryEntity = await _workoutRepository.GetSetEntryAsync(GetCurrentUserId(), workoutId, exerciseId, setEntryId);

        if (setEntryEntity is null) return NotFound();

        _mapper.Map(updateRequest, setEntryEntity);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Partially updates a set entry for a specific exercise within a workout.
    /// </summary>
    /// <param name="workoutId">The ID of the workout to which the exercise belongs.</param>
    /// <param name="exerciseId">The ID of the exercise to which the set entry belongs.</param>
    /// <param name="setEntryId">The ID of the set entry to be updated.</param>
    /// <param name="patchDocument">A JSON patch document describing the updates to be applied to the set entry.</param>
    /// <returns>An HTTP status code indicating the result of the operation. Returns 204 No Content if successful,
    /// 400 Bad Request if the model state is invalid, or 404 Not Found if the set entry does not exist.</returns>
    [HttpPatch("{setEntryId:int}")]
    public async Task<ActionResult> PartiallyUpdate(
        int workoutId,
        int exerciseId,
        int setEntryId,
        JsonPatchDocument<SetEntryUpdateRequestDto> patchDocument)
    {
        var setEntryEntity = await _workoutRepository.GetSetEntryAsync(GetCurrentUserId(), workoutId, exerciseId, setEntryId);

        if (setEntryEntity is null) return NotFound();

        var setEntryToPatch = _mapper.Map<SetEntryUpdateRequestDto>(setEntryEntity);
        
        patchDocument.ApplyTo(setEntryToPatch, ModelState);
        
        if (!ModelState.IsValid || !TryValidateModel(ModelState))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(setEntryToPatch, setEntryEntity);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific set entry for a given exercise within a workout.
    /// </summary>
    /// <param name="workoutId">The ID of the workout containing the exercise.</param>
    /// <param name="exerciseId">The ID of the exercise containing the set entry.</param>
    /// <param name="setEntryId">The ID of the set entry to be deleted.</param>
    /// <returns>A 204 No Content response if the deletion is successful,
    /// or a 404 Not Found response if the set entry is not found.</returns>
    [HttpDelete("{setEntryId:int}")]
    public async Task<ActionResult> Delete(
        int workoutId,
        int exerciseId,
        int setEntryId)
    {
        var setEntryToDelete = await _workoutRepository.GetSetEntryAsync(GetCurrentUserId(), workoutId, exerciseId, setEntryId);

        if (setEntryToDelete is null) return NotFound();
        
        _workoutRepository.DeleteSetEntry(setEntryToDelete);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }
    
    [NonAction]
    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}