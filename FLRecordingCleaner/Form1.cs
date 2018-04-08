using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace FLRecordingCleaner
{
    public partial class Form1 : Form
    {
        GatheringResult GatheringResult = null;
        ProjectInfo[] ProjectFolderGroups = null;

        public Form1()
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
                MessageBox.Show(this, "Folder you have choosen does not exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning );
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
                result.ProjectList.Add(new ProjectInfo {
                    FileName = file,
                });
            }

            // second we're parsing the FLP file for project folder path
            //foreach (var project in result.ProjectList)
            for(int i = 0; i < result.ProjectList.Count; i++)
            {
                var project = result.ProjectList[i];

                var parser = new Monad.FLParser.ProjectParser(false);

                try
                {
                    using (var stream = File.OpenRead(project.FileName))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            parser.Parse(reader);
                        }
                    }

                    // add only non-empty project paths
                    if (!string.IsNullOrWhiteSpace(parser.project.ProjectPath))
                        project.ProjectPath = parser.project.ProjectPath;
                }
                catch (Exception ex)
                {
                    // it has read error, so it may have invalid tracks
                    project.HasLoadErrors = true;

                    // if there is project path were parsed, it's correct
                    if (!string.IsNullOrWhiteSpace(parser.project.ProjectPath))
                        project.ProjectPath = parser.project.ProjectPath;
                }

                // skip project where is no project path set
                if (string.IsNullOrWhiteSpace(project.ProjectPath))
                    continue;

                // try to fix project data path, if hard drives were mysteriously changed
                if (!Directory.Exists(project.ProjectPath))
                {
                    var projectLocation = Helpers.NormalizePath(Path.GetDirectoryName(project.FileName));
                    var projectDataLocation = Helpers.NormalizePath(project.ProjectPath);

                    // remove drive letter
                    if (string.Compare(projectDataLocation.Substring(1), projectLocation.Substring(1), StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        project.ProjectPath = projectLocation;
                    }
                    else
                    {
                        // ask user if he want to use the project location as project data folder
                        var pathErrMsg = string.Format("Project folder '{0}' was not found for project {1}\n\nTry to use path {2} for it?",
                            project.ProjectPath, project.FileName, projectLocation);

                        var dlgResult = (DialogResult)this.Invoke((Func<DialogResult>)(() => MessageBox.Show(this, pathErrMsg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question)));

                        if (dlgResult == DialogResult.Yes)
                            project.ProjectPath = projectLocation;
                        else
                            project.PathError = true;
                    }
                }

                project.ProjectPath = Helpers.NormalizePath(project.ProjectPath);

                // second we try search for WAV files in project folder
                try {
                    foreach (string file in Directory.EnumerateFiles(project.ProjectPath, "*.wav", SearchOption.AllDirectories))
                    {
                        var fileInfo = new System.IO.FileInfo(file);

                        project.FolderWAVList.Add(new FileInfo {
                            FileName = file,
                            FileSize = fileInfo.Length,
                        });
                    }
                }
                catch (Exception ex)
                {
                    project.PathError = true;
                }

                foreach (var chan in parser.project.Channels)
                {
                    var chanData = chan.Data as Monad.FLParser.GeneratorData;

                    if (chanData != null && !string.IsNullOrWhiteSpace(chanData.SampleFileName))
                    {
                        var wavFilename = chanData.SampleFileName;
                        
                        try
                        {
                            // try to fix file path, if hard drives were mysteriously changed
                            if (!File.Exists(wavFilename))
                            {
                                var wavLocation = Helpers.NormalizePath(Path.GetDirectoryName(chanData.SampleFileName));

                                if (string.Compare(wavLocation.Substring(1), project.ProjectPath.Substring(1), StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    wavFilename = Path.Combine(project.ProjectPath, Path.GetFileName(wavFilename));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            project.FileError = true;
                        }

                        project.UsedWAVList.Add(wavFilename);
                    }
                }

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
            ProjectFolderGroups = folderProjectGroups.Select(x => new ProjectInfo {
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
