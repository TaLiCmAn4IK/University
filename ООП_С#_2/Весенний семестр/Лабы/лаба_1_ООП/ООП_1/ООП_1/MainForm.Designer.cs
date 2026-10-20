namespace ООП_1
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            genderTitleLabel = new Label();
            ageTitleLabel = new Label();
            goalTitleLabel = new Label();
            bmiTitleLabel = new Label();
            caloriesTitleLabel = new Label();
            heightTitleLabel = new Label();
            weightTitleLabel = new Label();
            heightTextBox = new TextBox();
            weightTextBox = new TextBox();
            ageTextBox = new TextBox();
            genderComboBox = new ComboBox();
            calculateButton = new Button();
            goalComboBox = new ComboBox();
            bmiLabel = new Label();
            caloriesLabel = new Label();
            SuspendLayout();
            // 
            // genderTitleLabel
            // 
            genderTitleLabel.AutoSize = true;
            genderTitleLabel.ForeColor = Color.Red;
            genderTitleLabel.Location = new Point(30, 32);
            genderTitleLabel.Name = "genderTitleLabel";
            genderTitleLabel.Size = new Size(40, 20);
            genderTitleLabel.TabIndex = 0;
            genderTitleLabel.Text = "Пол:";
            genderTitleLabel.Click += label1_Click;
            // 
            // ageTitleLabel
            // 
            ageTitleLabel.AutoSize = true;
            ageTitleLabel.Location = new Point(30, 180);
            ageTitleLabel.Name = "ageTitleLabel";
            ageTitleLabel.Size = new Size(67, 20);
            ageTitleLabel.TabIndex = 1;
            ageTitleLabel.Text = "Возраст:";
            // 
            // goalTitleLabel
            // 
            goalTitleLabel.AutoSize = true;
            goalTitleLabel.Location = new Point(30, 230);
            goalTitleLabel.Name = "goalTitleLabel";
            goalTitleLabel.Size = new Size(47, 20);
            goalTitleLabel.TabIndex = 2;
            goalTitleLabel.Text = "Цель:";
            // 
            // bmiTitleLabel
            // 
            bmiTitleLabel.AutoSize = true;
            bmiTitleLabel.Location = new Point(30, 331);
            bmiTitleLabel.Name = "bmiTitleLabel";
            bmiTitleLabel.Size = new Size(44, 20);
            bmiTitleLabel.TabIndex = 3;
            bmiTitleLabel.Text = "ИМТ:";
            bmiTitleLabel.Click += label4_Click;
            // 
            // caloriesTitleLabel
            // 
            caloriesTitleLabel.AutoSize = true;
            caloriesTitleLabel.Location = new Point(30, 368);
            caloriesTitleLabel.Name = "caloriesTitleLabel";
            caloriesTitleLabel.Size = new Size(126, 20);
            caloriesTitleLabel.TabIndex = 4;
            caloriesTitleLabel.Text = "Суточная норма:";
            caloriesTitleLabel.Click += label5_Click;
            // 
            // heightTitleLabel
            // 
            heightTitleLabel.AutoSize = true;
            heightTitleLabel.Location = new Point(30, 130);
            heightTitleLabel.Name = "heightTitleLabel";
            heightTitleLabel.Size = new Size(74, 20);
            heightTitleLabel.TabIndex = 5;
            heightTitleLabel.Text = "Рост (см):";
            // 
            // weightTitleLabel
            // 
            weightTitleLabel.AutoSize = true;
            weightTitleLabel.Location = new Point(30, 82);
            weightTitleLabel.Name = "weightTitleLabel";
            weightTitleLabel.Size = new Size(63, 20);
            weightTitleLabel.TabIndex = 6;
            weightTitleLabel.Text = "Вес (кг):";
            weightTitleLabel.Click += label7_Click;
            // 
            // heightTextBox
            // 
            heightTextBox.Location = new Point(137, 130);
            heightTextBox.Name = "heightTextBox";
            heightTextBox.Size = new Size(125, 27);
            heightTextBox.TabIndex = 7;
            // 
            // weightTextBox
            // 
            weightTextBox.Location = new Point(137, 79);
            weightTextBox.Name = "weightTextBox";
            weightTextBox.Size = new Size(125, 27);
            weightTextBox.TabIndex = 8;
            // 
            // ageTextBox
            // 
            ageTextBox.Location = new Point(137, 177);
            ageTextBox.Name = "ageTextBox";
            ageTextBox.Size = new Size(125, 27);
            ageTextBox.TabIndex = 9;
            // 
            // genderComboBox
            // 
            genderComboBox.FormattingEnabled = true;
            genderComboBox.Location = new Point(137, 29);
            genderComboBox.Name = "genderComboBox";
            genderComboBox.Size = new Size(151, 28);
            genderComboBox.TabIndex = 11;
            genderComboBox.SelectedIndexChanged += genderComboBox_SelectedIndexChanged;
            // 
            // calculateButton
            // 
            calculateButton.Location = new Point(30, 279);
            calculateButton.Name = "calculateButton";
            calculateButton.Size = new Size(94, 29);
            calculateButton.TabIndex = 12;
            calculateButton.Text = "Рассчитать";
            calculateButton.UseVisualStyleBackColor = true;
            calculateButton.Click += calculateButton_Click;
            // 
            // goalComboBox
            // 
            goalComboBox.FormattingEnabled = true;
            goalComboBox.Location = new Point(137, 227);
            goalComboBox.Name = "goalComboBox";
            goalComboBox.Size = new Size(151, 28);
            goalComboBox.TabIndex = 13;
            // 
            // bmiLabel
            // 
            bmiLabel.AutoSize = true;
            bmiLabel.Location = new Point(80, 331);
            bmiLabel.Name = "bmiLabel";
            bmiLabel.Size = new Size(15, 20);
            bmiLabel.TabIndex = 14;
            bmiLabel.Text = "-";
            // 
            // caloriesLabel
            // 
            caloriesLabel.AutoSize = true;
            caloriesLabel.Location = new Point(162, 368);
            caloriesLabel.Name = "caloriesLabel";
            caloriesLabel.Size = new Size(15, 20);
            caloriesLabel.TabIndex = 15;
            caloriesLabel.Text = "-";
            caloriesLabel.Click += label9_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(caloriesLabel);
            Controls.Add(bmiLabel);
            Controls.Add(goalComboBox);
            Controls.Add(calculateButton);
            Controls.Add(genderComboBox);
            Controls.Add(ageTextBox);
            Controls.Add(weightTextBox);
            Controls.Add(heightTextBox);
            Controls.Add(weightTitleLabel);
            Controls.Add(heightTitleLabel);
            Controls.Add(caloriesTitleLabel);
            Controls.Add(bmiTitleLabel);
            Controls.Add(goalTitleLabel);
            Controls.Add(ageTitleLabel);
            Controls.Add(genderTitleLabel);
            Name = "MainForm";
            Text = "Form1";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label genderTitleLabel;
        private Label ageTitleLabel;
        private Label goalTitleLabel;
        private Label bmiTitleLabel;
        private Label caloriesTitleLabel;
        private Label heightTitleLabel;
        private Label weightTitleLabel;
        private TextBox heightTextBox;
        private TextBox weightTextBox;
        private TextBox ageTextBox;
        private ComboBox genderComboBox;
        private Button calculateButton;
        private ComboBox goalComboBox;
        private Label bmiLabel;
        private Label caloriesLabel;
    }
}
