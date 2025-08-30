using C_Sharp_Web_API.Exercises.Models;
using C_Sharp_Web_API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Exercises;

[ApiController]
[Route("api/exercises")]
public class ExerciseController : ControllerBase
{

    private readonly ILogger<ExerciseController> _logger;

    public ExerciseController(ILogger<ExerciseController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<ExerciseDto>> GetExercises()
    {
        return Ok(ExercisesDataStore.Current.Exercises);
    }

    [HttpGet("{id}", Name = "GetExercise")]
    public ActionResult<ExerciseDto> GetExercises(int id)
    {
        try
        {
            var cityToReturn = ExercisesDataStore.Current.Exercises.FirstOrDefault(c => c.Id == id);

            if (cityToReturn == null)
            {
                _logger.LogWarning($"Exercise with id: {id} wasn't found when accessing exercises");
                return NotFound();
            }

            return Ok(cityToReturn);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Exception while getting exercise with id: {id}", e);

            // This information while go to the API consumer
            return StatusCode(500, "A problem happened while handling your request"); 
        }
        
    }

    [HttpPost]
    public ActionResult<ExerciseDto> CreateExercise(ExerciseCreateRequestDto request)
    {
        // demo purposes - to be improved
        var maxExerciseId = ExercisesDataStore.Current.Exercises.Max(e => e.Id);

        // Map request to exercise dto
        var toInsert = new ExerciseDto()
        {
            Id = ++maxExerciseId,
            MuscleGroup = request.MuscleGroup,
            Name = request.Name,
            Unit = request.Unit
        };
        
        ExercisesDataStore.Current.Exercises.Add(toInsert);

        return CreatedAtRoute("GetExercise", new
        {
            id = toInsert.Id
        }, 
            toInsert);
    }

    [HttpPut("{exerciseId}")]
    public ActionResult UpdateExercise(int exerciseId, ExerciseUpdateRequestDto request)
    {
        var exercise = ExercisesDataStore.Current.Exercises.FirstOrDefault(e => e.Id == exerciseId);

        if (exercise == null)
        {
            return NotFound(); 
        }

        exercise.Name = request.Name;
        exercise.MuscleGroup = request.MuscleGroup;
        exercise.Unit = request.Unit;

        return NoContent();
    }

    [HttpPatch("{exerciseId}")]
    public ActionResult PartiallyUpdateExercise(
        int exerciseId, 
        JsonPatchDocument<ExerciseUpdateRequestDto> patchDocument)
    {
        var exercise = ExercisesDataStore.Current.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise == null)
        {
            return NotFound(); 
        }

        var exerciseToPatch =
            new ExerciseUpdateRequestDto()
            {
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup,
                Unit = exercise.Unit
            };
        
        patchDocument.ApplyTo(exerciseToPatch, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(exerciseToPatch))
        {
            return BadRequest(); 
        }

        exercise.Name = exerciseToPatch.Name;
        exercise.MuscleGroup = exerciseToPatch.MuscleGroup;
        exercise.Unit = exerciseToPatch.Unit;

        return NoContent();
    }

    [HttpDelete("{exerciseId}")]
    public ActionResult DeleteExercise(int exerciseId)
    {
        // Step 1: Check if exercise with given id exists
        var exerciseToDelete = ExercisesDataStore.Current.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exerciseToDelete == null)
        {
            return NotFound(); 
        }
        
        // Step 2: Delete the exercise
        ExercisesDataStore.Current.Exercises.Remove(exerciseToDelete);

        return NoContent();
    }
}