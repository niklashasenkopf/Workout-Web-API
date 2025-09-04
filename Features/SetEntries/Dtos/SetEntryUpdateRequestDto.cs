using System.ComponentModel.DataAnnotations;

namespace C_Sharp_Web_API.Features.SetEntries.Dtos;

public class SetEntryUpdateRequestDto
{
    [Required(ErrorMessage = "Please supply a valid date for a set entry")]
    public DateOnly Date { get; init; }
    
    [Required(ErrorMessage = "Please supply a valid result for your set entry")]
    public double Result { get; init; }
    
    [Required(ErrorMessage = "Please supply a valid number of repetitions for your set entry")]
    public int Reps { get; init; }
}