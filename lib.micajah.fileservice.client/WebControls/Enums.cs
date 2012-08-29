using System;
namespace Micajah.FileService.WebControls
{
    /// <summary>
    /// Defines the different rendering modes for the Micajah.FileService.WebControls.FileList.
    /// </summary>
    [Serializable]
    public enum FileListRenderingMode
    {
        /// <summary>
        /// A Micajah.FileService.WebControls.FileList control is rendered as simple grid view.
        /// </summary>
        GridView,

        /// <summary>
        /// A Micajah.FileService.WebControls.FileList control is rendered as thumbnails list.
        /// </summary>
        ThumbnailsList,

        /// <summary>
        /// A Micajah.FileService.WebControls.FileList control is rendered as Micajah.Common.WebControls.CommonGridView control.
        /// </summary>
        CommonGridView,

        /// <summary>
        /// A Micajah.FileService.WebControls.FileList control is rendered as files list where one file on top of the other file.
        /// </summary>
        FilesList
    }

    /// <summary>
    /// Represents the different sizes of icon.
    /// </summary>
    [Serializable]
    public enum IconSize
    {
        /// <summary>
        /// The size is 128 x 128 pixels.
        /// </summary>
        Bigger = 128,

        /// <summary>
        /// The size is 48 x 48 pixels.
        /// </summary>
        Normal = 48,

        /// <summary>
        /// The size is 32 x 32 pixels.
        /// </summary>
        Small = 32,

        /// <summary>
        /// The size is 16 x 16 pixels.
        /// </summary>
        Smaller = 16
    }

    /// <summary>
    /// Represents the different modes for the file selector of the upload control.
    /// </summary>
    [Serializable]
    public enum UploadFileSelectorMode
    {
        /// <summary>
        /// Single file selector.
        /// </summary>
        SingleFile = 0,

        /// <summary>
        /// Multi file selector.
        /// </summary>
        MultiFile = 1
    }
}
