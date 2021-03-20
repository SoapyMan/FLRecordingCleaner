using Monad.FLParser;
using System;
using System.IO;
using System.Windows.Forms;

namespace FLRecordingCleaner
{
	class ProjectWorker
	{
		private readonly Control _parent;
		public ProjectWorker(Control parent)
		{
			_parent = parent;
		}

		private void CollectWAVFilesRecursively(ProjectInfo project)
		{
			try
			{
				foreach (string file in Directory.EnumerateFiles(project.ProjectPath, "*.wav", SearchOption.AllDirectories))
				{
					var fileInfo = new System.IO.FileInfo(file);

					project.FolderWAVList.Add(new FileInfo
					{
						FileName = file,
						FileSize = fileInfo.Length,
					});
				}
			}
			catch (Exception ex)
			{
				// TODO: save exception details
				project.PathError = true;
			}
		}

		private void CollectWAVFilesUsage(ProjectParser parser, ProjectInfo project)
		{
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
		}

		public bool CollectProjectData(ProjectInfo project)
		{
			var parser = new ProjectParser(false);

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
				return false;

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

					var dlgResult = (DialogResult)_parent.Invoke((Func<DialogResult>)(() => 
						MessageBox.Show(_parent, pathErrMsg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question)));

					if (dlgResult == DialogResult.Yes)
						project.ProjectPath = projectLocation;
					else
						project.PathError = true;
				}
			}

			// fix project path
			project.ProjectPath = Helpers.NormalizePath(project.ProjectPath);

			// second we try search for WAV files in project folder
			CollectWAVFilesRecursively(project);

			// third we check WAV files usage
			CollectWAVFilesUsage(parser, project);

			return true;
		}
	}
}
