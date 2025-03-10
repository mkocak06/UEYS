using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public static class MimeType
    {
        #region application/*

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationForceDownload = "application/force-download";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationJson = "application/json";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationManifestJson = "application/manifest+json";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationOctetStream = "application/octet-stream";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationPdf = "application/pdf";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationRssXml = "application/rss+xml";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationXml = "application/xml";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationXWwwFormUrlencoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationXZipCo = "application/x-zip-co";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationMsWord = "application/msword";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationMsWord2007 = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationOfficeDocument = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationExcel = "application/vnd.ms-excel";

        /// <summary>
        /// Type
        /// </summary>
        public const string ApplicationExcel2007 = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        #endregion

        #region image/*

        /// <summary>
        /// Type
        /// </summary>
        public const string ImageBmp = "image/bmp";

        /// <summary>
        /// Type
        /// </summary>
        public const string ImageGif = "image/gif";

        /// <summary>
        /// Type
        /// </summary>
        public const string ImageJpeg = "image/jpeg";

        /// <summary>
        /// Type
        /// </summary>
        public const string ImagePJpeg = "image/pjpeg";

        /// <summary>
        /// Type
        /// </summary>
        public const string ImagePng = "image/png";

        /// <summary>
        /// Type
        /// </summary>
        public const string ImageTiff = "image/tiff";

        #endregion

        #region text/*

        /// <summary>
        /// Type
        /// </summary>
        public const string TextCss = "text/css";

        /// <summary>
        /// Type
        /// </summary>
        public const string TextCsv = "text/csv";

        /// <summary>
        /// Type
        /// </summary>
        public const string TextJavascript = "text/javascript";

        /// <summary>
        /// Type
        /// </summary>
        public const string TextPlain = "text/plain";

        /// <summary>
        /// Type
        /// </summary>
        public const string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        #endregion

        #region Videos

        public const string VideoMp4 = "video/mp4";

        #endregion

        public static string[] ValidTypes = new[]
                {
                    ApplicationExcel,
                    ApplicationMsWord,
                    ApplicationPdf,
                    ApplicationOfficeDocument,
                    TextCss,
                    TextXlsx,
                    TextPlain,
                    ImageJpeg,
                    ImagePng,
                    ImageTiff,
                };
    }
}
