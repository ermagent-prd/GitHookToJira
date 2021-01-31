﻿using GeminiTools.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeminiTools.Engine
{
    public class LinkItemEngine
    {
        public LinkItemEngine()
        { }

        public LinkItem Execute(string htmlToParse)
        {
            var regex = new Regex("<a [^>]*href=(?:'(?<href>.*?)')|(?:\"(?<href>.*?)\")", RegexOptions.IgnoreCase);
            var urls = regex.Matches(htmlToParse).OfType<Match>().Select(m => m.Groups["href"].Value).SingleOrDefault();

            MatchCollection match = Regex.Matches(htmlToParse, @"(<a.*?>.*?</a>)",
                RegexOptions.Singleline);

            string value = match[0].Groups[1].Value;

            string fileName = Regex.Replace(value, @"\s*<.*?>\s*", "",
                    RegexOptions.Singleline);

            return new LinkItem
            {
                Href = urls,
                FileName = fileName
            };

        }
    }
}