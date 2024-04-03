
namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_pause = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.btn_get = new System.Windows.Forms.Button();
            this.txt_addlink = new System.Windows.Forms.TextBox();
            this.btn_detect_face = new System.Windows.Forms.Button();
            this.btn_addpsn = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnTrain = new System.Windows.Forms.Button();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.btn_log = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_themThe = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_pause
            // 
            this.btn_pause.Location = new System.Drawing.Point(117, 218);
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(99, 23);
            this.btn_pause.TabIndex = 1;
            this.btn_pause.Text = "Pause";
            this.btn_pause.UseVisualStyleBackColor = true;
            this.btn_pause.Click += new System.EventHandler(this.btn_pause_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.Location = new System.Drawing.Point(222, 218);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(99, 23);
            this.btn_stop.TabIndex = 1;
            this.btn_stop.Text = "Stop";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // btn_get
            // 
            this.btn_get.Location = new System.Drawing.Point(12, 218);
            this.btn_get.Name = "btn_get";
            this.btn_get.Size = new System.Drawing.Size(99, 23);
            this.btn_get.TabIndex = 1;
            this.btn_get.Text = "Get";
            this.btn_get.UseVisualStyleBackColor = true;
            this.btn_get.Click += new System.EventHandler(this.btn_get_Click);
            // 
            // txt_addlink
            // 
            this.txt_addlink.Location = new System.Drawing.Point(12, 247);
            this.txt_addlink.Name = "txt_addlink";
            this.txt_addlink.Size = new System.Drawing.Size(328, 20);
            this.txt_addlink.TabIndex = 2;
            // 
            // btn_detect_face
            // 
            this.btn_detect_face.Location = new System.Drawing.Point(347, 12);
            this.btn_detect_face.Name = "btn_detect_face";
            this.btn_detect_face.Size = new System.Drawing.Size(108, 23);
            this.btn_detect_face.TabIndex = 1;
            this.btn_detect_face.Text = "Detect face";
            this.btn_detect_face.UseVisualStyleBackColor = true;
            this.btn_detect_face.Click += new System.EventHandler(this.btn_detect_face_Click);
            // 
            // btn_addpsn
            // 
            this.btn_addpsn.Location = new System.Drawing.Point(461, 12);
            this.btn_addpsn.Name = "btn_addpsn";
            this.btn_addpsn.Size = new System.Drawing.Size(108, 23);
            this.btn_addpsn.TabIndex = 1;
            this.btn_addpsn.Text = "Add persons";
            this.btn_addpsn.UseVisualStyleBackColor = true;
            this.btn_addpsn.Click += new System.EventHandler(this.btn_addpsn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(351, 92);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(108, 97);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(328, 200);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // btnTrain
            // 
            this.btnTrain.Location = new System.Drawing.Point(465, 92);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(104, 23);
            this.btnTrain.TabIndex = 1;
            this.btnTrain.Text = "Train Image";
            this.btnTrain.UseVisualStyleBackColor = true;
            this.btnTrain.Click += new System.EventHandler(this.btnTrain_Click);
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(443, 41);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(126, 20);
            this.txt_name.TabIndex = 6;
            // 
            // btn_log
            // 
            this.btn_log.Location = new System.Drawing.Point(465, 121);
            this.btn_log.Name = "btn_log";
            this.btn_log.Size = new System.Drawing.Size(104, 23);
            this.btn_log.TabIndex = 7;
            this.btn_log.Text = "History Loggin";
            this.btn_log.UseVisualStyleBackColor = true;
            this.btn_log.Click += new System.EventHandler(this.btn_log_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(348, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Đặt tên mặt:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(348, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Đặt tên thẻ:";
            this.label2.Click += new System.EventHandler(this.label1_Click);
            // 
            // txt_themThe
            // 
            this.txt_themThe.Location = new System.Drawing.Point(435, 66);
            this.txt_themThe.Name = "txt_themThe";
            this.txt_themThe.Size = new System.Drawing.Size(134, 20);
            this.txt_themThe.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 296);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_log);
            this.Controls.Add(this.txt_themThe);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.txt_addlink);
            this.Controls.Add(this.btn_stop);
            this.Controls.Add(this.btnTrain);
            this.Controls.Add(this.btn_addpsn);
            this.Controls.Add(this.btn_detect_face);
            this.Controls.Add(this.btn_get);
            this.Controls.Add(this.btn_pause);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Face detect ESP32_CAM";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_pause;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Button btn_get;
        private System.Windows.Forms.TextBox txt_addlink;
        private System.Windows.Forms.Button btn_detect_face;
        private System.Windows.Forms.Button btn_addpsn;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnTrain;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.Button btn_log;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_themThe;
    }
}

