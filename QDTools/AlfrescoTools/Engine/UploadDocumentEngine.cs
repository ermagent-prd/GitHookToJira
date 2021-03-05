using AlfrescoTools.Parameters;
using AlfrescoTools.Service;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS.Data.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfrescoTools.Engine
{
    public class UploadDocumentEngine
    {
        #region Private properties
        
        private readonly IAlfrescoToolsParameters parameters;


        #endregion

        #region Constructor

        public UploadDocumentEngine(IAlfrescoToolsParameters parameters)
        {
            this.parameters = parameters;
            
        }

        #endregion

        #region Public methods

        public string Execute(IFolder parentFolder, string documentName, string attachmentPath)
        {
            
            // Check if document already exist, if not create it
            Document newDocument = null;

            var children = parentFolder.GetChildren();
            foreach (var child in children)
            {
                if (child.Name == documentName)
                    newDocument = (Document)child;
            }


            if (newDocument == null)
            {
                // Setup document metadata
                Dictionary<String, Object> newDocumentProps =
                        new Dictionary<String, Object>();
                newDocumentProps.Add(PropertyIds.ObjectTypeId, "cmis:document");
                newDocumentProps.Add(PropertyIds.Name, documentName);

                // Setup document content
                String mimetype = "text/plain; charset=UTF-8";

                byte[] bytes = File.ReadAllBytes(attachmentPath + documentName);

                // read a empty document with empty bytes  
                // fileUpload1: is a .net file upload control  
                ContentStream contentStream = new ContentStream
                {
                    FileName = documentName,
                    MimeType = mimetype,
                    Length = bytes.Length,
                    Stream = new MemoryStream(bytes)
                };

                // this statment would create document in default repository  
                newDocument = (Document)parentFolder.CreateDocument(newDocumentProps, contentStream, null);

            }

            return "http://10.100.2.85:8080/share/page/document-details?nodeRef=" + newDocument.VersionSeriesId;
        }
        #endregion

        #region Private


        #endregion
    }
}
