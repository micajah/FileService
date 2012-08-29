using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Micajah.FileService.Tools.RestoreFiles
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

        private static ReadOnlyCollection<string> s_ImageExtensions;
        private static ReadOnlyCollection<string> s_VideoExtensions;

        #endregion

        #region Public Properties

        /// <summary>
        /// The collection of the image files's extensions.
        /// </summary>
        public static ReadOnlyCollection<string> ImageExtensions
        {
            get
            {
                if (s_ImageExtensions == null)
                    s_ImageExtensions = new ReadOnlyCollection<string>(new string[] { ".bmp", ".gif", ".ief", ".jpg", ".pbm", ".png", ".pnm", ".ppm", ".ras", ".rgb", ".tif", ".tiff", ".xbm", ".xpm", ".xwd" });
                return s_ImageExtensions;
            }
        }

        /// <summary>
        /// The collection of the video files's extensions.
        /// </summary>
        public static ReadOnlyCollection<string> VideoExtensions
        {
            get
            {
                if (s_VideoExtensions == null)
                    s_VideoExtensions = new ReadOnlyCollection<string>(new string[] { ".asf", ".avi", ".fli", ".mov", ".movie", ".mp4", ".mpe", ".mpeg", ".mpg", ".swf", ".viv", ".vivo", ".wmv" });
                return s_VideoExtensions;
            }
        }

        #endregion

        #region Private Methods

        private static string GetAudioExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "AUDIO/X-AIFF":
                    return ".aif";
                case "AUDIO/BASIC":
                    return ".snd";
                case "AUDIO/MIDI":
                    return ".mid";
                case "AUDIO/MPEG":
                    return ".MP3";
                case "AUDIO/X-REALAUDIO":
                    return ".ra";
                case "AUDIO/X-PN-REALAUDIO":
                    return ".ram";
                case "AUDIO/X-PN-REALAUDIO-PLUGIN":
                    return ".rpm";
                case "AUDIO/TSP-AUDIO":
                    return ".tsi";
                case "AUDIO/X-WAV":
                    return ".wav";
            }
            return null;
        }

        private static string GetApplicationExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "APPLICATION/POSTSCRIPT":
                    return ".ps";
                case "APPLICATION/X-BCPIO":
                    return ".bcpio";
                case "APPLICATION/OCTET-STREAM":
                    return ".bin";
                case "APPLICATION/CLARISCAD":
                    return ".ccad";
                case "APPLICATION/X-NETCDF":
                    return ".cdf";
                case "APPLICATION/X-CPIO":
                    return ".cpio";
                case "APPLICATION/MAC-COMPACTPRO":
                    return ".cpt";
                case "APPLICATION/X-CSH":
                    return ".csh";
                case "APPLICATION/X-DIRECTOR":
                    return ".dir";
                case "APPLICATION/DRAFTING":
                    return ".drw";
                case "APPLICATION/X-DVI":
                    return ".dvi";
                case "APPLICATION/ACAD":
                    return ".dwg";
                case "APPLICATION/DXF":
                    return ".dxf";
                case "APPLICATION/ANDREW-INSET":
                    return ".ez";
                case "APPLICATION/X-GTAR":
                    return ".gtar";
                case "APPLICATION/X-GZIP":
                    return ".gz";
                case "APPLICATION/X-HDF":
                    return ".hdf";
                case "APPLICATION/MAC-BINHEX40":
                    return ".hqx";
                case "APPLICATION/X-IPSCRIPT":
                    return ".ips";
                case "APPLICATION/X-IPIX":
                    return ".ipx";
                case "APPLICATION/X-JAVASCRIPT":
                    return ".js";
                case "APPLICATION/X-LATEX":
                    return ".latex";
                case "APPLICATION/X-LISP":
                    return ".lsp";
                case "APPLICATION/X-TROFF-MAN":
                    return ".man";
                case "APPLICATION/X-TROFF-ME":
                    return ".me";
                case "APPLICATION/VND.MIF":
                    return ".mif";
                case "APPLICATION/X-TROFF-MS":
                    return ".ms";
                case "APPLICATION/ODA":
                    return ".oda";
                case "APPLICATION/PDF":
                    return ".pdf";
                case "APPLICATION/X-CHESS-PGN":
                    return ".pgn";
                case "APPLICATION/X-FREELANCE":
                    return ".pre";
                case "APPLICATION/PRO_ENG":
                    return ".prt";
                case "APPLICATION/X-TROFF":
                    return ".roff";
                case "APPLICATION/X-LOTUSSCREENCAM":
                    return ".scm";
                case "APPLICATION/SET":
                    return ".set";
                case "APPLICATION/X-SH":
                    return ".sh";
                case "APPLICATION/X-SILVERLIGHT":
                    return ".xap";
                case "APPLICATION/X-SHAR":
                    return ".shar";
                case "APPLICATION/X-STUFFIT":
                    return ".sit";
                case "APPLICATION/X-KOAN":
                    return ".skd";
                case "APPLICATION/SMIL":
                    return ".smi";
                case "APPLICATION/SOLIDS":
                    return ".sol";
                case "APPLICATION/X-FUTURESPLASH":
                    return ".spl";
                case "APPLICATION/X-WAIS-SOURCE":
                    return ".src";
                case "APPLICATION/STEP":
                    return ".stp";
                case "APPLICATION/SLA":
                    return ".stl";
                case "APPLICATION/X-SV4CPIO":
                    return ".sv4cpio";
                case "APPLICATION/X-SV4CRC":
                    return ".sv4crc";
                case "APPLICATION/X-SHOCKWAVE-FLASH":
                    return ".swf";
                case "APPLICATION/X-TAR":
                    return ".tar";
                case "APPLICATION/X-TCL":
                    return ".tcl";
                case "APPLICATION/X-TEX":
                    return ".tex";
                case "APPLICATION/X-TEXINFO":
                    return ".texi";
                case "APPLICATION/DSPTYPE":
                    return ".tsp";
                case "APPLICATION/I-DEAS":
                    return ".unv";
                case "APPLICATION/X-USTAR":
                    return ".ustar";
                case "APPLICATION/X-CDLINK":
                    return ".vcd";
                case "APPLICATION/VDA":
                    return ".vda";
                case "APPLICATION/ZIP":
                    return ".zip";
            }
            return GetMicrosoftOfficeExtension(mimeType);
        }

        private static string GetMicrosoftOfficeExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "APPLICATION/MSWORD":
                    return ".doc";
                case "APPLICATION/MSPOWERPOINT":
                    return ".ppt";
                case "APPLICATION/VND.MS-EXCEL":
                    return ".xls";
            }
            return GetMicrosoftOffice2007Extension(mimeType);
        }

        private static string GetMicrosoftOffice2007Extension(string mimeType)
        {
            switch (mimeType)
            {
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.WORDPROCESSINGML.DOCUMENT":
                    return ".docx";
                case "APPLICATION/VND.MS-WORD.DOCUMENT.MACROENABLED.12":
                    return ".docm";
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.WORDPROCESSINGML.TEMPLATE":
                    return ".dotx";
                case "APPLICATION/VND.MS-WORD.TEMPLATE.MACROENABLED.12":
                    return ".dotm";
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.SPREADSHEETML.SHEET":
                    return ".xlsx";
                case "APPLICATION/VND.MS-EXCEL.SHEET.MACROENABLED.12":
                    return ".xlsm";
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.SPREADSHEETML.TEMPLATE":
                    return ".xltx";
                case "APPLICATION/VND.MS-EXCEL.TEMPLATE.MACROENABLED.12":
                    return ".xltm";
                case "APPLICATION/VND.MS-EXCEL.SHEET.BINARY.MACROENABLED.12":
                    return ".xlsb";
                case "APPLICATION/VND.MS-EXCEL.ADDIN.MACROENABLED.12":
                    return ".xlam";
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.PRESENTATIONML.PRESENTATION":
                    return ".pptx";
                case "APPLICATION/VND.MS-POWERPOINT.PRESENTATION.MACROENABLED.12":
                    return ".pptm";
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.PRESENTATIONML.SLIDESHOW":
                    return ".ppsx";
                case "APPLICATION/VND.MS-POWERPOINT.SLIDESHOW.MACROENABLED.12":
                    return ".ppsm";
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.PRESENTATIONML.TEMPLATE":
                    return ".potx";
                case "APPLICATION/VND.MS-POWERPOINT.TEMPLATE.MACROENABLED.12":
                    return ".potm";
                case "APPLICATION/VND.MS-POWERPOINT.ADDIN.MACROENABLED.12":
                    return ".ppam";
                case "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.PRESENTATIONML.SLIDE":
                    return ".sldx";
                case "APPLICATION/VND.MS-POWERPOINT.SLIDE.MACROENABLED.12":
                    return ".sldm";
                case "APPLICATION/VND.MS-OFFICETHEME":
                    return ".thmx";
                case "APPLICATION/ONENOTE":
                    return ".onetoc";
            }
            return null;
        }

        private static string GetImageExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "IMAGE/BMP":
                    return ".bmp";
                case "IMAGE/GIF":
                    return ".gif";
                case "IMAGE/IEF":
                    return ".ief";
                case "IMAGE/JPEG":
                    return ".jpg";
                case "IMAGE/X-PORTABLE-BITMAP":
                    return ".pbm";
                case "IMAGE/X-PORTABLE-GRAYMAP":
                    return ".pgm";
                case "IMAGE/PNG":
                    return ".png";
                case "IMAGE/X-PORTABLE-ANYMAP":
                    return ".pnm";
                case "IMAGE/X-PORTABLE-PIXMAP":
                    return ".ppm";
                case "IMAGE/CMU-RASTER":
                    return ".ras";
                case "IMAGE/X-RGB":
                    return ".rgb";
                case "IMAGE/TIFF":
                    return ".tif";
                case "IMAGE/X-XBITMAP":
                    return ".xbm";
                case "IMAGE/X-XPIXMAP":
                    return ".xpm";
                case "IMAGE/X-XWINDOWDUMP":
                    return ".xwd";
            }
            return null;
        }

        private static string GetTextExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "TEXT/PLAIN":
                    return ".txt";
                case "TEXT/CSS":
                    return ".css";
                case "TEXT/X-SETEXT":
                    return ".etx";
                case "TEXT/HTML":
                    return ".htm";
                case "TEXT/RTF":
                    return ".rtf";
                case "TEXT/RICHTEXT":
                    return ".rtx";
                case "TEXT/SGML":
                    return ".sgm";
                case "TEXT/TAB-SEPARATED-VALUES":
                    return ".tsv";
                case "TEXT/XML":
                    return ".xml";
            }
            return null;
        }

        private static string GetVideoExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "VIDEO/X-MSVIDEO":
                    return ".avi";
                case "VIDEO/X-FLI":
                    return ".fli";
                case "VIDEO/QUICKTIME":
                    return ".qt";
                case "VIDEO/X-SGI-MOVIE":
                    return ".movie";
                case "VIDEO/MP4":
                    return ".mp4";
                case "VIDEO/MPEG":
                    return ".mpg";
                case "VIDEO/VND.VIVO":
                    return ".viv";
                case "VIDEO/X-MS-ASF":
                    return ".asf";
                case "VIDEO/X-MS-WMV":
                    return ".wmv";
            }
            return null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified MIME type is flash.
        /// </summary>
        /// <param name="mimeType">The string that contains the MIME type to check.</param>
        /// <returns>true, if the specified MIME type is flash; otherwise, false.</returns>
        public static bool IsFlash(string mimeType)
        {
            return (string.Compare(mimeType, "application/x-shockwave-flash", StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Determines whether the specified MIME type is image type.
        /// </summary>
        /// <param name="mimeType">The string that contains the MIME type to check.</param>
        /// <returns>true, if the specified MIME type is image type; otherwise, false.</returns>
        public static bool IsImageType(string mimeType)
        {
            if (!string.IsNullOrEmpty(mimeType))
                return mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
            return false;
        }

        /// <summary>
        /// Determines whether the specified MIME type is video type.
        /// </summary>
        /// <param name="mimeType">The string that contains the MIME type to check.</param>
        /// <returns>true, if the specified MIME type is video type; otherwise, false.</returns>
        public static bool IsVideoType(string mimeType)
        {
            if (!string.IsNullOrEmpty(mimeType))
                return mimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
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
        /// Returns the MIME type array for specified file extension list.
        /// </summary>
        /// <param name="extensions">The file extension list.</param>
        /// <returns>The MIME type array.</returns>
        public static string[] GetMimeTypes(IEnumerable<string> extensions)
        {
            List<string> mimeTypes = new List<string>();
            foreach (string ext in extensions)
            {
                mimeTypes.Add(MimeType.GetMimeType(ext));
            }
            return mimeTypes.ToArray();
        }

        /// <summary>
        /// Returns a file extension by specified MIME type.
        /// </summary>
        /// <param name="mimeType">The string that contains the MIME type.</param>
        /// <returns>The file extension for the MIME type or empty string, if the MIME type is not found.</returns>
        public static string GetFileExtension(string mimeType)
        {
            if (mimeType == null) return null;

            mimeType = mimeType.ToUpperInvariant();
            string extension = GetApplicationExtension(mimeType);
            if (extension == null)
            {
                extension = GetImageExtension(mimeType);
                if (extension == null)
                {
                    extension = GetAudioExtension(mimeType);
                    if (extension == null)
                    {
                        extension = GetVideoExtension(mimeType);
                        if (extension == null)
                        {
                            extension = GetTextExtension(mimeType);
                            if (extension == null)
                            {
                                switch (mimeType)
                                {
                                    case "CHEMICAL/X-PDB":
                                        return ".pdb";
                                    case "MODEL/IGES":
                                        return ".igs";
                                    case "MODEL/MESH":
                                        return ".msh";
                                    case "MODEL/VRML":
                                        return ".vrml";
                                    case "WWW/MIME":
                                        return ".mime";
                                    case "X-CONFERENCE/X-COOLTALK":
                                        return ".ice";
                                }
                            }
                        }
                    }
                }
            }
            return ((extension == null) ? string.Empty : extension);
        }

        #endregion
    }
}
