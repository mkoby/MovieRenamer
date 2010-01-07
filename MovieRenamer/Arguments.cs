using System;
using System.IO;

namespace MovieRenamer
{
	public class Arguments
	{
		private string _processdir = "/media/kobynas02/video/movies/";
		
		public Arguments() {}
				
		public Arguments(string[] args)
		{
			if(args.Length > 0)
				if(Directory.Exists(args[0]))
					_processdir = args[0];
		}
		
		public string ProcessDir { get{ return _processdir; } }
	}
}
