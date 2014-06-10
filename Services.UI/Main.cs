namespace Services.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using Ionic.Zip;
    using Services.Communication.Protocol;
    using Services.Management.Administration.Executioner;
    using Services.Management.Administration.Server;
    using Services.Management.Administration.Update;
    using Services.Management.Administration.Worker;
    using Services.UI.Dialogs;

    public partial class Main : Form
    {
        private static Dictionary<string, WorkUnitReportData> reports;
        private static Dictionary<string, List<RepoServicesData>> repoServices;
        internal static ClientConnectionContext AdminConnection;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(ReportThread);
            redrawFromReportsTimer.Start();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (AdminConnection != null)
                {
                    AdminConnection.Close();
                }
            }
            catch
            {
            }
        }

        private void redrawFromReportsTimer_Tick(object sender, EventArgs e)
        {
            if (AdminConnection == null)
            {
                mainTreeView.Nodes.Clear();
                welcomePanel.Visible = true;
                serviceNamePanel.Visible = false;
                serviceProcessPanel.Visible = false;
                toolStripStatusLabel.Text = "Not connected.";
                return;
            }

            try
            {
                if (reports != null && repoServices != null)
                {
                    var names = reports.Select(x => x.Value.StartData.ServiceName).ToList();
                    names.AddRange(repoServices.Keys);
                    names.Add(AdministrationContext.GeneralServiceName);
                    names = names.Distinct().ToList();

                    foreach (var name in names)
                    {
                        if (!mainTreeView.Nodes.ContainsKey(name))
                        {
                            mainTreeView.Nodes.Add(name, name);
                            mainTreeView.Nodes[name].Tag = name;
                        }

                        mainTreeView.Nodes[name].NodeFont = new Font(
                            FontFamily.GenericSansSerif, 8.25F, FontStyle.Regular);
                    }

                    foreach (var key in reports.Keys)
                    {
                        var item = reports[key];
                        if (!mainTreeView.Nodes[item.StartData.ServiceName].Nodes.ContainsKey(key))
                        {
                            mainTreeView.Nodes[item.StartData.ServiceName].Nodes.Add(
                                item.StartData.Id,
                                item.StartData.Id
                                    + (item.StartData.Version > 0 ? " v" + item.StartData.Version : string.Empty));
                        }

                        mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].Tag = item;
                        mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].NodeFont =
                            new Font(FontFamily.GenericSansSerif, 8.25F, FontStyle.Regular);
                        switch (item.WorkerState)
                        {
                            case WorkUnitState.Running:
                                mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].ForeColor =
                                    Color.Green;
                                break;
                            case WorkUnitState.Restarting:
                                mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].ForeColor =
                                    Color.Orange;
                                break;
                            case WorkUnitState.Updating:
                                mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].ForeColor =
                                    Color.MediumPurple;
                                break;
                            case WorkUnitState.Stopping:
                                mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].ForeColor =
                                    Color.LightCoral;
                                break;
                            case WorkUnitState.Abandoned:
                                mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].ForeColor =
                                    Color.LightGray;
                                break;
                            default:
                                mainTreeView.Nodes[item.StartData.ServiceName].Nodes[item.StartData.Id].ForeColor =
                                    Color.Black;
                                break;
                        }
                    }

                    foreach (var rootNode in mainTreeView.Nodes.Cast<TreeNode>())
                    {
                        var nodes = rootNode.Nodes.Cast<TreeNode>().ToList();
                        foreach (var node in nodes)
                        {
                            var tagItem = node.Tag as WorkUnitReportData;
                            if (tagItem == null || !reports.ContainsKey(tagItem.StartData.Id))
                            {
                                mainTreeView.Nodes[rootNode.Tag as string].Nodes.Remove(node);
                            }
                        }
                    }
                }

                if (mainTreeView.SelectedNode != null)
                {
                    mainTreeView.SelectedNode.NodeFont = new Font(FontFamily.GenericSansSerif, 8.25F, FontStyle.Bold);

                    var selectedWorkUnitReportData = mainTreeView.SelectedNode.Tag as WorkUnitReportData;
                    if (selectedWorkUnitReportData != null)
                    {
                        welcomePanel.Visible = false;
                        serviceNamePanel.Visible = false;
                        serviceProcessPanel.Visible = true;

                        serviceProcessIdLabel.Text = "Service instance " + selectedWorkUnitReportData.StartData.Id;

                        timeWorkingLabel.Text = (selectedWorkUnitReportData.Uptime.TotalDays >= 1
                            ? Math.Floor(selectedWorkUnitReportData.Uptime.TotalDays) + "d "
                            : string.Empty) + selectedWorkUnitReportData.Uptime.Hours + "h "
                            + selectedWorkUnitReportData.Uptime.Minutes + "m "
                            + selectedWorkUnitReportData.Uptime.Seconds + "s";

                        timeStartedLabel.Text = selectedWorkUnitReportData.StartedTime.ToLocalTime().ToString();

                        logTextBox.Text = selectedWorkUnitReportData.Log.Replace("\r\n", "\n").Replace("\n", "\r\n");

                        var newErrorCount = selectedWorkUnitReportData.ErrorCount.ToString();
                        if (totalReportedErrorsLabel.Text != newErrorCount)
                        {
                            totalReportedErrorsLabel.Text = newErrorCount;
                            errorsTextBox.Text = string.Empty;
                            foreach (var error in selectedWorkUnitReportData.Errors)
                            {
                                errorsTextBox.Text = error.Message + Environment.NewLine + Environment.NewLine
                                    + error.GetType().FullName + Environment.NewLine + Environment.NewLine
                                    + error.StackTrace + Environment.NewLine + Environment.NewLine + errorsTextBox.Text;
                            }
                        }

                        instanceTypeLabel.Text = selectedWorkUnitReportData.StartData.ContextType + " (repo version:"
                            + selectedWorkUnitReportData.StartData.Version + ")";

                        instanceParametersTextBox.Text = selectedWorkUnitReportData.StartData.Parameters != null &&
                                selectedWorkUnitReportData.StartData.Parameters.Any()
                            ? selectedWorkUnitReportData.StartData.Parameters.Select(x => x.ToString())
                                                        .Aggregate((x, y) => x + ", " + y)
                            : string.Empty;

                        instanceFilepathLabel.Text = selectedWorkUnitReportData.StartData.DllPath;

                        instanceIdLabel.Text = selectedWorkUnitReportData.StartData.Id;

                        instanceHostLabel.Text =
                            string.IsNullOrWhiteSpace(selectedWorkUnitReportData.StartData.ContextServerHost)
                                ? "Work unit does not broadcast this object."
                                : "tcp://" + selectedWorkUnitReportData.StartData.ContextServerHost + ":"
                                    + selectedWorkUnitReportData.StartData.ContextServerPort;

                        switch (selectedWorkUnitReportData.WorkerState)
                        {
                            case WorkUnitState.Running:
                                instanceStateLabel.Text = "Running";
                                instanceStateLabel.ForeColor = Color.Green;

                                closeServiceButton.Enabled = true;
                                deleteServiceEntryButton.Enabled = false;
                                restartServiceButton.Enabled = true;
                                cloneButton.Enabled = true;
                                changeVersionServiceEntryButton.Enabled = true;
                                break;
                            case WorkUnitState.Restarting:
                                instanceStateLabel.Text = "Restarting...";
                                instanceStateLabel.ForeColor = Color.Orange;

                                closeServiceButton.Enabled = false;
                                deleteServiceEntryButton.Enabled = false;
                                restartServiceButton.Enabled = false;
                                cloneButton.Enabled = true;
                                changeVersionServiceEntryButton.Enabled = false;
                                break;
                            case WorkUnitState.Updating:
                                instanceStateLabel.Text = "Updating";
                                instanceStateLabel.ForeColor = Color.MediumPurple;

                                closeServiceButton.Enabled = false;
                                deleteServiceEntryButton.Enabled = false;
                                restartServiceButton.Enabled = false;
                                cloneButton.Enabled = true;
                                changeVersionServiceEntryButton.Enabled = false;
                                break;
                            case WorkUnitState.Stopping:
                                instanceStateLabel.Text = "Stopped";
                                instanceStateLabel.ForeColor = Color.LightCoral;

                                closeServiceButton.Enabled = false;
                                deleteServiceEntryButton.Enabled = true;
                                restartServiceButton.Enabled = true;
                                cloneButton.Enabled = true;
                                changeVersionServiceEntryButton.Enabled = false;
                                break;
                            case WorkUnitState.Abandoned:
                                instanceStateLabel.Text = "Abandoned";
                                instanceStateLabel.ForeColor = Color.LightGray;

                                closeServiceButton.Enabled = false;
                                deleteServiceEntryButton.Enabled = true;
                                restartServiceButton.Enabled = false;
                                cloneButton.Enabled = false;
                                changeVersionServiceEntryButton.Enabled = false;
                                break;
                        }

                        instanceStartDataTextbox.Text = selectedWorkUnitReportData.StartData.SerializeToJson();
                        cmdScriptTextBox.Text =
                            new WorkerExecutioner(
                                ExecutionMode.StartWorkerFromAdmin, selectedWorkUnitReportData.StartData)
                                .GetProcessArguments(ExecutionMode.StartWorkerFromAdmin);

                        if (!repoServices.ContainsKey(selectedWorkUnitReportData.StartData.ServiceName)
                            || repoServices[selectedWorkUnitReportData.StartData.ServiceName].Count < 2)
                        {
                            changeVersionServiceEntryButton.Enabled = false;
                        }
                    }

                    var selectedServiceName = mainTreeView.SelectedNode.Tag as string;
                    if (!string.IsNullOrWhiteSpace(selectedServiceName))
                    {
                        welcomePanel.Visible = false;
                        serviceNamePanel.Visible = true;
                        serviceProcessPanel.Visible = false;
                        serviceNameLabel.Text = mainTreeView.SelectedNode.Text;

                        serviceInfoTextBox.Text = string.Empty;
                        if (!repoServices.Any(x => x.Key == selectedServiceName))
                        {
                            serviceInfoTextBox.Text +=
                                "Service has no custom files (the host process includes the code)" + Environment.NewLine;
                        }
                        else
                        {
                            serviceInfoTextBox.Text += "Available versions" + Environment.NewLine;
                            var versions = repoServices[selectedServiceName].OrderByDescending(x => x.Version).ToArray();
                            foreach (var repoServicesData in versions)
                            {
                                serviceInfoTextBox.Text += "Version #" + repoServicesData.Version + " created on "
                                    + repoServicesData.CreatedTime + Environment.NewLine;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel.Text = "An error has been reported: " + ex.Message;
            }
        }

        private void ReportThread(object state)
        {
            while (true)
            {
                if (AdminConnection == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                try
                {
                    reports = AdminConnection.Do(new Send<GetReportedDataReturnData>(new GetReportedData())).Reports;

                    repoServices =
                        AdminConnection.Do(new Send<GetAllRepoServicesReturnData>(new GetAllRepoServices()))
                            .RepoServices;

                    toolStripStatusLabel.Text = "tcp://" + AdminConnection.Parent.Hostname + ":"
                        + AdminConnection.Parent.Port;

                    Thread.Sleep(300);
                }
                catch
                {
                    toolStripStatusLabel.Text = "Connection to the server has been lost... Reconnecting...";
                    Thread.Sleep(1000);

                    try
                    {
                        AdminConnection.Do(new RetryConnection());
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Connect().ShowDialog();
        }

        private void startWorkUnitButton_Click(object sender, EventArgs e)
        {
            if (mainTreeView.SelectedNode == null)
            {
                return;
            }

            var selectedServiceName = mainTreeView.SelectedNode.Tag as string;
            if (string.IsNullOrWhiteSpace(selectedServiceName))
            {
                return;
            }

            toolStripStatusLabel.Text = "Starting process '" + mainTreeView.SelectedNode.Text + "'...";

            var repoServices = AdminConnection.Do(new Send<GetAllRepoServicesReturnData>(new GetAllRepoServices())).RepoServices;

            if (repoServices.All(x => x.Key != selectedServiceName))
            {
                var typesFromAssembly =
                    AdminConnection.Do(new Send<GetFileTypesReturnData>(new GetAdminTypes())).Types;

                if (typesFromAssembly != null)
                {
                    SelectTypeAndExecute(typesFromAssembly, selectedServiceName);
                }

                return;
            }

            var item =
                repoServices[selectedServiceName].First(
                    x => x.Version == (int)versionToStartNumericUpDown.Value);

            var serviceFilesData =
                AdminConnection.Do(
                    new Send<GetServiceFilesReturnData>(
                        new GetServiceFiles(
                            new GetServiceFilesData
                            {
                                ServiceName = selectedServiceName,
                                Version = item.Version
                            }))).Files;

            if (serviceFilesData != null)
            {
                // Pick file (dialog)
                var selectAssemblyForm = new SelectAssembly(serviceFilesData);

                if (selectAssemblyForm.ShowDialog() == DialogResult.OK)
                {
                    var fileSelected = selectAssemblyForm.SelectedFile;

                    var typesFromAssembly =
                        AdminConnection.Do(
                            new Send<GetFileTypesReturnData>(
                                new GetFileTypes(
                                    new GetFileTypesData
                                    {
                                        ServiceName = selectedServiceName,
                                        Version = item.Version,
                                        File = fileSelected
                                    }))).Types;

                    if (typesFromAssembly != null)
                    {
                        SelectTypeAndExecute(typesFromAssembly, selectedServiceName, fileSelected, item.Version);
                    }
                }
            }
        }

        private void SelectTypeAndExecute(
            string[] types, string selectedServiceName, string fileSelected = null, int version = 0)
        {
            var selectTypeForm = new SelectType(types);
            if (selectTypeForm.ShowDialog() == DialogResult.OK)
            {
                var typeSelected = selectTypeForm.ValueSelected;
                var id = selectTypeForm.IsAutoId ? Guid.NewGuid().ToString() : selectTypeForm.IdText;

                AdminConnection.Do(
                    new Send(
                        new StartWorkerProcess(
                            new StartWorkerData
                            {
                                Id = id,
                                ServiceName = selectedServiceName,
                                AdminHost = AdminConnection.Parent.Hostname,
                                AdminPort = AdminConnection.Parent.Port,
                                ContextType = typeSelected,
                                DllPath = fileSelected,
                                Version = version,
                                ContextServerHost = selectTypeForm.IsServed ? selectTypeForm.ServedHost : null,
                                ContextServerPort = selectTypeForm.IsServed ? selectTypeForm.ServedPort : 0,
                                Parameters = selectTypeForm.Parameters,
                            })));

                toolStripStatusLabel.Text = "Started a process of '" + selectedServiceName + "'";
            }
        }

        private void cloneButton_Click(object sender, EventArgs e)
        {
            if (mainTreeView.SelectedNode == null)
            {
                return;
            }

            var selectedWorkUnitReportData = mainTreeView.SelectedNode.Tag as WorkUnitReportData;
            if (selectedWorkUnitReportData == null)
            {
                return;
            }

            var cloneData = selectedWorkUnitReportData.StartData;
            var newId = Guid.NewGuid().ToString();

            AdminConnection.Do(
                new Send(
                    new StartWorkerProcess(
                        new StartWorkerData
                        {
                            Id = newId,
                            ServiceName = cloneData.ServiceName,
                            AdminHost = AdminConnection.Parent.Hostname,
                            AdminPort = AdminConnection.Parent.Port,
                            ContextType = cloneData.ContextType,
                            DllPath = cloneData.DllPath,
                            Version = cloneData.Version,
                            ContextServerHost = cloneData.ContextServerHost,
                            ContextServerPort = cloneData.ContextServerPort,
                            HostProcessFileName = cloneData.HostProcessFileName,
                            Parameters = cloneData.Parameters
                        })));
        }

        private void closeServiceButton_Click(object sender, EventArgs e)
        {
            if (mainTreeView.SelectedNode == null)
            {
                return;
            }

            var selectedWorkUnitReportData = mainTreeView.SelectedNode.Tag as WorkUnitReportData;
            if (selectedWorkUnitReportData == null)
            {
                return;
            }

            AdminConnection.Do(new Send(new StopWorkerProcess(selectedWorkUnitReportData.StartData.Id)));
        }

        private void restartServiceButton_Click(object sender, EventArgs e)
        {
            lock (AdminConnection)
            {
                if (mainTreeView.SelectedNode == null)
                {
                    return;
                }

                var selectedWorkUnitReportData = mainTreeView.SelectedNode.Tag as WorkUnitReportData;
                if (selectedWorkUnitReportData == null)
                {
                    return;
                }

                AdminConnection.Do(new Send(new RestartWorkerProcess(selectedWorkUnitReportData.StartData.Id)));
            }
        }

        private void deleteServiceEntryButton_Click(object sender, EventArgs e)
        {
            if (mainTreeView.SelectedNode == null)
            {
                return;
            }

            var selectedWorkUnitReportData = mainTreeView.SelectedNode.Tag as WorkUnitReportData;
            if (selectedWorkUnitReportData == null)
            {
                return;
            }

            AdminConnection.Do(new Send(new DeleteWorkerProcessEntry(selectedWorkUnitReportData.StartData.Id)));
        }

        private void installToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectFolder = new SelectFolderToInstall();
            if (selectFolder.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(selectFolder.FolderSelected);
                zip.Save("archive.zip");
            }

            toolStripStatusLabel.Text = "Uploading file to service '" + selectFolder.ServiceName + "'...";
            ThreadPool.QueueUserWorkItem(
                x =>
                {
                    AdminConnection.Do(
                        new Send(
                            new UploadZipAndApplyServiceVersionUpdateFromIt(
                                new FileUploadToAdminData
                                {
                                    ServiceName = selectFolder.ServiceName,
                                    FileData = File.ReadAllBytes("archive.zip")
                                })));

                    File.Delete("archive.zip");
                    toolStripStatusLabel.Text = "Service '" + selectFolder.ServiceName + "' has been updated";
                });
        }

        private void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainTreeView.SelectedNode == null)
            {
                return;
            }

            var selectedService = mainTreeView.SelectedNode.Tag as string;
            if (selectedService == null)
            {
                return;
            }

            AdminConnection.Do(new Send(new UninstallService(selectedService)));

            mainTreeView.Nodes.Remove(mainTreeView.SelectedNode);
        }

        private void changeVersionServiceEntryButton_Click(object sender, EventArgs e)
        {
            if (mainTreeView.SelectedNode == null)
            {
                return;
            }

            var selectedWorkUnitReportData = mainTreeView.SelectedNode.Tag as WorkUnitReportData;
            if (selectedWorkUnitReportData == null)
            {
                return;
            }

            var repoServices = AdminConnection.Do(new Send<GetAllRepoServicesReturnData>(new GetAllRepoServices())).RepoServices;
            var versionSelection =
                new SelectVersion(
                    selectedWorkUnitReportData.StartData.Version.ToString(),
                    repoServices[selectedWorkUnitReportData.StartData.ServiceName].OrderByDescending(x => x.Version)
                                                                                    .Select(x => x.Version.ToString())
                                                                                    .ToArray());
            if (versionSelection.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel.Text = "Updating a process with version "
                    + selectedWorkUnitReportData.StartData.Version + " to version " + versionSelection.SelectedValue;

                ThreadPool.QueueUserWorkItem(
                    x =>
                    {
                        AdminConnection.Do(new Send(new StopWorkerProcess(selectedWorkUnitReportData.StartData.Id)));

                        var startData = selectedWorkUnitReportData.StartData;
                        while (true)
                        {
                            lock (AdminConnection)
                            {
                                if (reports[startData.Id].WorkerState != WorkUnitState.Running)
                                {
                                    break;
                                }
                            }

                            Thread.Sleep(2000);
                        }

                        AdminConnection.Do(
                            new Send(new DeleteWorkerProcessEntry(selectedWorkUnitReportData.StartData.Id)));

                        startData.Version = int.Parse(versionSelection.SelectedValue);
                        AdminConnection.Do(new Send(new StartWorkerProcess(startData)));
                    });
            }
        }
    }
}
