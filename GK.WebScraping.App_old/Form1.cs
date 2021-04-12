using GK.WebScraping.Shared.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GK.WebScraping.Utilities;
using GK.WebScraping.Utilities.Thread;

namespace GK.WebScraping.App
{
    public partial class Form1 : Form
    {
        private CommunicationManager _commManager;
        private ProcessThread _processThread;


        public Form1()
        {
            InitializeComponent();
            this._commManager = CommunicationManager.Instance;
            ConsoleAgent.Init(this.rtxtConsole);

            this.gwProducts.CellContentClick += GwProducts_CellContentClick;
            this.cblSelectStores.Items.AddRange(this._commManager.GetAllStores());
            this.UpdateCallbackEvent += this.UpdateCallback;
            Control.CheckForIllegalCrossThreadCalls = false;
            //var store = new object[] {
            //"Inet",
            //"Webhallen",
            //"Computersalg",
            //"Mediamarkt",
            //"NetOnNet",
            //"Komplett",
            //"Dustin",
            //"Elgiganten",
            //"Cdon",
            //"Amazon",
            //"Proshop"};


        }


        private void GwProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.gwProducts.Columns[this.gwProducts.CurrentCell.ColumnIndex].HeaderText == "Link")
            {
                System.Diagnostics.Process.Start(this.gwProducts.CurrentCell.Value.ToString());
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            //HashSet<String> selectedStores = new HashSet<string>();


            if (this.btnStart.Text == "Start")
            {
                this.Start();
            }
            else
                this.Stop();


        }

        private void Stop()
        {
            this.btnStart.Text = "Start";
            this._processThread.Stop();
            this._processThread = null;
        }

        private void Start()
        {

            this.gwProducts.Rows.Clear();


            if (this.cblSelectStores.CheckedItems.Count <= 0)
            {
                ConsoleAgent.Write("You did not select any store.", color: "red");
                return;
            }

            if (String.IsNullOrEmpty(txtKeyword.Text))
            {
                ConsoleAgent.Write("You did not enter any key word for search.", color: "red");
                return;
            }

            ConsoleAgent.Speak("Starting");

            SearchOptions options = this.GetOptions();
            Configurations config = this.GetConfig();

            ConsoleAgent.UpdateConfig(config);


            if (this._processThread == null)
                this._processThread = new GK.WebScraping.Utilities.Thread.ProcessThread(this._commManager, options, this.UpdateCallbackEvent);
            else
                this._processThread.Update(options);


            this._processThread.Start();

            this.btnStart.Text = "Stop";




        }

        private Configurations GetConfig()
        {
            return new Configurations()
            {
                SupportAudioWarning = this.cbAudioWarning.Checked,
            };
        }

        private SearchOptions GetOptions()
        {
            HashSet<String> selectedStores = new HashSet<string>();
            for (int i = 0; i < this.cblSelectStores.CheckedItems.Count; i++)
            {
                String storeName = this.cblSelectStores.CheckedItems[i].ToString();
                if (storeName != "Select all")
                    selectedStores.Add(storeName);
            }



            SearchOptions retval = new SearchOptions()
            {
                Keyword = this.txtKeyword.Text,
                WaitSeconds = Int32.Parse(this.txtWaitSeconds.Text),
                RunHours = Int32.Parse(this.txtRunHours.Text),
                OnlyInStock = this.cbOnlyInStock.Checked,
                SelectedStores = selectedStores,
                MinPrice = Int32.Parse(this.txtMinPrice.Text),
                MaxPrice = Int32.Parse(this.txtMaxPrice.Text)
            };

            if (Int32.TryParse(this.txtRunCount.Text, out Int32 runCount) && runCount > 0)
                retval.RunCount = runCount;

            return retval;

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ConsoleAgent.Clear();
            this.gwProducts.Rows.Clear();
        }

        private void cblSelectStores_SelectedIndexChanged(object sender, EventArgs e)
        {

            Boolean allChecked = false;
            foreach (var item in this.cblSelectStores.CheckedItems)
            {
                if (item.ToString() == "Select all")
                {
                    allChecked = true;
                    break;
                }

            }

            if (allChecked)
            {
                for (int i = 0; i < this.cblSelectStores.Items.Count; i++)
                    this.cblSelectStores.SetItemChecked(i, true);
            }

        }
        public delegate void UpdateEventHandler(object sender, ProcessUpdateType type, params object[] values);
        public event UpdateEventHandler UpdateCallbackEvent;

        private void UpdateCallback(object sender, ProcessUpdateType type, object[] values)
        {

            switch (type)
            {
                case ProcessUpdateType.Found:
                    this.gwProducts.Rows.Add(values);
                    break;
                case ProcessUpdateType.EndingThread:
                    ConsoleAgent.Write(format: "Search job finished.", color: "white", bgcolor: "black", doSpeak: true);
                    ConsoleAgent.Write(format: "Total {0} items were found.", color: "white", bgcolor: "black", args: this.gwProducts.Rows.Count);
                    this.Stop();
                    break;
                case ProcessUpdateType.Error:
                    ConsoleAgent.Write(values[0] as Exception);
                    break;
                case ProcessUpdateType.UpdateRunCount:
                    this.txtRunCount.Text = values[0].ToString();
                    break;
                case ProcessUpdateType.ThreadStopped:
                    ConsoleAgent.Write(format: "Search job stopped by user.", color: "white", bgcolor: "black");

                    break;
                case ProcessUpdateType.StartingThread:
                    ConsoleAgent.Write(format: "Starting search job.", color: "white", bgcolor: "black", doSpeak: true);

                    break;
                case ProcessUpdateType.LoopStarting:

                    ConsoleAgent.Write(format: "Search {0} started iterating stores to find products.", color: "white", bgcolor: "black", args: values);

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

    }

}
