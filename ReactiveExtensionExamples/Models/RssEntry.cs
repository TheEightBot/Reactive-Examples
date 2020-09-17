using System;
namespace ReactiveExtensionExamples.Models
{
	public class RssEntry
	{

		public string Id
		{
			get;
			set;
		}

		public string Author
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		public string Content
		{
			get;
			set;
		}

		public DateTimeOffset Updated
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public bool New
		{
			get;
			set;
		}
	}
}
