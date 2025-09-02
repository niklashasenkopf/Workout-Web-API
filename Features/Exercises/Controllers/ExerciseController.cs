using System.Text.Json;
using AutoMapper;
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.Exercises.dtos;
using C_Sharp_Web_API.Features.Exercises.Dtos;
using C_Sharp_Web_API.Features.Exercises.Persistence;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.Exercises.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExerciseController(
    IExerciseRepository exerciseRepository,
    IMapper mapper
    ) : ControllerBase
{
    
    private readonly IExerciseRepository _exerciseRepository = 
        exerciseRepository ?? throw new ArgumentNullException(nameof(exerciseRepository));
    
    private readonly IMapper _mapper = 
        mapper ?? throw new ArgumentNullException(nameof(mapper));
    
    private const int MaxPageSize = 20; 
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseWithoutSetEntriesDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
        )
    {
        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }
        
        var (exerciseEntities, paginationMetadata) = 
            await _exerciseRepository.GetAllAsync(name, searchQuery, pageNumber, pageSize);
        
        var mappedExercises = _mapper.Map<IEnumerable<ExerciseWithoutSetEntriesDto>>(exerciseEntities);
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(mappedExercises);
    }

    [HttpGet("{id:int}", Name = "GetExercise")]
    public async Task<IActionResult> Get(int id, bool includeSetEntries)
    {
        var exercise = await _exerciseRepository.GetAsync(id, includeSetEntries);

        if (exercise is null) return NotFound($"Exercise with id: {id} wasn't found when accessing exercises");

        if (includeSetEntries)
        {
            var mappedExerciseWithSetEntries = _mapper.Map<ExerciseDto>(exercise);
            return Ok(mappedExerciseWithSetEntries);
        }

        var mappedExercise = _mapper.Map<ExerciseWithoutSetEntriesDto>(exercise);
        return Ok(mappedExercise);
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> Create(ExerciseCreateRequestDto request)
    {
        var exercise = _mapper.Map<Exercise>(request);
        
        await _exerciseRepository.CreateAsync(exercise);

        await _exerciseRepository.SaveChangesAsync();

        var createdExercise = _mapper.Map<ExerciseDto>(exercise);

        return CreatedAtRoute(
            "GetExercise",
            new { id = createdExercise.Id },
            createdExercise
        );
    }

    [HttpPut("{exerciseId:int}")]
    public async Task<ActionResult> Update(int exerciseId, ExerciseUpdateRequestDto request)
    {
        var exercise = await _exerciseRepository.GetAsync(exerciseId);

        if (exercise is null) return NotFound("The exercise you tried to update was not found in the database");

        _mapper.Map(request, exercise);

        await _exerciseRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{exerciseId:int}")]
    public async Task<ActionResult> PartiallyUpdate(
        int exerciseId, 
        JsonPatchDocument<ExerciseUpdateRequestDto> patchDocument)
    {
        var exercise = await _exerciseRepository.GetAsync(exerciseId);

        if (exercise is null) return NotFound("The exercise you tried to update was not found in the database");

        var exerciseToPatch = _mapper.Map<ExerciseUpdateRequestDto>(exercise);
        
        patchDocument.ApplyTo(exerciseToPatch, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(ModelState))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(exerciseToPatch, exercise);

        await _exerciseRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{exerciseId:int}")]
    public async Task<ActionResult> Delete(int exerciseId)
    {
        var exerciseToDelete = await _exerciseRepository.GetAsync(exerciseId);
        
        if (exerciseToDelete is null) return NotFound("The exercise you tried to delete wasn't found");
        
        _exerciseRepository.Delete(exerciseToDelete);

        await _exerciseRepository.SaveChangesAsync();

        return NoContent();
    }
}