echo on

set Path=ProtoGen\protogen.exe

%Path%	-i:Request.proto	-o:OpenAPIModel\Request.cs

%Path%	-i:Response.proto	-o:OpenAPIModel\Respnse.cs

%Path%	-i:UserInfo.proto	-o:OpenAPIModel\UserInfo.cs

%Path%	-i:login.proto	-o:OpenAPIModel\login.cs

pause