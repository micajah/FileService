using System;

namespace Micajah.FileService.WebControls
{
    public interface IUploadControl
    {
        /// <summary>
        /// Gets or sets the unique identifier of the organization.
        /// </summary>
        Guid OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the department.
        /// </summary>
        Guid DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        string DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the metadata database.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the the user that updates the files.
        /// </summary>
        string UpdatedBy { get; set; }
    }
}
