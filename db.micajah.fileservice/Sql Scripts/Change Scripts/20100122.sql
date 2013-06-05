UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
WHERE [FileExtension] = '.docx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = '.docm'
WHERE [FileExtension] = 'application/vnd.ms-word.document.macroEnabled.12'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = '.dotx'
WHERE [FileExtension] = 'application/vnd.openxmlformats-officedocument.wordprocessingml.template'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-word.template.macroEnabled.12'
WHERE [FileExtension] = '.dotm'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
WHERE [FileExtension] = '.xlsx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-excel.sheet.macroEnabled.12'
WHERE [FileExtension] = '.xlsm'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.openxmlformats-officedocument.spreadsheetml.template'
WHERE [FileExtension] = '.xltx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-excel.template.macroEnabled.12'
WHERE [FileExtension] = '.xltm'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-excel.sheet.binary.macroEnabled.12'
WHERE [FileExtension] = '.xlsb'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-excel.addin.macroEnabled.12'
WHERE [FileExtension] = '.xlam'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.openxmlformats-officedocument.presentationml.presentation'
WHERE [FileExtension] = '.pptx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-powerpoint.presentation.macroEnabled.12'
WHERE [FileExtension] = '.pptm'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.openxmlformats-officedocument.presentationml.slideshow'
WHERE [FileExtension] = '.ppsx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-powerpoint.slideshow.macroEnabled.12'
WHERE [FileExtension] = '.ppsm'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.openxmlformats-officedocument.presentationml.template'
WHERE [FileExtension] = '.potx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-powerpoint.template.macroEnabled.12'
WHERE [FileExtension] = '.potm'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-powerpoint.addin.macroEnabled.12'
WHERE [FileExtension] = '.ppam'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.openxmlformats-officedocument.presentationml.slide'
WHERE [FileExtension] = '.sldx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-powerpoint.slide.macroEnabled.12'
WHERE [FileExtension] = '.sldm'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/vnd.ms-officetheme'
WHERE [FileExtension] = '.thmx'

UPDATE [dbo].[FileExtension] 
SET [MimeType] = 'application/onenote'
WHERE [FileExtension] IN ('.onetoc', '.onetoc2', '.onetmp', '.onepkg')
