namespace dummy_server
{
    partial class dummyServer
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.connect_Server = new System.Windows.Forms.Button();
            this.DebugBox = new System.Windows.Forms.RichTextBox();
            this.connectCount = new System.Windows.Forms.TextBox();
            this.MovePos = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // connect_Server
            // 
            this.connect_Server.Location = new System.Drawing.Point(135, 15);
            this.connect_Server.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.connect_Server.Name = "connect_Server";
            this.connect_Server.Size = new System.Drawing.Size(177, 26);
            this.connect_Server.TabIndex = 0;
            this.connect_Server.Text = "연결하기";
            this.connect_Server.UseVisualStyleBackColor = true;
            this.connect_Server.Click += new System.EventHandler(this.connect_Server_Click);
            // 
            // DebugBox
            // 
            this.DebugBox.Location = new System.Drawing.Point(504, 15);
            this.DebugBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DebugBox.Name = "DebugBox";
            this.DebugBox.Size = new System.Drawing.Size(426, 306);
            this.DebugBox.TabIndex = 1;
            this.DebugBox.Text = "";
            // 
            // connectCount
            // 
            this.connectCount.Location = new System.Drawing.Point(14, 15);
            this.connectCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.connectCount.Name = "connectCount";
            this.connectCount.Size = new System.Drawing.Size(114, 25);
            this.connectCount.TabIndex = 2;
            this.connectCount.Text = "30";
            this.connectCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MovePos
            // 
            this.MovePos.Location = new System.Drawing.Point(14, 48);
            this.MovePos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MovePos.Name = "MovePos";
            this.MovePos.Size = new System.Drawing.Size(114, 25);
            this.MovePos.TabIndex = 3;
            this.MovePos.Text = "10";
            this.MovePos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dummyServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 411);
            this.Controls.Add(this.MovePos);
            this.Controls.Add(this.connectCount);
            this.Controls.Add(this.DebugBox);
            this.Controls.Add(this.connect_Server);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "dummyServer";
            this.Text = "Dummy Server";
            this.Load += new System.EventHandler(this.dummyServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connect_Server;
        private System.Windows.Forms.RichTextBox DebugBox;
        private System.Windows.Forms.TextBox connectCount;
        private System.Windows.Forms.TextBox MovePos;
    }
}

