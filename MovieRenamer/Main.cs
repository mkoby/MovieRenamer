using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MovieRenamer
{
	class MainClassfile
	{
		public static void Main(string[] args)
		{
			Arguments myArgs = new Arguments();
			FileInfo[] movieFiles = GetMovieFilesToProcess(myArgs.ProcessDir);
			IList<string> fileRenamingList = new List<string>();
			
			if(movieFiles != null && movieFiles.Length > 0)
			{
				foreach(FileInfo movieFile in movieFiles)
				{
					Console.WriteLine("{0}", movieFile.Directory.Name);
					MovieInfo movieInfo = GetMovieInfo(movieFile);
					StringBuilder sb = new StringBuilder();
					//Name.Year.Source.Resolution.AudioCodec.Channels.VideoCodec_cd.EXT
					sb.AppendFormat("{0}.{1}.{2}.{3}.{4}.{5}.{6}{7}", 
					                movieInfo.GetMovieNameText(), movieInfo.MovieYear, 
					                movieInfo.GetSourceText(), movieInfo.GetResolutionText(), 
					                movieInfo.GetAudioCodecText(), movieInfo.GetAudioChannelsText(), 
					                movieInfo.GetCodecText(), GetFileNumberText(movieFile));
					
					fileRenamingList.Add(sb.ToString());
					RenameFilesInDir(movieFile.Directory, movieFile.Name, sb.ToString());
					sb = null;
				}
			}
			
			if(fileRenamingList.Count > 0 )
			{
				using(StreamWriter w = new StreamWriter("/home/mkoby/output.txt"))
				{
					foreach(string s in fileRenamingList)
						w.WriteLine(s);
				}
			}
		}
		
		private static void RenameFilesInDir(DirectoryInfo movieFileDir, string movieFilename, string renameString)
		{
			foreach(FileInfo file in movieFileDir.GetFiles(movieFilename.Substring(0, movieFilename.Length - 3) + "*"))
			{
				string fileExt = file.Name.Substring(file.Name.Length - 3);
				Console.WriteLine("{0} -> {1}", file.Name, String.Format("{0}.{1}", renameString, fileExt));
				file.MoveTo(String.Format("{0}{1}{2}.{3}", file.Directory, Path.DirectorySeparatorChar, renameString, fileExt));
			}
		}
		
		private static FileInfo[] GetMovieFilesToProcess(string ProcessDir)
		{
			DirectoryInfo di = new DirectoryInfo(ProcessDir);
			return di.GetFiles("*.mkv", SearchOption.AllDirectories);
		}
		
		private static MovieInfo GetMovieInfo(FileInfo movieInfo)
		{
			string movieNFOFile = movieInfo.Directory.FullName + Path.DirectorySeparatorChar + "movie.nfo";
			return new MovieInfo(movieNFOFile);
		}
		
		private static string GetFileNumberText(FileInfo fileName)
		{
			string Output = String.Empty;
			
			if(fileName.Name.IndexOf("_cd") > -1)
				Output = fileName.Name.Substring(fileName.Name.IndexOf("_cd"), 4);
			
			return Output;
		}
	}
}