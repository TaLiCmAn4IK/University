using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ООП_1
{
    public class CalorieCalculator : ICalorieCalculator
    {
        public double CalculateBmi(double weightKg, double heightCm)
        {
            double heightM = heightCm / 100.0;
            return weightKg / (heightM * heightM);
        }

        public string GetBmiCategory(double bmi)
        {
            if (bmi < 18.5) return "Недостаточная масса";
            if (bmi < 25) return "Норма";
            if (bmi < 30) return "Избыточная масса";
            return "Ожирение";
        }

        public double CalculateDailyCalories(
            double weightKg,
            double heightCm,
            int age,
            bool isMale,
            CalorieGoal goal)
        {
            double bmr = isMale
                ? 10 * weightKg + 6.25 * heightCm - 5 * age + 5
                : 10 * weightKg + 6.25 * heightCm - 5 * age - 161;

            double maintenance = bmr * 1.2; // малоподвижный режим

            double result = maintenance;
            switch (goal)
            {
                case CalorieGoal.Lose:
                    result -= 400;
                    break;
                case CalorieGoal.Gain:
                    result += 400;
                    break;
            }

            const double minSafeCalories = 1200;
            if (result < minSafeCalories)
                result = minSafeCalories;

            return result;
        }
    }

}
