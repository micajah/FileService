using System;
using System.Drawing.Imaging;
using System.Linq;

namespace Micajah.FileService.WebService
{
    /// <summary>
    /// The MIME (Multipurpose Internet Mail Extensions) types of a file.
    /// </summary>
    public static class MimeType
    {
        #region Members

        /// <summary>
        /// text/javascript
        /// </summary>
        public const string JavaScript = "application/x-javascript";

        /// <summary>
        /// text/css
        /// </summary>
        public const string Css = "text/css";

        /// <summary>
        /// image/gif
        /// </summary>
        public const string Gif = "image/gif";

        /// <summary>
        /// image/jpeg
        /// </summary>
        public const string Jpeg = "image/jpeg";

        /// <summary>
        /// text/plain
        /// </summary>
        public const string Text = "text/plain";

        #endregion

        #region Public Methods

        public static bool IsFlash(string mimeType)
        {
            return (string.Compare(mimeType, "application/x-shockwave-flash", StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Determines whether the specified MIME type is image type.
        /// </summary>
        /// <param name="mimeType">The string that contains the MIME type to check.</param>
        /// <returns>true if the specified MIME type is image type; otherwise, false.</returns>
        public static bool IsImageType(string mimeType)
        {
            if (!string.IsNullOrEmpty(mimeType))
                return mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            return false;
        }

        /// <summary>
        /// Returns a MIME type by specified file extension.
        /// </summary>
        /// <param name="fileExtension">A file extension.</param>
        /// <returns>The MIME type string.</returns>
        public static string GetMimeType(string fileExtension)
        {
            if (fileExtension == null) return null;

            switch (fileExtension.ToUpperInvariant())
            {
                case ".AI":
                case ".EPS":
                case ".PS":
                    return "application/postscript";
                case ".AIF":
                case ".AIFC":
                case ".AIFF":
                    return "audio/x-aiff";
                case ".ASC":
                case ".C":
                case ".CC":
                case ".F":
                case ".F90":
                case ".H":
                case ".HH":
                case ".M":
                case ".TXT":
                    return Text;
                case ".AU":
                case ".SND":
                    return "audio/basic";
                case ".AVI":
                    return "video/x-msvideo";
                case ".ASF":
                    return "video/x-ms-asf";
                case ".BCPIO":
                    return "application/x-bcpio";
                case ".BIN":
                case ".CLASS":
                case ".DMS":
                case ".EXE":
                case ".LHA":
                case ".LZH":
                    return "application/octet-stream";
                case ".BMP":
                    return "image/bmp";
                case ".CCAD":
                    return "application/clariscad";
                case ".CDF":
                case ".NC":
                    return "application/x-netcdf";
                case ".CPIO":
                    return "application/x-cpio";
                case ".CPT":
                    return "application/mac-compactpro";
                case ".CSH":
                    return "application/x-csh";
                case ".CSS":
                    return "text/css";
                case ".DCR":
                case ".DIR":
                case ".DXR":
                    return "application/x-director";
                case ".DOC":
                    return "application/msword";
                case ".DRW":
                    return "application/drafting";
                case ".DVI":
                    return "application/x-dvi";
                case ".DWG":
                    return "application/acad";
                case ".DXF":
                    return "application/dxf";
                case ".ETX":
                    return "text/x-setext";
                case ".EZ":
                    return "application/andrew-inset";
                case ".FLI":
                    return "video/x-fli";
                case ".GIF":
                    return Gif;
                case ".GTAR":
                    return "application/x-gtar";
                case ".GZ":
                    return "application/x-gzip";
                case ".HDF":
                    return "application/x-hdf";
                case ".HQX":
                    return "application/mac-binhex40";
                case ".HTM":
                case ".HTML":
                    return "text/html";
                case ".ICE":
                    return "x-conference/x-cooltalk";
                case ".IEF":
                    return "image/ief";
                case ".IGES":
                case ".IGS":
                    return "model/iges";
                case ".IPS":
                    return "application/x-ipscript";
                case ".IPX":
                    return "application/x-ipix";
                case ".JPE":
                case ".JPEG":
                case ".JPG":
                    return Jpeg;
                case ".JS":
                    return JavaScript;
                case ".KAR":
                case ".MID":
                case ".MIDI":
                    return "audio/midi";
                case ".LATEX":
                    return "application/x-latex";
                case ".LSP":
                    return "application/x-lisp";
                case ".MAN":
                    return "application/x-troff-man";
                case ".ME":
                    return "application/x-troff-me";
                case ".MESH":
                case ".MSH":
                case ".SILO":
                    return "model/mesh";
                case ".MIF":
                    return "application/vnd.mif";
                case ".MIME":
                    return "www/mime";
                case ".MOV":
                case ".QT":
                    return "video/quicktime";
                case ".MOVIE":
                    return "video/x-sgi-movie";
                case ".MP2":
                case ".MP3":
                case ".MPGA":
                    return "audio/mpeg";
                case ".MPE":
                case ".MPEG":
                case ".MPG":
                    return "video/mpeg";
                case ".MP4":
                    return "video/mp4";
                case ".MS":
                    return "application/x-troff-ms";
                case ".ODA":
                    return "application/oda";
                case ".PBM":
                    return "image/x-portable-bitmap";
                case ".PDB":
                case ".XYZ":
                    return "chemical/x-pdb";
                case ".PDF":
                    return "application/pdf";
                case ".PGM":
                    return "image/x-portable-graymap";
                case ".PGN":
                    return "application/x-chess-pgn";
                case ".PNG":
                    return "image/png";
                case ".PNM":
                    return "image/x-portable-anymap";
                case ".POT":
                case ".PPS":
                case ".PPT":
                case ".PPZ":
                    return "application/mspowerpoint";
                case ".PPM":
                    return "image/x-portable-pixmap";
                case ".PRE":
                    return "application/x-freelance";
                case ".PRT":
                    return "application/pro_eng";
                case ".RA":
                    return "audio/x-realaudio";
                case ".RAM":
                case ".RM":
                    return "audio/x-pn-realaudio";
                case ".RAS":
                    return "image/cmu-raster";
                case ".RGB":
                    return "image/x-rgb";
                case ".ROFF":
                case ".T":
                case ".TR":
                    return "application/x-troff";
                case ".RPM":
                    return "audio/x-pn-realaudio-plugin";
                case ".RTF":
                    return "text/rtf";
                case ".RTX":
                    return "text/richtext";
                case ".SCM":
                    return "application/x-lotusscreencam";
                case ".SET":
                    return "application/set";
                case ".SGM":
                case ".SGML":
                    return "text/sgml";
                case ".SH":
                    return "application/x-sh";
                case ".SHAR":
                    return "application/x-shar";
                case ".SIT":
                    return "application/x-stuffit";
                case ".SKD":
                case ".SKM":
                case ".SKP":
                case ".SKT":
                    return "application/x-koan";
                case ".SMI":
                case ".SMIL":
                    return "application/smil";
                case ".SOL":
                    return "application/solids";
                case ".SPL":
                    return "application/x-futuresplash";
                case ".SRC":
                    return "application/x-wais-source";
                case ".STEP":
                case ".STP":
                    return "application/STEP";
                case ".STL":
                    return "application/SLA";
                case ".SV4CPIO":
                    return "application/x-sv4cpio";
                case ".SV4CRC":
                    return "application/x-sv4crc";
                case ".SWF":
                    return "application/x-shockwave-flash";
                case ".TAR":
                    return "application/x-tar";
                case ".TCL":
                    return "application/x-tcl";
                case ".TEX":
                    return "application/x-tex";
                case ".TEXI":
                case ".TEXINFO":
                    return "application/x-texinfo";
                case ".TIF":
                case ".TIFF":
                    return "image/tiff";
                case ".TSI":
                    return "audio/TSP-audio";
                case ".TSP":
                    return "application/dsptype";
                case ".TSV":
                    return "text/tab-separated-values";
                case ".UNV":
                    return "application/i-deas";
                case ".USTAR":
                    return "application/x-ustar";
                case ".VCD":
                    return "application/x-cdlink";
                case ".VDA":
                    return "application/vda";
                case ".VIV":
                case ".VIVO":
                    return "video/vnd.vivo";
                case ".WMV":
                    return "video/x-ms-wmv";
                case ".VRML":
                case ".WRL":
                    return "model/vrml";
                case ".WAV":
                    return "audio/x-wav";
                case ".XAP":
                    return "application/x-silverlight";
                case ".XBM":
                    return "image/x-xbitmap";
                case ".XLC":
                case ".XLL":
                case ".XLM":
                case ".XLS":
                case ".XLW":
                    return "application/vnd.ms-excel";
                case ".XML":
                    return "text/xml";
                case ".XPM":
                    return "image/x-xpixmap";
                case ".XWD":
                    return "image/x-xwindowdump";
                case ".ZIP":
                    return "application/zip";
                //Microsoft Office 2007
                case ".DOCX":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".DOCM":
                    return "application/vnd.ms-word.document.macroEnabled.12";
                case ".DOTX":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case ".DOTM":
                    return "application/vnd.ms-word.template.macroEnabled.12";
                case ".XLSX":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".XLSM":
                    return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case ".XLTX":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case ".XLTM":
                    return "application/vnd.ms-excel.template.macroEnabled.12";
                case ".XLSB":
                    return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case ".XLAM":
                    return "application/vnd.ms-excel.addin.macroEnabled.12";
                case ".PPTX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".PPTM":
                    return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case ".PPSX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case ".PPSM":
                    return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
                case ".POTX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case ".POTM":
                    return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case ".PPAM":
                    return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case ".SLDX":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slide";
                case ".SLDM":
                    return "application/vnd.ms-powerpoint.slide.macroEnabled.12";
                case ".THMX":
                    return "application/vnd.ms-officetheme";
                case ".ONETOC":
                case ".ONETOC2":
                case ".ONETMP":
                case ".ONEPKG":
                    return "application/onenote";
            }
            return Text;
        }

        /// <summary>
        /// Returns image format associated to the specified MIME type.
        /// </summary>
        /// <param name="mimeType">The string that contains the MIME type.</param>
        /// <returns>An image format, if it is found; otherwise null reference.</returns>
        public static ImageFormat GetImageFormat(string mimeType)
        {
            ImageCodecInfo[] imageCodecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo imageCodec = imageCodecs.First(codec => codec.MimeType == mimeType);
            if (imageCodec != null)
            {
                return new ImageFormat(imageCodec.FormatID);
            }

            return null;
        }

        #endregion
    }
}
