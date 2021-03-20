using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace FLRecordingCleaner
{
	public partial class MainForm : Form
	{
		GatheringResult GatheringResult = null;
		ProjectInfo[] ProjectFolderGroups = null;

		public MainForm()
		{
			InitializeComponent();
		}

		private void browseBtn_Click(object sender, EventArgs e)
		{
			if (folderPickDlg.ShowDialog() == DialogResult.OK)
			{
				projectsPathBox.Text = folderPickDlg.SelectedPath;
			}
		}

		private void gatherInfoBtn_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(projectsPathBox.Text))
			{
				MessageBox.Show(this, "Folder you have choosen does not exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			totalProjectsLabel.Text = "Please wait...";
			totalRecLabel.Text = "-";
			totalRecSizeLabel.Text = "-";
			unusedRecLabel.Text = "-";
			unusedRecSizeLabel.Text = "-";

			GatheringResult = null;
			ProjectFolderGroups = null;

			progressBar.Value = 0;

			bgWorker.RunWorkerAsync(projectsPathBox.Text);
		}

		private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			string folderPath = (string)e.Argument;

			var result = new GatheringResult();

			// first we search for FLP files
			foreach (var file in Directory.EnumerateFiles(folderPath, "*.flp", SearchOption.AllDirectories))
			{
				result.ProjectList.Add(new ProjectInfo
				{
					FileName = file,
				});
			}

			var worker = new ProjectWorker(this);

			// second we're parsing the FLP file for project folder path
			//foreach (var project in result.ProjectList)
			for (int i = 0; i < result.ProjectList.Count; i++)
			{
				worker.CollectProjectData(result.ProjectList[i]);

				bgWorker.ReportProgress(Convert.ToInt32((float)i / (float)result.ProjectList.Count * 100.0f));
			}

			bgWorker.ReportProgress(100);

			// store the result
			e.Result = result;
		}

		private void bgWorker_OnComplete(object sender, RunWorkerCompletedEventArgs e)
		{
			GatheringResult = (GatheringResult)e.Result;

			// for counting records size
			var folderProjectGroups = GatheringResult.ProjectList.GroupBy(x => x.ProjectPath);

			// store project groups
			ProjectFolderGroups = folderProjectGroups.Select(x => new ProjectInfo
			{
				FolderWAVList = x.First().FolderWAVList,
				ProjectPath = x.First().ProjectPath,
				HasLoadErrors = x.Any(p => p.HasLoadErrors),
				FileError = x.Any(p => p.FileError),
				PathError = x.Any(p => p.PathError),
				ProjectFiles = x.Select(p => p.FileName).ToArray(),
				UsedWAVList = x.SelectMany(p => p.UsedWAVList.Select(f => f)).ToList(),
			}).Where(x => !string.IsNullOrWhiteSpace(x.ProjectPath)).ToArray();

			// now complete the unused files list to proceed
			foreach (var projectGroup in ProjectFolderGroups)
			{
				projectGroup.UnusedWAVList =
					projectGroup.FolderWAVList.Where(folderFile =>
						!projectGroup.UsedWAVList.Any(usedFile =>
							string.Compare(Path.GetFileName(folderFile.FileName), Path.GetFileName(usedFile), StringComparison.InvariantCultureIgnoreCase) == 0
							)
						).ToArray();
			}

			var totalRecordings = GatheringResult.ProjectList.Sum(x => x.FolderWAVList.Count);
			var totalRecordingsSize = ProjectFolderGroups.Sum(x => x.FolderWAVList.Sum(r => r.FileSize));
			var totalUnusedRecordings = ProjectFolderGroups.Sum(x => x.UnusedWAVList.Count());
			var totalUnusedRecordingsSize = ProjectFolderGroups.Sum(x => x.UnusedWAVList.Sum(r => r.FileSize));

			totalProjectsLabel.Text = string.Format("{0}", GatheringResult.ProjectList.Count);
			totalRecLabel.Text = string.Format("{0}", totalRecordings);
			totalRecSizeLabel.Text = string.Format("{0}", Helpers.BytesToString(totalRecordingsSize));
			unusedRecLabel.Text = string.Format("{0}", totalUnusedRecordings);
			unusedRecSizeLabel.Text = string.Format("{0}", Helpers.BytesToString(totalUnusedRecordingsSize));
		}

		private void doCleanupBtn_Click(object sender, EventArgs e)
		{
			progressBar.Value = 0;

			delWorker.RunWorkerAsync();
		}

		private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Value = e.ProgressPercentage;
		}

		private void delWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Value = e.ProgressPercentage;
		}

		private void delWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (ProjectFolderGroups == null)
			{
				MessageBox.Show(this, "You have to gather information before performing cleanup", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// this is made for progress bar
			var delFilesList = new List<FileInfo>();

			foreach (var projectGroup in ProjectFolderGroups)
			{
				if (!ignoreErrorsCheck.Checked)
				{
					if (projectGroup.PathError)
					{
						var pathErrMsg = string.Format("Project folder '{0}' was not found in next projects:\n{1}", projectGroup.ProjectPath, string.Join("\n", projectGroup.ProjectFiles));

						this.Invoke((Func<DialogResult>)(() => MessageBox.Show(this, pathErrMsg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
					}

					if (projectGroup.HasLoadErrors || projectGroup.FileError)
					{
						var errorDesc = (projectGroup.HasLoadErrors ? "FLP Problem" : "") + (projectGroup.FileError ? "Missing file" : "");

						var message = string.Format("Projects in '{0}' had errors while gathering ({1}).\n\nDo you want to skip them?", projectGroup.ProjectPath, errorDesc);


						var dialogResult = (DialogResult)this.Invoke((Func<DialogResult>)(() => MessageBox.Show(this, message, "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)));

						if (dialogResult == DialogResult.Yes)
							continue;

						if (dialogResult == DialogResult.Cancel)
						{
							this.Invoke((Func<DialogResult>)(() => MessageBox.Show(this, "Operation cancelled", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information)));
							break;
						}
					}
				}

				delFilesList.AddRange(projectGroup.UnusedWAVList);
			}

			for (int i = 0; i < delFilesList.Count; i++)
			{
				var wavInfo = delFilesList[i];

				// send files to recycle bin first
				Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(wavInfo.FileName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

				delWorker.ReportProgress(Convert.ToInt32((float)i / (float)delFilesList.Count * 100.0f));
			}

			delWorker.ReportProgress(100);

			this.Invoke((Func<DialogResult>)(() => MessageBox.Show(this, "Job completed. You still can find your files in recycle bin.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)));
		}
	}
}
