using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using SIL.Machine.Corpora;
using SIL.Machine.Tokenization;

namespace SIL.Machine.WebApi.Models
{
    public class ShareDBMongoText : IText
    {
		private readonly BsonDocument _doc;
		private readonly ITokenizer<string, int> _wordTokenizer;
		public ShareDBMongoText(BsonDocument doc, ITokenizer<string, int> wordTokenizer)
		{
			_doc = doc;
			_wordTokenizer = wordTokenizer;
		}
        public string Id
		{
			get
			{
				var id = (string) _doc["_id"];
				int index = id.IndexOf(":", StringComparison.Ordinal);
				return id.Substring(0, index);
			}
		}

        public IEnumerable<TextSegment> Segments 
		{
			get
			{
				var ops = (BsonArray) _doc["ops"];
				var sb = new StringBuilder();
				foreach (BsonDocument op in ops)
					sb.Append(op["insert"]);

				string[] segments = sb.ToString().Split('\n');
				for (int i = 0; i < segments.Length; i++)
					yield return new TextSegment(new TextSegmentRef(1, i + 1), _wordTokenizer.TokenizeToStrings(segments[i]).ToArray());
			}
		}
    }
}