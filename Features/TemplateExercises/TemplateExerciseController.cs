using System.Text.Json;
using AutoMapper;
using C_Sharp_Web_API.Features.TemplateExercises.Dtos;
using C_Sharp_Web_API.Features.TemplateExercises.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.TemplateExercises;

[ApiController]
[Authorize]
[Route("api/exercises")]
public class TemplateExerciseController(
    ITemplateExerciseRepository templateExerciseRepository,
    IMapper mapper
    ) : ControllerBase
{
    
    private readonly ITemplateExerciseRepository _templateExerciseRepository = 
        templateExerciseRepository ?? throw new ArgumentNullException(nameof(templateExerciseRepository));
    
    private readonly IMapper _mapper = 
        mapper ?? throw new ArgumentNullException(nameof(mapper));
    
    private const int MaxPageSize = 20; 
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateExerciseDto>>> GetAll(
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
            await _templateExerciseRepository.GetAllAsync(name, searchQuery, pageNumber, pageSize);
        
        var mappedExercises = _mapper.Map<IEnumerable<TemplateExerciseDto>>(exerciseEntities);
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(mappedExercises);
    }

    [HttpGet("{id:int}", Name = "GetExercise")]
    public async Task<IActionResult> Get(int id, bool includeSetEntries)
    {
        var exercise = await _templateExerciseRepository.GetAsync(id, includeSetEntries);

        if (exercise is null) return NotFound($"Exercise with id: {id} wasn't found when accessing exercises");

        if (includeSetEntries)
        {
            var mappedExerciseWithSetEntries = _mapper.Map<TemplateExerciseDto>(exercise);
            return Ok(mappedExerciseWithSetEntries);
        }

        var mappedExercise = _mapper.Map<TemplateExerciseDto>(exercise);
        return Ok(mappedExercise);
    }

    [HttpPost]
    public async Task<ActionResult<TemplateExerciseDto>> Create(TemplateExerciseCreateRequestDto request)
    {
        var exercise = _mapper.Map<TemplateExercise>(request);
        
        await _templateExerciseRepository.CreateAsync(exercise);

        await _templateExerciseRepository.SaveChangesAsync();

        var createdExercise = _mapper.Map<TemplateExerciseDto>(exercise);

        return CreatedAtRoute(
            "GetExercise",
            new { id = createdExercise.Id },
            createdExercise
        );
    }

    [HttpPut("{exerciseId:int}")]
    public async Task<ActionResult> Update(int exerciseId, TemplateExerciseUpdateRequestDto request)
    {
        var exercise = await _templateExerciseRepository.GetAsync(exerciseId);

        if (exercise is null) return NotFound("The exercise you tried to update was not found in the database");

        _mapper.Map(request, exercise);

        await _templateExerciseRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{exerciseId:int}")]
    public async Task<ActionResult> PartiallyUpdate(
        int exerciseId, 
        JsonPatchDocument<TemplateExerciseUpdateRequestDto> patchDocument)
    {
        var exercise = await _templateExerciseRepository.GetAsync(exerciseId);

        if (exercise is null) return NotFound("The exercise you tried to update was not found in the database");

        var exerciseToPatch = _mapper.Map<TemplateExerciseUpdateRequestDto>(exercise);
        
        patchDocument.ApplyTo(exerciseToPatch, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(ModelState))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(exerciseToPatch, exercise);

        await _templateExerciseRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{exerciseId:int}")]
    public async Task<ActionResult> Delete(int exerciseId)
    {
        var exerciseToDelete = await _templateExerciseRepository.GetAsync(exerciseId);
        
        if (exerciseToDelete is null) return NotFound("The exercise you tried to delete wasn't found");
        
        _templateExerciseRepository.Delete(exerciseToDelete);

        await _templateExerciseRepository.SaveChangesAsync();

        return NoContent();
    }
}