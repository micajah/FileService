using System;

namespace Micajah.FileService.Dal
{
    public static class Helper
    {
        #region Public Methods

        public static Guid CreateGuid(string value)
        {
            Guid guid = Guid.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    guid = new Guid(value);
                }
                catch (ArgumentNullException) { }
                catch (FormatException) { }
                catch (OverflowException) { }
            }

            return guid;
        }

        public static Guid? CreateNullableGuid(string value)
        {
            Guid? guid = null;

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    guid = new Guid(value);
                }
                catch (ArgumentNullException) { }
                catch (FormatException) { }
                catch (OverflowException) { }
            }

            return guid;
        }

        public static bool IsNullOrDBNull(object value)
        {
            return ((value == null) || Convert.IsDBNull(value));
        }

        #endregion
    }
}
