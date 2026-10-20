namespace ООП_1
{
    public partial class MainForm : Form
    {
        private readonly ICalorieCalculator _calculator = new CalorieCalculator();

        public MainForm()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Пол
            genderComboBox.Items.Add("Мужчина");
            genderComboBox.Items.Add("Женщина");
            genderComboBox.Items.Add("Женщина_1");
            genderComboBox.SelectedIndex = 0;

            // Цель
            goalComboBox.Items.Add("Поддержание веса");
            goalComboBox.Items.Add("Похудение");
            goalComboBox.Items.Add("Набор веса");
            goalComboBox.SelectedIndex = 0;

            bmiLabel.Text = "-";
            caloriesLabel.Text = "-";
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            try
            {
                double weight = double.Parse(weightTextBox.Text);
                double height = double.Parse(heightTextBox.Text);
                int age = int.Parse(ageTextBox.Text);

                bool isMale = genderComboBox.SelectedItem.ToString() == "Мужчина";

                CalorieGoal goal;
                switch (goalComboBox.SelectedIndex)
                {
                    case 1:
                        goal = CalorieGoal.Lose;
                        break;
                    case 2:
                        goal = CalorieGoal.Gain;
                        break;
                    default:
                        goal = CalorieGoal.Maintain;
                        break;
                }

                double bmi = _calculator.CalculateBmi(weight, height);
                string category = _calculator.GetBmiCategory(bmi);
                double calories = _calculator.CalculateDailyCalories(
                    weight, height, age, isMale, goal);

                bmiLabel.Text = $"{bmi:F1} ({category})";
                caloriesLabel.Text = $"{calories:F0} ккал";
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    "Проверьте ввод: вес, рост и возраст должны быть числами.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Неожиданная ошибка: " + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void genderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
