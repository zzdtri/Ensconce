﻿using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Ensconce
{
    internal static class DictionaryExport
    {
        internal static void ExportTagDictionary()
        {
            var dictionary = JsonConvert.SerializeObject(TextRendering.TagDictionary, Formatting.Indented);

            if (!string.IsNullOrWhiteSpace(Arguments.DictionarySavePath))
            {
                File.WriteAllText(Arguments.DictionarySavePath, dictionary);
            }

            if (!string.IsNullOrWhiteSpace(Arguments.DictionaryPostUrl))
            {
                using (var cli = new WebClient { Headers = { [HttpRequestHeader.ContentType] = "application/json" } })
                {
                    cli.UploadString(Arguments.DictionaryPostUrl, dictionary);
                }
            }
        }
    }
}
