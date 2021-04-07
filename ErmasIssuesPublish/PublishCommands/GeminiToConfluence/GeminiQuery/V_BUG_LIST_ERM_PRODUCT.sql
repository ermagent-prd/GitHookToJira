USE [GEMINI_CLOUD]
GO

/****** Object:  View [dbo].[V_BUG_LIST_ERM]    Script Date: 08/01/2021 17:54:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[V_BUG_LIST_ERM_PRODUCT]  
as
select * from [fn_GetBugsByProduct]('ERMBUG')
WHERE Status='Fixed'



GO


