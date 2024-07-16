namespace ServerApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Button startButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            listBox1 = new ListBox();
            portTextBox = new TextBox();
            startButton = new Button();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(10, 80);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(680, 319);
            listBox1.TabIndex = 0;
            // 
            // portTextBox
            // 
            portTextBox.Location = new Point(10, 10);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(100, 23);
            portTextBox.TabIndex = 1;
            // 
            // startButton
            // 
            startButton.Location = new Point(120, 10);
            startButton.Name = "startButton";
            startButton.Size = new Size(100, 23);
            startButton.TabIndex = 2;
            startButton.Text = "Iniciar Servidor";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += startButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 422);
            Controls.Add(startButton);
            Controls.Add(portTextBox);
            Controls.Add(listBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Servidor De Microsoft Profesional Plus Ultra Max 14 Pro Amoled 4K Super Sayayin Fase 4 Aguanta Warzones";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(portTextBox.Text, out int port))
            {
                Thread serverThread = new Thread(() => StartServer(port));
                serverThread.Start();
                startButton.Enabled = false;
                portTextBox.Enabled = false;
            }
            else
            {
                MessageBox.Show("No seas burraaaaa, pon bien el puershto.");
            }
        }
    }
}
