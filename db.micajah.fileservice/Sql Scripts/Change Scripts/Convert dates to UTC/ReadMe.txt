To convert the value of datetime fields to UTC format please do the following step by step:

1. Run "StoredProcedures.sql" on file service server database to update the stored procedures.

2. Open "GetUTCDateTime.sql" and modify the "GetUTCDateTime" function: specify the offset of your's timezone to @offset variable.

3. Create this function in file service server database.

4. Run "Server.sql" on database to fix the dates.

5. Delete "GetUTCDateTime" function from all databases. Use first part of the script from "GetUTCDateTime.sql".
