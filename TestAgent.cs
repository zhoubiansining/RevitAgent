using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAgent
{
    /// <summary>
    /// Start benchmark test with the agent.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class TestAgent : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                // Select test file
                string testFilePath = "";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text Files (*.txt)|*.txt";
                openFileDialog.Title = "Select a test file for benchmarking";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    testFilePath = openFileDialog.FileName;
                }
                else
                {
                    TaskDialog.Show("Notice", "Test cancelled since no test file is selected.");
                    return Result.Cancelled;
                }

                // Load data from the test file
                List<string> testLines = new List<string>();
                using (System.IO.StreamReader file = new System.IO.StreamReader(testFilePath, Encoding.UTF8))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        testLines.Add(line);
                    }
                }

                // Select language
                TaskDialog mainDialog = new TaskDialog("Select Language");
                mainDialog.MainInstruction = "Please select a language for the test.\n--Would you prefer to use Chinese over English (default)? ";
                mainDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
                bool useChinese = mainDialog.Show() == TaskDialogResult.Yes;

                // Initialize progress form
                System.Windows.Forms.Form progressForm = new System.Windows.Forms.Form();
                progressForm.Text = "Test Progress";
                progressForm.Width = 1000;
                progressForm.Height = 300;

                // Initialize table layout panel
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.RowCount = 2;
                tableLayoutPanel.ColumnCount = 1;
                tableLayoutPanel.Dock = DockStyle.Fill;
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 75F));
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tableLayoutPanel.Padding = new Padding(20);
                progressForm.Controls.Add(tableLayoutPanel);

                // Initialize label
                Label progressLabel = new Label();
                progressLabel.Text = "0/" + testLines.Count;
                progressLabel.TextAlign = ContentAlignment.MiddleCenter;
                progressLabel.Font = new Font(progressLabel.Font.FontFamily, 14);
                progressLabel.Dock = DockStyle.Fill;
                tableLayoutPanel.Controls.Add(progressLabel, 0, 0);

                // Initialize progress bar
                ProgressBar progressBar = new ProgressBar();
                progressBar.Minimum = 0;
                progressBar.Maximum = testLines.Count;
                progressBar.Step = 1;
                progressBar.Value = 1;
                progressBar.Dock = DockStyle.Fill;
                progressBar.Height = 40;
                tableLayoutPanel.Controls.Add(progressBar, 0, 1);

                progressForm.Show();

                // Start test
                List<Dictionary<string, string>> testResults = new List<Dictionary<string, string>>();
                int completedLines = 0;
                foreach (string line in testLines)
                {
                    int retry = 3;
                    string testMessage = "";
                    Agent.Agent agent = new Agent.Agent(useChinese, retry);
                    Result result = agent.StartTest(ref testMessage, line);
                    if (result != Result.Succeeded)
                    {
                        Dictionary<string, string> testResultFailed = new Dictionary<string, string>{
                            {"Query", line},
                            {"Status", "Failed"},
                            {"Message", testMessage},
                            {"Responses", ""}
                        };
                        testResults.Add(testResultFailed);

                        string endMessageFailed = "";
                        agent.EndTest(ref endMessageFailed);

                        // Update progress bar and label
                        completedLines++;
                        progressBar.PerformStep();
                        progressLabel.Text = completedLines + "/" + testLines.Count;

                        continue;
                    }

                    // Start a transaction group
                    TransactionGroup transactionGroup = new TransactionGroup(commandData.Application.ActiveUIDocument.Document);
                    transactionGroup.Start("Agent Operations");

                    // Execute conversation rounds
                    int maxRounds = 25;
                    string responses = "";
                    string roundMessage = "";
                    bool succeeded = false;
                    int cnt = 1;
                    while (maxRounds > 0)
                    {
                        result = agent.ExecuteTestRound(commandData, ref roundMessage, elements, out bool shouldEnd, out string actResponse, out string systemResponse);
                        if (result != Result.Succeeded)
                        {
                            if (actResponse != null)
                            {
                                responses += $"**{cnt}**\n" + "<Action>\n" + actResponse;
                            }
                            cnt++;
                            succeeded = false;
                            break;
                        }
                        if (shouldEnd)
                        {
                            responses += $"**{cnt}**\n" + "<Action>\n" + actResponse;
                            cnt++;
                            succeeded = true;
                            break;
                        }
                        responses += $"**{cnt}**\n" + "<Action>\n" + actResponse + "\n<Result>\n" + systemResponse + '\n';
                        cnt++;
                        maxRounds--;
                    }
                    if (maxRounds == 0)
                    {
                        succeeded = false;
                        roundMessage = "The test is ended due to the maximum rounds reached.";
                    }

                    // Add test result
                    Dictionary<string, string> testResult = new Dictionary<string, string>{
                        {"Query", line},
                        {"Status", succeeded? "Succeeded" : "Failed"},
                        {"Message", succeeded? "" : roundMessage},
                        {"Responses", responses}
                    };
                    testResults.Add(testResult);

                    // End the test
                    transactionGroup.RollBack();
                    string endMessage = "";
                    agent.EndTest(ref endMessage);

                    // Update progress bar and label
                    completedLines++;
                    progressBar.PerformStep();
                    progressLabel.Text = completedLines + "/" + testLines.Count;
                }

                // Close progress form
                progressForm.Close();

                // Save test results
                string saveFilePath = "your-path" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFilePath, false, Encoding.UTF8))
                {
                    file.WriteLine("Test results for test file: " + testFilePath);
                    file.WriteLine(new string('-', 100));
                    int cnt = 1;
                    foreach (var result in testResults)
                    {
                        file.WriteLine($"ID: {cnt}\n");
                        file.WriteLine("Query: " + result["Query"] + '\n');
                        file.WriteLine("Status: " + result["Status"] + '\n');
                        file.WriteLine("Message: " + result["Message"] + '\n');
                        file.WriteLine("Responses:\n" + result["Responses"] + '\n');
                        file.WriteLine(new string('-', 100));
                        cnt++;
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = "Exception occurred when testing:\n" + ex.Message;
                return Result.Failed;
            }
        }
    }
}
