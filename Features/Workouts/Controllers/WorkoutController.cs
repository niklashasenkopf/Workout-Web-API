using AutoMapper;
using C_Sharp_Web_API.Features.Workouts.Dtos;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace C_Sharp_Web_API.Features.Workouts.Controllers;


[ApiController]
[Route("api/workouts")]
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
    public async Task<ActionResult<IEnumerable<WorkoutWithoutExercisesDto>>> GetAll()
    {
        var workoutEntities = await _workoutRepository.GetAllAsync();

        var mappedWorkoutEntities = _mapper.Map<IEnumerable<WorkoutWithoutExercisesDto>>(workoutEntities);

        return Ok(mappedWorkoutEntities);
    }

    [HttpGet("{workoutId:int}")]
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
}