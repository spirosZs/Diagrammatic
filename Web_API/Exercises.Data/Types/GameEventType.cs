namespace Exercises.Data.Types
{
    public enum GameEventType
    {
        Started,
        Updated,
        Restarted,
        Ended,

        ParticipantEntered,
        ParticipantLeft,
        ExerciseCompleted,
        ExerciseSkipped,
        ExerciseTimeChanged,
    }
}