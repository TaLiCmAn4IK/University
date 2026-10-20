namespace ООП_2
{
    partial class SearchResultsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ListBox listBoxResults;
        private System.Windows.Forms.TextBox textDetails;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.GroupBox groupBoxActions;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.listBoxResults = new System.Windows.Forms.ListBox();
            this.textDetails = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelCount = new System.Windows.Forms.Label();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.groupBoxActions = new System.Windows.Forms.GroupBox();
            this.groupBoxResults.SuspendLayout();
            this.groupBoxDetails.SuspendLayout();
            this.groupBoxActions.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Результаты поиска";
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            // groupBoxResults
            this.groupBoxResults.Text = "Найденные элементы";
            this.groupBoxResults.Location = new System.Drawing.Point(12, 12);
            this.groupBoxResults.Size = new System.Drawing.Size(576, 200);
            this.groupBoxResults.TabIndex = 0;

            // listBoxResults
            this.listBoxResults.Location = new System.Drawing.Point(6, 22);
            this.listBoxResults.Size = new System.Drawing.Size(564, 160);
            this.listBoxResults.TabIndex = 0;
            this.listBoxResults.SelectedIndexChanged += new System.EventHandler(this.listBoxResults_SelectedIndexChanged);

            // groupBoxDetails
            this.groupBoxDetails.Text = "Детали";
            this.groupBoxDetails.Location = new System.Drawing.Point(12, 218);
            this.groupBoxDetails.Size = new System.Drawing.Size(576, 150);
            this.groupBoxDetails.TabIndex = 1;

            // textDetails
            this.textDetails.Location = new System.Drawing.Point(6, 22);
            this.textDetails.Size = new System.Drawing.Size(564, 23);
            this.textDetails.Multiline = true;
            this.textDetails.Height = 110;
            this.textDetails.ReadOnly = true;
            this.textDetails.TabIndex = 0;

            // groupBoxActions
            this.groupBoxActions.Text = "Действия";
            this.groupBoxActions.Location = new System.Drawing.Point(12, 374);
            this.groupBoxActions.Size = new System.Drawing.Size(576, 80);
            this.groupBoxActions.TabIndex = 2;

            // buttonSave
            this.buttonSave.Text = "Сохранить результаты";
            this.buttonSave.Location = new System.Drawing.Point(150, 25);
            this.buttonSave.Size = new System.Drawing.Size(130, 30);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);

            // buttonClose
            this.buttonClose.Text = "Закрыть";
            this.buttonClose.Location = new System.Drawing.Point(290, 25);
            this.buttonClose.Size = new System.Drawing.Size(100, 30);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);

            // labelCount
            this.labelCount.Text = "Найдено: 0";
            this.labelCount.Location = new System.Drawing.Point(12, 420);
            this.labelCount.Size = new System.Drawing.Size(200, 23);
            this.labelCount.TabIndex = 3;

            // Добавляем элементы
            this.groupBoxResults.Controls.Add(this.listBoxResults);
            this.groupBoxDetails.Controls.Add(this.textDetails);
            this.groupBoxActions.Controls.Add(this.buttonSave);
            this.groupBoxActions.Controls.Add(this.buttonClose);

            this.Controls.Add(this.groupBoxResults);
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.groupBoxActions);
            this.Controls.Add(this.labelCount);

            this.groupBoxResults.ResumeLayout(false);
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            this.groupBoxActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}