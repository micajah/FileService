using System;

namespace Micajah.FileService.Bll
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
            switch (fileExtension.ToUpperInvariant())
            {
                case ".AI":
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
                    return "audio/basic";
                case ".AVI":
                    return "video/x-msvideo";
                case ".BCPIO":
                    return "application/x-bcpio";
                case ".BIN":
                    return "application/octet-stream";
                case ".BMP":
                    return "image/bmp";
                case ".CCAD":
                    return "application/clariscad";
                case ".CDF":
                    return "application/x-netcdf";
                case ".CLASS":
                    return "application/octet-stream";
                case ".CPIO":
                    return "application/x-cpio";
                case ".CPT":
                    return "application/mac-compactpro";
                case ".CSH":
                    return "application/x-csh";
                case ".CSS":
                    return "text/css";
                case ".DCR":
                    return "application/x-director";
                case ".DIR":
                    return "application/x-director";
                case ".DMS":
                    return "application/octet-stream";
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
                case ".DXR":
                    return "application/x-director";
                case ".EPS":
                    return "application/postscript";
                case ".ETX":
                    return "text/x-setext";
                case ".EXE":
                    return "application/octet-stream";
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
                    return "audio/midi";
                case ".LATEX":
                    return "application/x-latex";
                case ".LHA":
                    return "application/octet-stream";
                case ".LSP":
                    return "application/x-lisp";
                case ".LZH":
                    return "application/octet-stream";
                case ".MAN":
                    return "application/x-troff-man";
                case ".ME":
                    return "application/x-troff-me";
                case ".MESH":
                case ".MSH":
                case ".SILO":
                    return "model/mesh";
                case ".MID":
                case ".MIDI":
                    return "audio/midi";
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
                case ".MS":
                    return "application/x-troff-ms";
                case ".NC":
                    return "application/x-netcdf";
                case ".ODA":
                    return "application/oda";
                case ".PBM":
                    return "image/x-portable-bitmap";
                case ".PDB":
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
                    return "application/mspowerpoint";
                case ".PPM":
                    return "image/x-portable-pixmap";
                case ".PPS":
                case ".PPT":
                case ".PPZ":
                    return "application/mspowerpoint";
                case ".PRE":
                    return "application/x-freelance";
                case ".PRT":
                    return "application/pro_eng";
                case ".PS":
                    return "application/postscript";
                case ".RA":
                    return "audio/x-realaudio";
                case ".RAM":
                    return "audio/x-pn-realaudio";
                case ".RAS":
                    return "image/cmu-raster";
                case ".RGB":
                    return "image/x-rgb";
                case ".RM":
                    return "audio/x-pn-realaudio";
                case ".ROFF":
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
                case ".SND":
                    return "audio/basic";
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
                case ".T":
                    return "application/x-troff";
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
                case ".TR":
                    return "application/x-troff";
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
                    return "video/vnd.vivo";
                case ".VIVO":
                    return "video/vnd.vivo";
                case ".VRML":
                    return "model/vrml";
                case ".WAV":
                    return "audio/x-wav";
                case ".WRL":
                    return "model/vrml";
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
                case ".XYZ":
                    return "chemical/x-pdb";
                case ".ZIP":
                    return "application/zip";
            }
            return Text;
        }

        #endregion
    }
}
