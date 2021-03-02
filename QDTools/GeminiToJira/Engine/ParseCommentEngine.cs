using GeminiToJira.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net;
using System.Linq;
using JiraTools.Model;

namespace GeminiToJira.Engine
{
    public class ParseCommentEngine
    {
        private readonly string HTML_TAG_PATTERN = "<.*?>";
        
        public string Execute(string comment)
        {
            var text = Regex.Replace(comment, HTML_TAG_PATTERN, string.Empty);

            return WebUtility.HtmlDecode(text);

        }        

        public string Execute(string comment, string commentPrefix, List<string> descAttachments)
        {
            comment = SaveAndReferAttachmentImages(comment, commentPrefix, descAttachments);

            
            var text = Regex.Replace(CleanFromImageTag(comment), HTML_TAG_PATTERN, string.Empty);

            return WebUtility.HtmlDecode(text);

        }

        /// <summary>
        /// Add image attachments
        /// </summary>
        /// <param name="jiraIssue"></param>
        /// <param name="comment"></param>
        /// <param name="commentPrefix"></param>
        public void Execute(CreateIssueInfo jiraIssue, string comment, string commentPrefix)
        {
            HtmlNodeCollection nodes = GetNodes(comment);
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    var fileName = commentPrefix + "_CommentAttachedImage" + i + ".png";
                    jiraIssue.Attachments.Add(fileName);
                }
            }
        }


        private string CleanFromImageTag(string comment)
        {
            if (comment.Contains("<img src="))
            {
                comment = comment.Replace("\" /> ", "");
                comment = comment.Replace("<img src=\"", "");
                comment = comment.Replace("\" />", "");
            }
            return comment;
        }

       
        private static string SaveAndReferAttachmentImages(string comment, string commentPrefix, List<string> descAttachments)
        {
            HtmlNodeCollection nodes = GetNodes(comment);
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    var img = nodes[i];
                    HtmlAttribute att = img.Attributes["src"];
                    try
                    {
                        byte[] imageBytes = Convert.FromBase64String(att.Value.Substring(att.Value.IndexOf(',') + 1));
                        var fileName = commentPrefix + "_CommentAttachedImage" + i + ".png";
                        File.WriteAllBytes(GeminiConstants.SAVING_PATH + fileName, imageBytes);
                        comment = comment.Replace(att.Value, "[^" + fileName + "]\n\n");

                        //for image's description only
                        if (descAttachments != null)
                            descAttachments.Add(fileName);
                    }
                    catch
                    {
                        if(att.Value.Contains("data:image"))
                            comment = comment.Replace(att.Value, "\n");

                        //else nothing
                        //for image linked to shared folder
                    }
                }
            }
            return comment;
        }

        private static HtmlNodeCollection GetNodes(string comment)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(comment);//or doc.Load(htmlFileStream)
            var nodes = doc.DocumentNode.SelectNodes(@"//img[@src]");
            return nodes;
        }

    }
}
