USE [GEMINI_CLOUD]
GO

/****** Object:  UserDefinedFunction [dbo].[[fn_GetBugsByProduct]    Script Date: 08/01/2021 17:47:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE function [dbo].[fn_GetBugsByProduct](@project varchar(100))
returns table
as
return(

select 
	ID						= a.issueid
	,[Bug fixing]			= convert(varchar,c.datedata,111)
	,[Status]				= s.statusdesc
	,[Product Modules]		= b.TotModules
	,[Title]				= a.summary 
	,[Deliverable Version]	= v.TotModules
	,[Fixed In Build]		= replace(replace(replace(replace(
									ll.fielddata
								,'|',', '),',',', '),';','; '),'-','- ')
	,[Description]			= dbo.fn_StripHTML(a.longdesc) 
	,[Product]	= prod.fieldData
from 
	gemini_issues a
	LEFT OUTER JOIN (select * from fn_GetModulesDescr(299)) b on a.issueid=b.issueid 
	LEFT OUTER JOIN (select * from fn_GetVersionDescr(300)) v on a.issueid=v.issueid 
	LEFT OUTER JOIN (select * from [dbo].[gemini_customfielddata] where customfieldid=297) c on a.projectid=c.projectid and a.issueid=c.issueid 
	LEFT OUTER JOIN (select * from [dbo].[gemini_customfielddata] where customfieldid=301) d on a.projectid=d.projectid and a.issueid=d.issueid 
	LEFT OUTER JOIN (select * from [dbo].[gemini_customfielddata] where customfieldid=211) h on a.projectid=h.projectid and a.issueid=h.issueid 
	LEFT OUTER JOIN (select * from [dbo].[gemini_customfielddata] where customfieldid=212) ll on a.projectid=ll.projectid and a.issueid=ll.issueid 
	LEFT OUTER JOIN (select * from [dbo].[gemini_customfielddata] where customfieldid=293) prod on a.projectid=ll.projectid and a.issueid=prod.issueid
	INNER JOIN (select p1.projectid,p1.projectcode,s1.statusdesc,s1.templateid,s1.statusid from gemini_projects p1, gemini_issuestatus s1 where p1.templateid=s1.templateid) s on a.projectid=s.projectid and a.issuestatusid=s.statusid
	INNER JOIN (select p1.projectid,p1.projectcode,s1.resdesc,s1.templateid,s1.resolutionid from gemini_projects p1, gemini_issueresolutions s1 where p1.templateid=s1.templateid) res on a.projectid=res.projectid and a.issueresolutionid=res.resolutionid
where  
	a.projectid=(select projectid from gemini_projects where projectcode=@project)

)  

GO


