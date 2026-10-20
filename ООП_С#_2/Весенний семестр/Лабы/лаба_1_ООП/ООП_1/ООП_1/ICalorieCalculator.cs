using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ООП_1
{
    public enum CalorieGoal
    {
        Maintain,
        Lose,
        Gain
    }

    public interface ICalorieCalculator
    {
        double CalculateBmi(double weightKg, double heightCm);
        string GetBmiCategory(double bmi);
        double CalculateDailyCalories(
            double weightKg,
            double heightCm,
            int age,
            bool isMale,
            CalorieGoal goal);
    }
}
