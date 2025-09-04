using System.Text.Json;
using Asp.Versioning;
using AutoMapper;
using C_Sharp_Web_API.Features.TemplateExercises.Dtos;
using C_Sharp_Web_API.Features.TemplateExercises.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.TemplateExercises;

/// <summary>
/// Controller responsible for managing template exercise entities through CRUD operations and other related actions.
/// </summary>
[ApiController]
[Authorize]
[Route("workout-api/templateExercises")]
[ApiVersion(0.1)]
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

    /// <summary>
    /// Retrieves a paginated list of template exercises based on the provided filters and pagination parameters.
    /// </summary>
    /// <param name="name">An optional name filter to match exercises.</param>
    /// <param name="searchQuery">An optional search query for matching exercises.</param>
    /// <param name="pageNumber">The page number of the results to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10 and has a maximum limit of 20.</param>
    /// <returns>An <see cref="ActionResult"/> containing a collection of <see cref="TemplateExerciseDto"/> instances.</returns>
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

    /// <summary>
    /// Retrieves the details of a specific template exercise by its ID, optionally including associated set entries.
    /// </summary>
    /// <param name="id">The ID of the template exercise to retrieve.</param>
    /// <param name="includeSetEntries">A boolean indicating whether to include associated set entries in the result.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the details of the requested template exercise.
    /// Returns a 404 status code if the exercise with the specified ID is not found.
    /// </returns>
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

    /// <summary>
    /// Creates a new template exercise based on the provided request data.
    /// </summary>
    /// <param name="request">The data for creating the template exercise, including its name, muscle group, and unit type.</param>
    /// <returns>An <see cref="ActionResult"/> containing the created <see cref="TemplateExerciseDto"/> instance.</returns>
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

    /// <summary>
    /// Updates an existing template exercise with the provided details.
    /// </summary>
    /// <param name="exerciseId">The unique identifier of the exercise to be updated.</param>
    /// <param name="request">The updated details of the exercise encapsulated in a <see cref="TemplateExerciseUpdateRequestDto"/>.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the outcome of the operation. Returns a 204 No Content status on success or a 404 Not Found status if the exercise does not exist.</returns>
    [HttpPut("{exerciseId:int}")]
    public async Task<ActionResult> Update(int exerciseId, TemplateExerciseUpdateRequestDto request)
    {
        var exercise = await _templateExerciseRepository.GetAsync(exerciseId);

        if (exercise is null) return NotFound("The exercise you tried to update was not found in the database");

        _mapper.Map(request, exercise);

        await _templateExerciseRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing template exercise with the specified changes.
    /// </summary>
    /// <param name="exerciseId">The unique identifier of the template exercise to update.</param>
    /// <param name="patchDocument">The JSON Patch document containing the changes to apply.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns 404 if the exercise is not found, or 204 for successful partial update.</returns>
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

    /// <summary>
    /// Deletes a specific template exercise identified by its ID.
    /// </summary>
    /// <param name="exerciseId">The unique identifier of the template exercise to delete.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the delete operation:
    /// either <see cref="NoContentResult"/> for a successful deletion or <see cref="NotFoundResult"/> if the exercise was not found.</returns>
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