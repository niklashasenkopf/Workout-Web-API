namespace C_Sharp_Web_API.Features.SetEntries.Dtos;

public class SetEntryDto
{
    public int Id { get; init; }
    public DateOnly Date { get; init; }
    public double Result { get; init; }
    public int Reps { get; init; }
}