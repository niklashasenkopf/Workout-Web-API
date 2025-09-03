# Relational Model

- ApiUser(UserGuid)
- Workout(WorkoutId, Name, UserGuid)
- WorkoutExercise(WorkoutExerciseId, Order, TemplateExerciseId, WorkoutId)
- TemplateExercise(TemplateExerciseId, Name, MuscleGroup, Unit)
- SetEntry(SetEntryId, Date, Result, Reps, WorkoutExerciseId)
