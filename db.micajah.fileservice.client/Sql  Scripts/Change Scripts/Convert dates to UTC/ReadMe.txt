To convert the value of datetime fields to UTC format please do the following step by step:

1. Open "GetUTCDateTime.sql" and modify the "GetUTCDateTime" function: specify the offset of your's timezone to @offset variable.

2. Create this function in metadata database.

3. Run "Client.sql" on metadata database to fix the dates.

4. Delete "GetUTCDateTime" function from all databases. Use first part of the script from "GetUTCDateTime.sql".
