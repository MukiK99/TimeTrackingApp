using TimeTracking.Domain.Enums;

namespace TimeTracking.Domain.Models
{
    public class Exercising : BaseActivity
    {
        public TypeOfExercise ExerciseType { get; set; }
        public Exercising() { }

        public Exercising(TypeOfExercise exerciseType)
        {
            ExerciseType = exerciseType;
        }
    }
}
