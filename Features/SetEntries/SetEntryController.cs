using System.Text.Json;
using AutoMapper;
using C_Sharp_Web_API.Features.SetEntries.Domain;
using C_Sharp_Web_API.Features.SetEntries.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.SetEntries;

[Route("workout-api/workouts/{workoutId:int}/exercises/{exerciseId:int}/setEntries")]
[Authorize]
[ApiController]
public class SetEntryController(
    IWorkoutRepository workoutRepository,
    IMapper mapper
    ) : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository = 
        workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));

    private readonly IMapper _mapper =
        mapper ?? throw new ArgumentNullException(nameof(mapper));

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
        if (!await _workoutRepository.ExistsAsync(workoutId))
        {
            return NotFound();
        }
        
        var (setEntriesOfExercise, paginationMetadata) = 
            await _workoutRepository.GetAllSetEntriesAsync(
                workoutId, exerciseId, date, searchQuery, pageSize, pageNumber);

        var mappedSetEntries = _mapper.Map<IEnumerable<SetEntryDto>>(setEntriesOfExercise);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(mappedSetEntries);
    }

    [HttpGet("{setEntryId:int}", Name = "GetSetEntry")]
    public async Task<ActionResult<SetEntryDto>> Get(
        int workoutId,
        int exerciseId, 
        int setEntryId)
    {
        if (!await _workoutRepository.ExistsAsync(exerciseId))
        {
            return NotFound();
        }

        var setEntry = await _workoutRepository.GetSetEntryAsync(workoutId, exerciseId, setEntryId);

        if (setEntry is null) return NotFound();

        var mappedSetEntry = _mapper.Map<SetEntryDto>(setEntry);

        return Ok(mappedSetEntry);
    }

    [HttpPost]
    public async Task<ActionResult<SetEntryDto>> Create(
        int workoutId,
        int exerciseId, 
        SetEntryCreateRequestDto createRequest)
    {
        if (!await _workoutRepository.ExistsAsync(exerciseId)) return NotFound();
        
        var finalSetEntry = _mapper.Map<SetEntry>(createRequest);

        await _workoutRepository.CreateSetEntryAsync(workoutId, exerciseId, finalSetEntry);

        await _workoutRepository.SaveChangesAsync();

        var createdSetEntryToReturn = _mapper.Map<SetEntryDto>(finalSetEntry);

        return CreatedAtRoute("GetSetEntry",
            new
            {
                exerciseId,
                setEntryId = createdSetEntryToReturn.Id
            }, createdSetEntryToReturn);
    }

    [HttpPut("{setEntryId:int}")]
    public async Task<ActionResult> Update(
        int workoutId,
        int exerciseId,
        int setEntryId,
        SetEntryUpdateRequestDto updateRequest)
    {
        if (!await _workoutRepository.ExistsAsync(exerciseId)) return NotFound();

        var setEntryEntity = await _workoutRepository.GetSetEntryAsync(workoutId, exerciseId, setEntryId);

        if (setEntryEntity is null) return NotFound();

        _mapper.Map(updateRequest, setEntryEntity);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{setEntryId:int}")]
    public async Task<ActionResult> PartiallyUpdate(
        int workoutId,
        int exerciseId,
        int setEntryId,
        JsonPatchDocument<SetEntryUpdateRequestDto> patchDocument)
    {
        if (!await _workoutRepository.ExistsAsync(exerciseId)) return NotFound();

        var setEntryEntity = await _workoutRepository.GetSetEntryAsync(workoutId, exerciseId, setEntryId);

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

    [HttpDelete("{setEntryId:int}")]
    public async Task<ActionResult> Delete(
        int workoutId,
        int exerciseId, 
        int setEntryId)
    {
        if (!await _workoutRepository.ExistsAsync(exerciseId)) return NotFound();

        var setEntryToDelete = await _workoutRepository.GetSetEntryAsync(workoutId, exerciseId, setEntryId);

        if (setEntryToDelete is null) return NotFound();
        
        _workoutRepository.DeleteSetEntry(workoutId, exerciseId, setEntryToDelete);

        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }
}