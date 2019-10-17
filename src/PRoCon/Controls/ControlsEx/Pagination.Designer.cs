namespace PRoCon.Controls.ControlsEx {
    partial class Pagination {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.First = new System.Windows.Forms.Button();
            this.Previous = new System.Windows.Forms.Button();
            this.Page = new System.Windows.Forms.Label();
            this.Next = new System.Windows.Forms.Button();
            this.Last = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // First
            // 
            this.First.Location = new System.Drawing.Point(0, 0);
            this.First.Name = "First";
            this.First.Size = new System.Drawing.Size(35, 23);
            this.First.TabIndex = 0;
            this.First.Text = "<<";
            this.First.UseVisualStyleBackColor = true;
            this.First.Click += new System.EventHandler(this.First_Click);
            // 
            // Previous
            // 
            this.Previous.Location = new System.Drawing.Point(42, 0);
            this.Previous.Name = "Previous";
            this.Previous.Size = new System.Drawing.Size(35, 23);
            this.Previous.TabIndex = 1;
            this.Previous.Text = "<";
            this.Previous.UseVisualStyleBackColor = true;
            this.Previous.Click += new System.EventHandler(this.Previous_Click);
            // 
            // Page
            // 
            this.Page.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Page.Location = new System.Drawing.Point(83, 4);
            this.Page.Name = "Page";
            this.Page.Size = new System.Drawing.Size(81, 15);
            this.Page.TabIndex = 2;
            this.Page.Text = "1 of 100";
            this.Page.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Next
            // 
            this.Next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Next.Location = new System.Drawing.Point(170, 0);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(35, 23);
            this.Next.TabIndex = 3;
            this.Next.Text = ">";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // Last
            // 
            this.Last.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Last.Location = new System.Drawing.Point(212, 0);
            this.Last.Name = "Last";
            this.Last.Size = new System.Drawing.Size(35, 23);
            this.Last.TabIndex = 4;
            this.Last.Text = ">>";
            this.Last.UseVisualStyleBackColor = true;
            this.Last.Click += new System.EventHandler(this.Last_Click);
            // 
            // Pagination
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Last);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.Page);
            this.Controls.Add(this.Previous);
            this.Controls.Add(this.First);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Pagination";
            this.Size = new System.Drawing.Size(253, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button First;
        private System.Windows.Forms.Button Previous;
        private System.Windows.Forms.Label Page;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Button Last;
    }
}
