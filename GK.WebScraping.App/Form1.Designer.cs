using System;

namespace GK.WebScraping.App
{
    partial class Form1
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.txtMaxPrice = new System.Windows.Forms.TextBox();
            this.txtRunCount = new System.Windows.Forms.TextBox();
            this.txtWaitSeconds = new System.Windows.Forms.TextBox();
            this.txtRunHours = new System.Windows.Forms.TextBox();
            this.txtMinPrice = new System.Windows.Forms.TextBox();
            this.cbSoundSiren = new System.Windows.Forms.CheckBox();
            this.cbAutoBuy = new System.Windows.Forms.CheckBox();
            this.cbAudioWarning = new System.Windows.Forms.CheckBox();
            this.cbOnlyInStock = new System.Windows.Forms.CheckBox();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.lblMaxPrice = new System.Windows.Forms.Label();
            this.lblMinPrice = new System.Windows.Forms.Label();
            this.lblRunCount2 = new System.Windows.Forms.Label();
            this.lblStopSeconds2 = new System.Windows.Forms.Label();
            this.lblRunCount = new System.Windows.Forms.Label();
            this.lblStopSeconds = new System.Windows.Forms.Label();
            this.lblRunHours2 = new System.Windows.Forms.Label();
            this.lblRunHours = new System.Windows.Forms.Label();
            this.lblKeyword = new System.Windows.Forms.Label();
            this.lblSelectStores = new System.Windows.Forms.Label();
            this.cblSelectStores = new System.Windows.Forms.CheckedListBox();
            this.rtxtConsole = new System.Windows.Forms.RichTextBox();
            this.gwProducts = new System.Windows.Forms.DataGridView();
            this.colProductID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitsInStock = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLink = new System.Windows.Forms.DataGridViewLinkColumn();
            this.gbOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gwProducts)).BeginInit();
            this.SuspendLayout();
            // 
            // btnReset
            // 
            this.btnReset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReset.BackColor = System.Drawing.SystemColors.Menu;
            this.btnReset.Location = new System.Drawing.Point(297, 191);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(82, 28);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Clear";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnStart
            // 
            this.btnStart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStart.Location = new System.Drawing.Point(199, 191);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(92, 28);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.txtMaxPrice);
            this.gbOptions.Controls.Add(this.txtRunCount);
            this.gbOptions.Controls.Add(this.txtWaitSeconds);
            this.gbOptions.Controls.Add(this.txtRunHours);
            this.gbOptions.Controls.Add(this.txtMinPrice);
            this.gbOptions.Controls.Add(this.cbSoundSiren);
            this.gbOptions.Controls.Add(this.cbAutoBuy);
            this.gbOptions.Controls.Add(this.cbAudioWarning);
            this.gbOptions.Controls.Add(this.cbOnlyInStock);
            this.gbOptions.Controls.Add(this.btnReset);
            this.gbOptions.Controls.Add(this.btnStart);
            this.gbOptions.Controls.Add(this.txtKeyword);
            this.gbOptions.Controls.Add(this.lblMaxPrice);
            this.gbOptions.Controls.Add(this.lblMinPrice);
            this.gbOptions.Controls.Add(this.lblRunCount2);
            this.gbOptions.Controls.Add(this.lblStopSeconds2);
            this.gbOptions.Controls.Add(this.lblRunCount);
            this.gbOptions.Controls.Add(this.lblStopSeconds);
            this.gbOptions.Controls.Add(this.lblRunHours2);
            this.gbOptions.Controls.Add(this.lblRunHours);
            this.gbOptions.Controls.Add(this.lblKeyword);
            this.gbOptions.Controls.Add(this.lblSelectStores);
            this.gbOptions.Controls.Add(this.cblSelectStores);
            this.gbOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbOptions.Location = new System.Drawing.Point(20, 20);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(998, 238);
            this.gbOptions.TabIndex = 3;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // txtMaxPrice
            // 
            this.txtMaxPrice.Location = new System.Drawing.Point(700, 47);
            this.txtMaxPrice.Name = "txtMaxPrice";
            this.txtMaxPrice.Size = new System.Drawing.Size(80, 20);
            this.txtMaxPrice.TabIndex = 5;
            this.txtMaxPrice.Text = "999999";
            // 
            // txtRunCount
            // 
            this.txtRunCount.Location = new System.Drawing.Point(251, 129);
            this.txtRunCount.Name = "txtRunCount";
            this.txtRunCount.Size = new System.Drawing.Size(32, 20);
            this.txtRunCount.TabIndex = 5;
            this.txtRunCount.Text = "1";
            // 
            // txtWaitSeconds
            // 
            this.txtWaitSeconds.BackColor = System.Drawing.Color.White;
            this.txtWaitSeconds.Location = new System.Drawing.Point(243, 103);
            this.txtWaitSeconds.Name = "txtWaitSeconds";
            this.txtWaitSeconds.Size = new System.Drawing.Size(32, 20);
            this.txtWaitSeconds.TabIndex = 5;
            this.txtWaitSeconds.Text = "0";
            // 
            // txtRunHours
            // 
            this.txtRunHours.Location = new System.Drawing.Point(269, 79);
            this.txtRunHours.Name = "txtRunHours";
            this.txtRunHours.Size = new System.Drawing.Size(32, 20);
            this.txtRunHours.TabIndex = 5;
            this.txtRunHours.Text = "0";
            // 
            // txtMinPrice
            // 
            this.txtMinPrice.Location = new System.Drawing.Point(597, 47);
            this.txtMinPrice.Name = "txtMinPrice";
            this.txtMinPrice.Size = new System.Drawing.Size(80, 20);
            this.txtMinPrice.TabIndex = 5;
            this.txtMinPrice.Text = "0";
            // 
            // cbSoundSiren
            // 
            this.cbSoundSiren.AutoSize = true;
            this.cbSoundSiren.Checked = true;
            this.cbSoundSiren.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSoundSiren.Location = new System.Drawing.Point(396, 119);
            this.cbSoundSiren.Name = "cbSoundSiren";
            this.cbSoundSiren.Size = new System.Drawing.Size(185, 17);
            this.cbSoundSiren.TabIndex = 4;
            this.cbSoundSiren.Text = "Honk the horn when item is found";
            this.cbSoundSiren.UseVisualStyleBackColor = true;
            // 
            // cbAutoBuy
            // 
            this.cbAutoBuy.AutoSize = true;
            this.cbAutoBuy.Checked = true;
            this.cbAutoBuy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoBuy.Location = new System.Drawing.Point(396, 96);
            this.cbAutoBuy.Name = "cbAutoBuy";
            this.cbAutoBuy.Size = new System.Drawing.Size(120, 17);
            this.cbAutoBuy.TabIndex = 4;
            this.cbAutoBuy.Text = "Buy automatically (!)";
            this.cbAutoBuy.UseVisualStyleBackColor = true;
            // 
            // cbAudioWarning
            // 
            this.cbAudioWarning.AutoSize = true;
            this.cbAudioWarning.Checked = true;
            this.cbAudioWarning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAudioWarning.Location = new System.Drawing.Point(396, 73);
            this.cbAudioWarning.Name = "cbAudioWarning";
            this.cbAudioWarning.Size = new System.Drawing.Size(93, 17);
            this.cbAudioWarning.TabIndex = 4;
            this.cbAudioWarning.Text = "Audio warning";
            this.cbAudioWarning.UseVisualStyleBackColor = true;
            // 
            // cbOnlyInStock
            // 
            this.cbOnlyInStock.AutoSize = true;
            this.cbOnlyInStock.Checked = true;
            this.cbOnlyInStock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOnlyInStock.Location = new System.Drawing.Point(396, 50);
            this.cbOnlyInStock.Name = "cbOnlyInStock";
            this.cbOnlyInStock.Size = new System.Drawing.Size(87, 17);
            this.cbOnlyInStock.TabIndex = 4;
            this.cbOnlyInStock.Text = "Only in stock";
            this.cbOnlyInStock.UseVisualStyleBackColor = true;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(196, 50);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(183, 20);
            this.txtKeyword.TabIndex = 2;
            // 
            // lblMaxPrice
            // 
            this.lblMaxPrice.AutoSize = true;
            this.lblMaxPrice.Location = new System.Drawing.Point(682, 31);
            this.lblMaxPrice.Name = "lblMaxPrice";
            this.lblMaxPrice.Size = new System.Drawing.Size(77, 13);
            this.lblMaxPrice.TabIndex = 1;
            this.lblMaxPrice.Text = "Maximum price";
            // 
            // lblMinPrice
            // 
            this.lblMinPrice.AutoSize = true;
            this.lblMinPrice.Location = new System.Drawing.Point(602, 31);
            this.lblMinPrice.Name = "lblMinPrice";
            this.lblMinPrice.Size = new System.Drawing.Size(74, 13);
            this.lblMinPrice.TabIndex = 1;
            this.lblMinPrice.Text = "Minimum price";
            // 
            // lblRunCount2
            // 
            this.lblRunCount2.AutoSize = true;
            this.lblRunCount2.Location = new System.Drawing.Point(285, 132);
            this.lblRunCount2.Name = "lblRunCount2";
            this.lblRunCount2.Size = new System.Drawing.Size(31, 13);
            this.lblRunCount2.TabIndex = 1;
            this.lblRunCount2.Text = "times";
            // 
            // lblStopSeconds2
            // 
            this.lblStopSeconds2.AutoSize = true;
            this.lblStopSeconds2.Location = new System.Drawing.Point(277, 106);
            this.lblStopSeconds2.Name = "lblStopSeconds2";
            this.lblStopSeconds2.Size = new System.Drawing.Size(102, 13);
            this.lblStopSeconds2.TabIndex = 1;
            this.lblStopSeconds2.Text = "seconds in between";
            // 
            // lblRunCount
            // 
            this.lblRunCount.AutoSize = true;
            this.lblRunCount.Location = new System.Drawing.Point(196, 132);
            this.lblRunCount.Name = "lblRunCount";
            this.lblRunCount.Size = new System.Drawing.Size(56, 13);
            this.lblRunCount.TabIndex = 1;
            this.lblRunCount.Text = "Run query";
            // 
            // lblStopSeconds
            // 
            this.lblStopSeconds.AutoSize = true;
            this.lblStopSeconds.Location = new System.Drawing.Point(196, 106);
            this.lblStopSeconds.Name = "lblStopSeconds";
            this.lblStopSeconds.Size = new System.Drawing.Size(44, 13);
            this.lblStopSeconds.TabIndex = 1;
            this.lblStopSeconds.Text = "Wait for";
            // 
            // lblRunHours2
            // 
            this.lblRunHours2.AutoSize = true;
            this.lblRunHours2.Location = new System.Drawing.Point(305, 82);
            this.lblRunHours2.Name = "lblRunHours2";
            this.lblRunHours2.Size = new System.Drawing.Size(43, 13);
            this.lblRunHours2.TabIndex = 1;
            this.lblRunHours2.Text = "minutes";
            // 
            // lblRunHours
            // 
            this.lblRunHours.AutoSize = true;
            this.lblRunHours.Location = new System.Drawing.Point(196, 82);
            this.lblRunHours.Name = "lblRunHours";
            this.lblRunHours.Size = new System.Drawing.Size(71, 13);
            this.lblRunHours.TabIndex = 1;
            this.lblRunHours.Text = "Run query for";
            // 
            // lblKeyword
            // 
            this.lblKeyword.AutoSize = true;
            this.lblKeyword.Location = new System.Drawing.Point(193, 31);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(87, 13);
            this.lblKeyword.TabIndex = 1;
            this.lblKeyword.Text = "Product keyword";
            // 
            // lblSelectStores
            // 
            this.lblSelectStores.AutoSize = true;
            this.lblSelectStores.Location = new System.Drawing.Point(7, 31);
            this.lblSelectStores.Name = "lblSelectStores";
            this.lblSelectStores.Size = new System.Drawing.Size(159, 13);
            this.lblSelectStores.TabIndex = 1;
            this.lblSelectStores.Text = "Select stores you want to check";
            // 
            // cblSelectStores
            // 
            this.cblSelectStores.FormattingEnabled = true;
            this.cblSelectStores.Items.AddRange(new object[] {
            "Select all"});
            this.cblSelectStores.Location = new System.Drawing.Point(7, 50);
            this.cblSelectStores.Name = "cblSelectStores";
            this.cblSelectStores.Size = new System.Drawing.Size(183, 169);
            this.cblSelectStores.TabIndex = 0;
            this.cblSelectStores.SelectedIndexChanged += new System.EventHandler(this.cblSelectStores_SelectedIndexChanged);
            // 
            // rtxtConsole
            // 
            this.rtxtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.rtxtConsole.BackColor = System.Drawing.Color.Black;
            this.rtxtConsole.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.rtxtConsole.Location = new System.Drawing.Point(20, 264);
            this.rtxtConsole.Name = "rtxtConsole";
            this.rtxtConsole.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtConsole.Size = new System.Drawing.Size(360, 684);
            this.rtxtConsole.TabIndex = 4;
            this.rtxtConsole.Text = "";
            // 
            // gwProducts
            // 
            this.gwProducts.AllowUserToAddRows = false;
            this.gwProducts.AllowUserToOrderColumns = true;
            this.gwProducts.BackgroundColor = System.Drawing.Color.White;
            this.gwProducts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gwProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gwProducts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProductID,
            this.colName,
            this.colUnitsInStock,
            this.colUnitPrice,
            this.colLink});
            this.gwProducts.Dock = System.Windows.Forms.DockStyle.Right;
            this.gwProducts.GridColor = System.Drawing.SystemColors.ControlLight;
            this.gwProducts.Location = new System.Drawing.Point(389, 258);
            this.gwProducts.Name = "gwProducts";
            this.gwProducts.ReadOnly = true;
            this.gwProducts.Size = new System.Drawing.Size(629, 684);
            this.gwProducts.TabIndex = 5;
            // 
            // colProductID
            // 
            this.colProductID.HeaderText = "ProductID";
            this.colProductID.Name = "colProductID";
            this.colProductID.ReadOnly = true;
            // 
            // colName
            // 
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 200;
            // 
            // colUnitsInStock
            // 
            this.colUnitsInStock.HeaderText = "Units in stock";
            this.colUnitsInStock.Name = "colUnitsInStock";
            this.colUnitsInStock.ReadOnly = true;
            // 
            // colUnitPrice
            // 
            this.colUnitPrice.HeaderText = "Unit price";
            this.colUnitPrice.Name = "colUnitPrice";
            this.colUnitPrice.ReadOnly = true;
            // 
            // colLink
            // 
            this.colLink.HeaderText = "Link";
            this.colLink.Name = "colLink";
            this.colLink.ReadOnly = true;
            this.colLink.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colLink.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colLink.Width = 200;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 962);
            this.Controls.Add(this.gwProducts);
            this.Controls.Add(this.rtxtConsole);
            this.Controls.Add(this.gbOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Text = "GK Stock Sniper";
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gwProducts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.CheckBox cbOnlyInStock;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.Label lblKeyword;
        private System.Windows.Forms.Label lblSelectStores;
        private System.Windows.Forms.CheckedListBox cblSelectStores;
        private System.Windows.Forms.RichTextBox rtxtConsole;
        private System.Windows.Forms.DataGridView gwProducts;
        private System.Windows.Forms.CheckBox cbAudioWarning;
        private System.Windows.Forms.TextBox txtMaxPrice;
        private System.Windows.Forms.TextBox txtWaitSeconds;
        private System.Windows.Forms.TextBox txtRunHours;
        private System.Windows.Forms.TextBox txtMinPrice;
        private System.Windows.Forms.CheckBox cbSoundSiren;
        private System.Windows.Forms.CheckBox cbAutoBuy;
        private System.Windows.Forms.Label lblMaxPrice;
        private System.Windows.Forms.Label lblMinPrice;
        private System.Windows.Forms.Label lblStopSeconds2;
        private System.Windows.Forms.Label lblStopSeconds;
        private System.Windows.Forms.Label lblRunHours2;
        private System.Windows.Forms.Label lblRunHours;
        private System.Windows.Forms.TextBox txtRunCount;
        private System.Windows.Forms.Label lblRunCount2;
        private System.Windows.Forms.Label lblRunCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitsInStock;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitPrice;
        private System.Windows.Forms.DataGridViewLinkColumn colLink;
    }
}

