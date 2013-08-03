namespace TestApplication
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
            this.label1 = new System.Windows.Forms.Label();
            this._logsLocationTxt = new System.Windows.Forms.TextBox();
            this._logRequestsChk = new System.Windows.Forms.CheckBox();
            this._logErrorsChk = new System.Windows.Forms.CheckBox();
            this._logInformationChk = new System.Windows.Forms.CheckBox();
            this._logDebugChk = new System.Windows.Forms.CheckBox();
            this._eventLogChk = new System.Windows.Forms.CheckBox();
            this._proxyIPTxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._proxyPortsTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._enablePerfmonChk = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._maxQueueLengthCtrl = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this._cacheContentChk = new System.Windows.Forms.CheckBox();
            this._supportsGzipChk = new System.Windows.Forms.CheckBox();
            this._proxiedServerTxt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._startBtn = new System.Windows.Forms.Button();
            this._installCounterBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._maxQueueLengthCtrl)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Location";
            // 
            // _logsLocationTxt
            // 
            this._logsLocationTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._logsLocationTxt.Location = new System.Drawing.Point(81, 19);
            this._logsLocationTxt.Name = "_logsLocationTxt";
            this._logsLocationTxt.Size = new System.Drawing.Size(493, 20);
            this._logsLocationTxt.TabIndex = 1;
            // 
            // _logRequestsChk
            // 
            this._logRequestsChk.AutoSize = true;
            this._logRequestsChk.Location = new System.Drawing.Point(15, 54);
            this._logRequestsChk.Name = "_logRequestsChk";
            this._logRequestsChk.Size = new System.Drawing.Size(92, 17);
            this._logRequestsChk.TabIndex = 2;
            this._logRequestsChk.Text = "Log Requests";
            this._logRequestsChk.UseVisualStyleBackColor = true;
            // 
            // _logErrorsChk
            // 
            this._logErrorsChk.AutoSize = true;
            this._logErrorsChk.Location = new System.Drawing.Point(113, 54);
            this._logErrorsChk.Name = "_logErrorsChk";
            this._logErrorsChk.Size = new System.Drawing.Size(74, 17);
            this._logErrorsChk.TabIndex = 3;
            this._logErrorsChk.Text = "Log Errors";
            this._logErrorsChk.UseVisualStyleBackColor = true;
            // 
            // _logInformationChk
            // 
            this._logInformationChk.AutoSize = true;
            this._logInformationChk.Location = new System.Drawing.Point(193, 54);
            this._logInformationChk.Name = "_logInformationChk";
            this._logInformationChk.Size = new System.Drawing.Size(99, 17);
            this._logInformationChk.TabIndex = 4;
            this._logInformationChk.Text = "Log Information";
            this._logInformationChk.UseVisualStyleBackColor = true;
            // 
            // _logDebugChk
            // 
            this._logDebugChk.AutoSize = true;
            this._logDebugChk.Location = new System.Drawing.Point(298, 54);
            this._logDebugChk.Name = "_logDebugChk";
            this._logDebugChk.Size = new System.Drawing.Size(79, 17);
            this._logDebugChk.TabIndex = 5;
            this._logDebugChk.Text = "Log Debug";
            this._logDebugChk.UseVisualStyleBackColor = true;
            // 
            // _eventLogChk
            // 
            this._eventLogChk.AutoSize = true;
            this._eventLogChk.Location = new System.Drawing.Point(149, 77);
            this._eventLogChk.Name = "_eventLogChk";
            this._eventLogChk.Size = new System.Drawing.Size(143, 17);
            this._eventLogChk.TabIndex = 6;
            this._eventLogChk.Text = "Errors to Windows Event";
            this._eventLogChk.UseVisualStyleBackColor = true;
            // 
            // _proxyIPTxt
            // 
            this._proxyIPTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._proxyIPTxt.Location = new System.Drawing.Point(81, 19);
            this._proxyIPTxt.Name = "_proxyIPTxt";
            this._proxyIPTxt.Size = new System.Drawing.Size(493, 20);
            this._proxyIPTxt.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "IP";
            // 
            // _proxyPortsTxt
            // 
            this._proxyPortsTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._proxyPortsTxt.Location = new System.Drawing.Point(81, 45);
            this._proxyPortsTxt.Name = "_proxyPortsTxt";
            this._proxyPortsTxt.Size = new System.Drawing.Size(493, 20);
            this._proxyPortsTxt.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Ports";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this._maxQueueLengthCtrl);
            this.groupBox1.Controls.Add(this._proxiedServerTxt);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this._cacheContentChk);
            this.groupBox1.Controls.Add(this._proxyIPTxt);
            this.groupBox1.Controls.Add(this._supportsGzipChk);
            this.groupBox1.Controls.Add(this._proxyPortsTxt);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 130);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(587, 165);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Proxy";
            // 
            // _enablePerfmonChk
            // 
            this._enablePerfmonChk.AutoSize = true;
            this._enablePerfmonChk.Location = new System.Drawing.Point(15, 77);
            this._enablePerfmonChk.Name = "_enablePerfmonChk";
            this._enablePerfmonChk.Size = new System.Drawing.Size(128, 17);
            this._enablePerfmonChk.TabIndex = 16;
            this._enablePerfmonChk.Text = "Enable Perfmon Stats";
            this._enablePerfmonChk.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this._enablePerfmonChk);
            this.groupBox2.Controls.Add(this._logsLocationTxt);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this._logErrorsChk);
            this.groupBox2.Controls.Add(this._eventLogChk);
            this.groupBox2.Controls.Add(this._logRequestsChk);
            this.groupBox2.Controls.Add(this._logDebugChk);
            this.groupBox2.Controls.Add(this._logInformationChk);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(587, 112);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Logging";
            // 
            // _maxQueueLengthCtrl
            // 
            this._maxQueueLengthCtrl.Location = new System.Drawing.Point(113, 94);
            this._maxQueueLengthCtrl.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._maxQueueLengthCtrl.Name = "_maxQueueLengthCtrl";
            this._maxQueueLengthCtrl.Size = new System.Drawing.Size(120, 20);
            this._maxQueueLengthCtrl.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Max Queue Length";
            // 
            // _cacheContentChk
            // 
            this._cacheContentChk.AutoSize = true;
            this._cacheContentChk.Location = new System.Drawing.Point(179, 71);
            this._cacheContentChk.Name = "_cacheContentChk";
            this._cacheContentChk.Size = new System.Drawing.Size(97, 17);
            this._cacheContentChk.TabIndex = 16;
            this._cacheContentChk.Text = "Cache Content";
            this._cacheContentChk.UseVisualStyleBackColor = true;
            // 
            // _supportsGzipChk
            // 
            this._supportsGzipChk.AutoSize = true;
            this._supportsGzipChk.Location = new System.Drawing.Point(81, 71);
            this._supportsGzipChk.Name = "_supportsGzipChk";
            this._supportsGzipChk.Size = new System.Drawing.Size(92, 17);
            this._supportsGzipChk.TabIndex = 15;
            this._supportsGzipChk.Text = "Supports Gzip";
            this._supportsGzipChk.UseVisualStyleBackColor = true;
            // 
            // _proxiedServerTxt
            // 
            this._proxiedServerTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._proxiedServerTxt.Location = new System.Drawing.Point(81, 120);
            this._proxiedServerTxt.Name = "_proxiedServerTxt";
            this._proxiedServerTxt.Size = new System.Drawing.Size(493, 20);
            this._proxiedServerTxt.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Proxied";
            // 
            // _startBtn
            // 
            this._startBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._startBtn.Location = new System.Drawing.Point(524, 312);
            this._startBtn.Name = "_startBtn";
            this._startBtn.Size = new System.Drawing.Size(75, 23);
            this._startBtn.TabIndex = 15;
            this._startBtn.Text = "Start";
            this._startBtn.UseVisualStyleBackColor = true;
            this._startBtn.Click += new System.EventHandler(this._startBtn_Click);
            // 
            // _installCounterBtn
            // 
            this._installCounterBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._installCounterBtn.Location = new System.Drawing.Point(12, 312);
            this._installCounterBtn.Name = "_installCounterBtn";
            this._installCounterBtn.Size = new System.Drawing.Size(109, 23);
            this._installCounterBtn.TabIndex = 16;
            this._installCounterBtn.Text = "Install Counters";
            this._installCounterBtn.UseVisualStyleBackColor = true;
            this._installCounterBtn.Click += new System.EventHandler(this._installCounterBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 347);
            this.Controls.Add(this._installCounterBtn);
            this.Controls.Add(this._startBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Babalu rProxy Test Application";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._maxQueueLengthCtrl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _logsLocationTxt;
        private System.Windows.Forms.CheckBox _logRequestsChk;
        private System.Windows.Forms.CheckBox _logErrorsChk;
        private System.Windows.Forms.CheckBox _logInformationChk;
        private System.Windows.Forms.CheckBox _logDebugChk;
        private System.Windows.Forms.CheckBox _eventLogChk;
        private System.Windows.Forms.TextBox _proxyIPTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _proxyPortsTxt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox _proxiedServerTxt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox _cacheContentChk;
        private System.Windows.Forms.CheckBox _supportsGzipChk;
        private System.Windows.Forms.CheckBox _enablePerfmonChk;
        private System.Windows.Forms.NumericUpDown _maxQueueLengthCtrl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button _startBtn;
        private System.Windows.Forms.Button _installCounterBtn;
    }
}

