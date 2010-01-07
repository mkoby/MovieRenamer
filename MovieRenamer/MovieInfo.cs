using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MovieRenamer
{
	public class MovieInfo
	{
		private string _movieName = String.Empty;
		private int _movieYear = 0;
		private int _movieWidth = 0;
		private string _movieCodec = String.Empty;
		private IList<AudioStream> _audioStreams = null;
		
		public MovieInfo(string NFOFile)
		{
			string NFOFileContents = ReadNFOFile(NFOFile);
			XElement movieElement = XElement.Parse(NFOFileContents);
			
			_movieName = movieElement.Element("title").Value;
			_movieYear = Convert.ToInt32(movieElement.Element("year").Value);
			_movieWidth = Convert.ToInt32(movieElement.Element("fileinfo").Element("streamdetails").Element("video").Element("width").Value);
			_movieCodec = movieElement.Element("fileinfo").Element("streamdetails").Element("video").Element("codec").Value.ToString().ToUpper();
			
			foreach(XElement audioStreamElement in movieElement.Element("fileinfo").Element("streamdetails").DescendantsAndSelf("audio"))
			{
				if(_audioStreams == null )
					_audioStreams = new List<AudioStream>();
				
				_audioStreams.Add(new AudioStream(audioStreamElement));
			}
			
		}
		
		private string ReadNFOFile(string NFOFile)
		{
			string Output = String.Empty;
			
			using(StreamReader r = new StreamReader(NFOFile))
				Output = r.ReadToEnd();
			
			return Output;				
		}
		
		public string GetMovieNameText()
		{
			string Output = String.Empty;
			string removeChars = "!@#$%^*():;'\".,<>?`~{}|";
			StringBuilder sb = new StringBuilder();			
			
			foreach(char c in _movieName.ToCharArray())
			{
				if(!removeChars.Contains(c.ToString()) && c != '&' && c != '/')
					sb.Append(c);
				
				if(c == '/')
					sb.Append(".");
				
				if(c == '&')
					sb.Append("and");
			}
			
			Output = sb.ToString().Replace(" ", ".");
			
			return Output;
		}
		
		public string GetCodecText()
		{
			string Output = "x264";
			
			if(_movieCodec == "XVID")
				Output = "XVID";
			if(_movieCodec == "DIVX")
				Output = "Divx";
			
			return Output;
		}
		
		public string GetAudioChannelsText()
		{
			string Output = "5.1ch";
			int channels = 0;
			
			foreach(AudioStream stream in _audioStreams)
			{
				if(stream.Channels > channels)
				{
					channels = stream.Channels;
					
					if(stream.Codec == "AC3" || stream.Codec == "DTS" || stream.Codec == "DCA")
					{
						if(stream.Channels > 2)
							Output = String.Format("{0}.1ch", stream.Channels - 1);
						else
							Output = "2ch";
					}
				}
			}
			
			return Output;
		}
		
		public string GetAudioCodecText()
		{
			string Output = "AC3(Dolby)";
			int channels = 0;
			
			foreach(AudioStream stream in _audioStreams)
			{
				if(stream.Channels > channels)
				{
					channels = stream.Channels;
					
					if(stream.Codec == "AC3")
						Output = "AC3(Dolby)";
								
					if(stream.Codec == "DTS" || stream.Codec == "DCA")
						Output = "DTS";
				}
			}
			
			return Output;
		}
		
		public string GetSourceText()
		{
			string Output = "DVDRip";
			
			if(_movieWidth == 1920)
				Output = "Blu-Ray";
			
			return Output;
		}
		
		public string GetResolutionText()
		{
			string Output = "480p";
			
			if(_movieWidth == 1920)
				Output = "1080p";
			
			return Output;
		}
		
		public string MovieName { get{ return _movieName; } }
		public int MovieYear { get{ return _movieYear; } }
		public int MovieWidth { get{ return _movieWidth; } }
		public string MovieCodec { get{ return _movieCodec; } }
		public IList<AudioStream> AudioStreams { get{ return _audioStreams; } }
	}
	
	public class AudioStream
	{
		private string _streamCodec = String.Empty;
		private int _streamChannels = 0;
		
		public AudioStream()
		{}
		
		public AudioStream(XElement streamElement)
		{
			_streamCodec = streamElement.Element("codec").Value.ToString().ToUpper();
			_streamChannels = Convert.ToInt32(streamElement.Element("channels").Value);
		}
		
		public string Codec { get{ return _streamCodec; } }
		public int Channels { get{ return _streamChannels; } }
	}
}
